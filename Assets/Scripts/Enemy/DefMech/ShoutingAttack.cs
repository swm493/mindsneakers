using UnityEngine;
using System.Collections;

public class ShoutingAttack : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private DefMechanism dm;

    [SerializeField] private Collider2D hitRange;
    [SerializeField] private GameObject shoutingEffect;

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
        yield return new WaitForSeconds(0.9f); //선딜
        GameObject effect = Instantiate(shoutingEffect, transform.position + new Vector3(0, -1), Quaternion.identity);
        Collider2D col = Physics2D.OverlapCircle(hitRange.bounds.center, hitRange.bounds.size.x/2, LayerMask.GetMask("Player"));
        if (col != null)
        {
            player.GetComponent<PlayerController>().TakeDamage(3);
        }
        yield return new WaitForSeconds(0.7f); //공격 중
        Destroy(effect);
        dm.StopAttack(6f);
    }
}
