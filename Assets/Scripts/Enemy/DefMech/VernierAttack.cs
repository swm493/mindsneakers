using UnityEngine;
using System.Collections;

public class VernierAttack : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private EnemyController ec;
    private DefMechanism dm;

    [SerializeField] private Collider2D calliper;
    private EnterTrigger calliper_et;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ec = GetComponent<EnemyController>();
        dm = GetComponent<DefMechanism>();
        calliper_et = calliper.GetComponent<EnterTrigger>();
    }

    private void Update()
    {
        calliper.transform.localPosition = new Vector3(0.054f, -0.025f);
    }

    public IEnumerator Attack(Collider2D player)
    {
        rb.linearVelocityX = 0;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(1f); //선딜
        if (ec.stunned) yield break;
        rb.linearVelocityX = 9f * ec.SightXInt();

        
        float startTime = Time.time;
        while (Time.time - startTime < 0.25f)
        {
            Collider2D col = calliper_et.GetCollider();
            if(col && col.CompareTag("Player")) player.GetComponent<PlayerController>().TakeDamage(3);
            yield return null;
        }
        yield return new WaitForSeconds(0.25f);
        dm.StopAttack(1f);
    }
}
