using UnityEngine;
using System.Collections;

public class MessageSender : MonoBehaviour
{
    private bool isWorking = false;

    private Animator anim;
    private EnterTrigger et;
    private EnemyController ec;

    [SerializeField] private new GameObject light;

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
    }

    private IEnumerator SendMessageToEnemy()
    {
        anim.SetTrigger("Send");
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
