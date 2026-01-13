using UnityEngine;
using System.Collections;

public class BeamShooter : MonoBehaviour
{
    private bool isBeaming = false;
    private bool isAttacking = false;

    private Animator anim;
    private EnterTrigger et;
    private Rigidbody2D rb;
    private EnemyController ec;

    [SerializeField] private new GameObject light;
    [SerializeField] private GameObject beamPrefab;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        et = light.GetComponent<EnterTrigger>();
        rb = GetComponent<Rigidbody2D>();
        ec = GetComponent<EnemyController>();
    }

    private void Update()
    {
        if (!ec.isEA)
        {
            Collider2D col = et.GetCollider();
            if(col && col.CompareTag("Player") && !isBeaming)
            {
                StartCoroutine(Beeeeeeeeeeeam(col));
                isBeaming = true;
            }
        }
    }

    private IEnumerator Beeeeeeeeeeeam(Collider2D player)
    {
        rb.linearVelocityY = 10f;
        yield return new WaitForSeconds(0.4f);
        if (ec.isEA) yield break;
        anim.SetTrigger("EatingChocolate");
        yield return new WaitForSeconds(1f);
        if (ec.isEA) yield break;
        rb.linearVelocityX = -3f;
        yield return new WaitForSeconds(0.8f);
        if (ec.isEA) yield break;
        rb.linearVelocityX = 3f;
        yield return new WaitForSeconds(0.8f);
        rb.linearVelocityX = 0f;
        if (ec.isEA) yield break;

        RaycastHit2D hit = Physics2D.Raycast(player.transform.position + new Vector3(0, 2f, 0), Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground", "Platform"));
        GameObject beam = Instantiate(beamPrefab, hit.point + new Vector2(0, 3.5f), Quaternion.identity);
        SpriteRenderer sr = beam.GetComponent<SpriteRenderer>();
        StartCoroutine(HitByBeam(player, beam));
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 2; i++)
        {
            sr.color = new Color(1f, 1f, 0.2f, 1f);
            isAttacking = true;
            yield return new WaitForSeconds(1f);
            isAttacking = false;
            sr.color = new Color(1f, 1f, 0.2f, 0.4f);
            yield return new WaitForSeconds(0.25f);
            hit = Physics2D.Raycast(player.transform.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground", "Platform"));
            beam.transform.position = hit.point + new Vector2(0, 3.5f);
            yield return new WaitForSeconds(0.25f);
        }
        sr.color = new Color(1f, 1f, 0.2f, 1f);
        isAttacking = true;
        yield return new WaitForSeconds(1f);
        isAttacking = false;
        Destroy(beam);

        yield return new WaitForSeconds(3f);
        isBeaming = false;
    }

    private IEnumerator HitByBeam(Collider2D player, GameObject beam)
    {
        while (isBeaming)
        {
            if (isAttacking)
            {
                Collider2D col = beam.GetComponent<EnterTrigger>().GetCollider();
                if(col && col.CompareTag("Player"))
                {
                    player.GetComponent<PlayerController>().TakeDamage(3);
                }
            }
            yield return null;
        }
    }
}
