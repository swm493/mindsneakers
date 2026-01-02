using UnityEngine;
using System.Collections;

public class ShoutingAttack : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private DefMechanism dm;

    [SerializeField] private Collider2D hitRange;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        dm = GetComponent<DefMechanism>();
    }

    public IEnumerator Attack(Collider2D player)
    {
        rb.linearVelocityX = 0;
        anim.SetTrigger("Attack"); //웅크리다 소리지르기
        yield return new WaitForSeconds(1f); //선딜
        Collider2D col = Physics2D.OverlapCircle(hitRange.bounds.center, hitRange.bounds.size.x/2, LayerMask.GetMask("Player"));
        if (col != null)
        {
            player.GetComponent<PlayerController>().TakeDamage(3);
        }
        dm.StopAttack(6f);
    }
}
