using UnityEngine;
using System.Collections;

public class VernierAttack : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private EnemyController ec;
    private DefMechanism dm;

    [SerializeField] private Collider2D calliper;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ec = GetComponent<EnemyController>();
        dm = GetComponent<DefMechanism>();
    }

    private void Update()
    {
        calliper.transform.localPosition = new Vector3(0.054f, -0.025f);
    }

    public IEnumerator Attack(Collider2D player)
    {
        rb.linearVelocityX = 0;
        anim.SetTrigger("VernierAttack");
        yield return new WaitForSeconds(1f); //선딜
        if (ec.isEA) yield break;
        rb.linearVelocityX = 9f * ec.SightXInt();
        
        float startTime = Time.time;
        while (Time.time - startTime < 0.3f)
        {
            calliper.transform.localRotation = Quaternion.Euler(0, 0, -400f * (Time.time - startTime));
            Collider2D col = calliper.GetComponent<EnterTrigger>().GetCollider();
            if(col && col.CompareTag("Player")) player.GetComponent<PlayerController>().TakeDamage(3);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        startTime = Time.time;
        while (Time.time - startTime < 0.5f)
        {
            calliper.transform.localRotation = Quaternion.Euler(0, 0, -120f + 240f * (Time.time - startTime));
            yield return null;
        }
        calliper.transform.localRotation = Quaternion.Euler(0, 0, 0);
        yield return new WaitForSeconds(0.5f); //후딜
        dm.StopAttack(1f);
    }
}
