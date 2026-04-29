using UnityEngine;
using System.Collections;

public class MessageSender : MonoBehaviour
{
    private bool isWorking = false;

    private Animator anim;
    private EnterTrigger et;
    private EnemyController ec;

    private bool EAOffset = false;

    [SerializeField] private new GameObject light;
    [SerializeField] private AudioClip sendSfx;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        ec = GetComponent<EnemyController>();
        et = light.GetComponent<EnterTrigger>();
    }

    private void Update() //플레이어가 빛 범위에 들어왔을 때
    {
        if (!ec.stunned)
        {
            Collider2D col = et.GetCollider();
            if(col && col.CompareTag("Player") && !isWorking && !col.GetComponent<PlayerMove>().onBush)
            {
                StartCoroutine(SendMessageToEnemy());
                isWorking = true;
            }
        }
        else if (!EAOffset)
        {
            EAOffset = true;
            GetComponent<Rigidbody2D>().gravityScale = 0;
            transform.localPosition = new Vector3(-0.7f * ec.SightXInt(), -1.2f, 0);
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    private IEnumerator SendMessageToEnemy()
    {
        anim.SetTrigger("Send");
        AudioManager.Instance.Play(sendSfx, 0.8f);
        yield return new WaitForSeconds(3f);

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 13f, LayerMask.GetMask("Enemy"));
        foreach (Collider2D enemy in enemies)
        {
            if(enemy.GetComponent<DefMechanism>() != null) enemy.GetComponent<DefMechanism>().StartFollowing();
        }

        yield return new WaitForSeconds(3f);
        isWorking = false;
    }
}
