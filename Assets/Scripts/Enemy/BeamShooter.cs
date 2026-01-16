using UnityEngine;
using System.Collections;

public class BeamShooter : MonoBehaviour
{
    private bool isBeaming = false;
    private bool isAttacking = false;
    private bool EAOffset = false;

    private Animator anim, tw_anim;
    private EnterTrigger et;
    private EnemyController ec;

    [SerializeField] private new GameObject light;
    [SerializeField] private GameObject beamPrefab;
    [SerializeField] private GameObject tower;
    [SerializeField] private AudioClip beamSfx;
    [SerializeField] private AudioClip buttonSfx;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        tw_anim = tower.GetComponent<Animator>();
        et = light.GetComponent<EnterTrigger>();
        ec = GetComponent<EnemyController>();
    }

    private void Update()
    {
        if (!ec.stunned && !isBeaming)
        {
            Collider2D col = et.GetCollider();
            if(col && col.CompareTag("Player") && !col.GetComponent<PlayerMove>().onBush)
            {
                StartCoroutine(Beeeeeeeeeeeam(col));
                isBeaming = true;
            }
        }
        else if (!EAOffset && ec.stunned)
        {
            EAOffset = true;
            GetComponent<Rigidbody2D>().gravityScale = 0;
            transform.localPosition = new Vector3(0, 2f, 0);
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    private IEnumerator Beeeeeeeeeeeam(Collider2D player)
    {
        anim.SetBool("IsBeaming", true);
        tw_anim.SetBool("IsBeaming", true);
        yield return new WaitForSeconds(1f);
        anim.SetBool("IsBeaming", false);
        yield return new WaitForSeconds(1.2f);
        AudioManager.Instance.Play(buttonSfx, 0.5f);
        yield return new WaitForSeconds(0.8f);

        RaycastHit2D hit = Physics2D.Raycast(player.transform.position + new Vector3(0, 3.5f, 0), Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground", "Platform"));
        GameObject beam = Instantiate(beamPrefab, hit.point + new Vector2(0, 3.5f), Quaternion.identity);
        SpriteRenderer sr = beam.GetComponent<SpriteRenderer>();
        StartCoroutine(HitByBeam(player, beam));
        sr.color = new Color(1f, 1f, 0.2f, 0.4f);
        yield return new WaitForSeconds(0.75f);
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.25f);
            hit = Physics2D.Raycast(player.transform.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground", "Platform"));
            beam.transform.position = hit.point + new Vector2(0, 3.5f);
            AudioManager.Instance.Play(beamSfx, 0.5f);
            yield return new WaitForSeconds(0.25f);
            sr.color = new Color(1f, 1f, 0.2f, 1f);
            isAttacking = true;
            yield return new WaitForSeconds(1f);
            isAttacking = false;
            sr.color = new Color(1f, 1f, 0.2f, 0.4f);
        }
        Destroy(beam);
        tw_anim.SetBool("IsBeaming", false);
    
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
