using UnityEngine;
using System.Collections;

public class VernierAttack : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private EnemyController ec;
    private DefMechanism dm;

    [SerializeField] private Collider2D hitRange;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ec = GetComponent<EnemyController>();
        dm = GetComponent<DefMechanism>();
    }

    public IEnumerator Attack(Collider2D player)
    {
        rb.linearVelocityX = 0;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(1f); //공격 모션 시간
        rb.linearVelocityX = 10f * ec.SightXInt();
        yield return new WaitForSeconds(0.15f);
        rb.linearVelocityX = 0;
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Player"));
        /*if (Physics2D.OverlapCollider(hitRange, filter) != null)
        {
            player.GetComponent<PlayerController>().TakeDamage(3);
        }*/
        //무기에 닿으면 피 닳는 걸로 할 거임
        dm.StopAttack(1f);
    }
}
