using UnityEngine;
using System.Collections;

public class MessageSender : MonoBehaviour
{
    private bool isWorking = false;

    private Animator anim;
    private EnterTrigger et;
    private Rigidbody2D rb;
    private EnemyController ec;

    [SerializeField] private new GameObject light;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        ec = GetComponent<EnemyController>();
        et = light.GetComponent<EnterTrigger>();
    }

    private void Update() //플레이어가 빛 범위에 들어왔을 때
    {
        if (!ec.isEA)
        {
            Collider2D col = et.GetCollider();
            if(col && col.CompareTag("Player") && !isWorking)
            {
                if (col.transform.position.x - transform.position.x > 0)
                    ec.lookingDirection = Vector2.right;
                else
                    ec.lookingDirection = Vector2.left;
                StartCoroutine(SendMessageToEnemy());
                isWorking = true;
            }
        }
    }

    private IEnumerator SendMessageToEnemy()
    {
        rb.linearVelocityY = 10f;
        yield return new WaitForSeconds(1f);
        if (ec.isEA) yield break;
        ec.lookingDirection = new Vector2(-ec.SightXInt(), 0f);
        rb.linearVelocityX = 7f * ec.SightXInt();
        yield return new WaitForSeconds(1f);
        rb.linearVelocityX = 0f;
        if (ec.isEA) yield break;
        anim.SetTrigger("EatingChocolate");
        yield return new WaitForSeconds(1f);


        anim.SetTrigger("SendMessage");
        yield return new WaitForSeconds(1f);
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 13f, LayerMask.GetMask("Enemy"));
        foreach (Collider2D enemy in enemies)
        {
            if(enemy.GetComponent<DefMechanism>() != null) enemy.GetComponent<DefMechanism>().StartFollowing();
        }
    }
}
