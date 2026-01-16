using UnityEngine;
using System.Collections;

public class BadWordAttack : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private DefMechanism dm;
    private EnemyController ec;

    [SerializeField] private GameObject BadWordBeam;
    [SerializeField] private GameObject BadWordParticle;
    [SerializeField] private AudioClip badWordSfx;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        dm = GetComponent<DefMechanism>();
        ec = GetComponent<EnemyController>();
    }

    public IEnumerator Attack(Collider2D player)
    {
        rb.linearVelocityX = 0;
        anim.SetTrigger("Attack"); //확성기 들기
        yield return new WaitForSeconds(0.5f); //선딜
        AudioManager.Instance.Play(badWordSfx, 0.5f);
        yield return new WaitForSeconds(0.5f);

        int shootDirection = ec.SightXInt();
        Vector3 spawnOffset = new Vector3(0.5f * shootDirection, 0.2f);
        GameObject laser = Instantiate(BadWordBeam, transform.position + spawnOffset, Quaternion.identity);
        laser.transform.localScale = new Vector3(0.6f * shootDirection, laser.transform.localScale.y);
        
        float startTime = Time.time;
        StartCoroutine(SpawnParticle(spawnOffset, shootDirection));
        while (Time.time - startTime < 1f)
        {
            rb.linearVelocityX = -3f * shootDirection;
            laser.transform.position = transform.position + spawnOffset;
            laser.transform.localScale += new Vector3(Time.deltaTime * 30f, 0) * shootDirection;
            Collider2D col = laser.GetComponent<EnterTrigger>().GetCollider();
            if(col && col.CompareTag("Player")) player.GetComponent<PlayerController>().TakeDamage(2);
            yield return null;
        }

        rb.linearVelocityX = 0;
        Destroy(laser);
        yield return new WaitForSeconds(1f); //후딜
        
        dm.StopAttack(4f);
    }

    private IEnumerator SpawnParticle(Vector3 spawnOffset, int shootDirection)
    {
        GameObject[] particles = new GameObject[20];
        for (int i = 0; i < 20; i++)
        {
            particles[i] = Instantiate(BadWordParticle, transform.position + spawnOffset 
                + i/20f * new Vector3(shootDirection, 0) * 40f, Quaternion.identity);
            for (int j = 0; j < i; j++)
            {
                particles[j].transform.position = transform.position + spawnOffset + j/20f * new Vector3(shootDirection, 0) * 40f;
            }
            yield return new WaitForSeconds(0.05f);
        }
        foreach (GameObject p in particles)
        {
            Destroy(p);
        }
    }
}