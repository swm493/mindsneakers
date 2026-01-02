using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;

[RequireComponent(typeof(EnemyController))]
public class DefMechanism : MonoBehaviour
{
    //방어기제 공통 시스템 관리 (어그로 게이지 등)
    public float aggroGauge = 0;
    private int maxAggroGauge = 4;
    private bool isAttacking = false;
    public bool attackEnabled = true;

    [SerializeField] private Collider2D startAttack;
    [SerializeField] private Image aggroGaugeBar;
    [SerializeField] private DefMechType defMechType;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float SightRange = 7f;


    private EnemyController ec;
    private Rigidbody2D rb;

    [SerializeField]
    private enum DefMechType {VernierAttack, ShoutingAttack, BadWordAttack, BeakerAttack}

    private void Awake()
    {
        ec = GetComponent<EnemyController>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        aggroGaugeBar.fillAmount = (float)aggroGauge / maxAggroGauge;
        if (!GetComponent<EnemyController>().isEA)
        {
            AttackSystem(); //공격 시스템
        }
    }


    private void AttackSystem()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, 15f, LayerMask.GetMask("Player"));
        if (aggroGauge > 0)
        {
            if (!isAttacking) //따라다니기
            {
                if (player != null)
                {
                    ec.lookingDirection = (player.transform.position - transform.position).normalized;
                    if (player.transform.position.x - transform.position.x > 1.2f)
                    {
                        rb.linearVelocityX = moveSpeed;
                    }
                    else if (player.transform.position.x - transform.position.x < -1.2f)
                    {
                        rb.linearVelocityX = -moveSpeed;
                    }
                    else
                    {
                        rb.linearVelocityX = 0;
                    }
                }
            }

            if (IsInSight()) //플레이어가 시야 내
            {
                aggroGauge = maxAggroGauge;
                if (Physics2D.OverlapBox(startAttack.bounds.center, startAttack.bounds.size, 0, LayerMask.GetMask("Player")) != null
                    && isAttacking == false && attackEnabled) //공격 범위 내
                {
                    isAttacking = true;
                    attackEnabled = false;
                    switch (defMechType)
                    {
                        case DefMechType.VernierAttack:
                            StartCoroutine(GetComponent<VernierAttack>().Attack(player));
                            break;

                        case DefMechType.ShoutingAttack:
                            StartCoroutine(GetComponent<ShoutingAttack>().Attack(player));
                            break;

                        case DefMechType.BadWordAttack:
                            StartCoroutine(GetComponent<BadWordAttack>().Attack(player));
                            break;

                        case DefMechType.BeakerAttack:
                            if (GetComponent<BeakerAttack>().attacklevel == 1)
                                StartCoroutine(GetComponent<BeakerAttack>().Attack1(player));
                            else if (GetComponent<BeakerAttack>().attacklevel == 2)
                                StartCoroutine(GetComponent<BeakerAttack>().Attack2(player));
                            break;
                    }
                }
            }
            else //플레이어가 시야 밖
            {
                aggroGauge -= Time.deltaTime;
                if (aggroGauge <= 0)
                {
                    rb.linearVelocityX = 0;
                    aggroGauge = 0;
                    ec.lookingDirection = new Vector2 (ec.SightXInt(), 0);
                }
            }
        }
        else if (aggroGauge <= 0)
        {
            if (IsInSight()
            || (player != null && Mathf.Abs(player.transform.position.x - transform.position.x) < 1.3f 
                && Mathf.Abs(player.transform.position.y - transform.position.y) < 0.5f))
            {
                StartFollowing();
            }
        }
    }

    public void StartFollowing()
    {
        aggroGauge = maxAggroGauge;
    }

    public void StopAttack(float cooltime)
    {
        isAttacking = false;
        StartCoroutine(AttackCool(cooltime));
    }
    private IEnumerator AttackCool(float cooltime)
    {
        yield return new WaitForSeconds(cooltime);
        attackEnabled = true;
    }

    private bool IsInSight()
    {
        RaycastHit2D player = Physics2D.Raycast(transform.position, ec.lookingDirection, SightRange, LayerMask.GetMask("Player", "Wall", "Ground"));
        Debug.DrawRay(transform.position, ec.lookingDirection * SightRange, Color.red);
        
        return player.collider != null && player.collider.gameObject.CompareTag("Player")
                && Mathf.Abs(player.transform.position.y - transform.position.y) < 6f;
    }
}
