using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private bool isClearing = false;
    public bool onNeuron = false;
    private bool FMenable = true;
    private bool onLMB = false;
    public int maxHealth = 10;
    public int currentHealth;
    private bool onDamageCooldown = false;

    [SerializeField] private GameObject healthUI;
    [SerializeField] private GameObject spark;
    [SerializeField] private GameObject feelMindPrefab;
    [SerializeField] private Transform goalTransform;

    private BoxCollider2D bc;
    private PlayerMove player;



    private void Start()
    {
        if (goalTransform == null)
        {
            goalTransform = GameObject.FindGameObjectWithTag("Goal").transform;
        }
    }

    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        player = GetComponent<PlayerMove>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        ElectricalAnesthesia();
    }


    //전기 마취 (LMB)
    public void OnLMB(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onLMB = true;
        }
        else if (context.canceled)
        {
            onLMB = false;
        }
    }
    private void ElectricalAnesthesia()
    {
        //마우스 주변 감지
        Collider2D[] enemies = Physics2D.OverlapCircleAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.5f, LayerMask.GetMask("Enemy"));

        foreach (Collider2D enemy in enemies)
        {
            EnemyController ec = enemy.GetComponent<EnemyController>();
            Vector2 enemyHeadPos = enemy.transform.position + new Vector3(0, enemy.GetComponent<BoxCollider2D>().size.y * enemy.transform.localScale.y / 4f, 0);

            Vector2 headPos = transform.position + new Vector3(0, bc.size.y * transform.localScale.y / 4f, 0);
            RaycastHit2D hit = Physics2D.Raycast(headPos, enemyHeadPos - headPos, Vector2.Distance(headPos, enemyHeadPos),
                LayerMask.GetMask("Enemy", "Wall", "Ground"));

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy") //적과 플레이어 사이가 막히진 않았는가
                    && Vector2.Dot(ec.lookingDirection, headPos - enemyHeadPos) < 0 // 적의 뒤에 플레이어가 있는가
                    && Vector2.Distance(headPos, enemyHeadPos) <= 4.5f //적과의 거리
                    && Mathf.Abs(headPos.y - enemyHeadPos.y) <= 2f //적과의 높이 차이
                    && ec.isEA == false) //적이 전기마취 당하지 않았는가
            {
                ec.isTargeted = true;
                GameManager.Instance.CursorInactive();
                if (onLMB)
                {
                    ec.ElectricalAnesthesia();
                }
            }
            else
            {
                ec.isTargeted = false;
                GameManager.Instance.CursorActive();
            }
        }
    }


    //들어가기 (W)
    public void OnEnter(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, bc.size * transform.localScale, 0);
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Goal") && !isClearing) //마음의 핵
                {
                    collider.GetComponent<Goal>().GameClear(bc, player);
                    isClearing = true;
                }
                else if (collider.gameObject.layer == LayerMask.NameToLayer("NeuronHead") && !onNeuron) //뉴런
                {
                    collider.GetComponent<Neuron>().SendSignal(player, spark);
                    onNeuron = true;
                }
            }
        }
    }


    //정신 감정 (출구 방향) (TAB)
    public void OnFeelMind(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (goalTransform != null)
            {
                if (FMenable)
                {
                    StartCoroutine(FeelMind());
                    StartCoroutine(FMTimer());
                }
                else
                {
                    Debug.Log("정신 감정 쿨타임...");
                }
            }
            else
            {
                Debug.LogWarning("플레이어 컨트롤러에 출구를 설정해주세요.");
            }
        }
    }


    //코르티솔 (적 자취 드러내기) (F)
    public void OnCortisol(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("코르티솔");
        }
    }


    //스테이지 특수 스킬 (E)
    public void OnSpecialSkill(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switch (GameManager.Instance.stageNumber)
            {
                case 1:
                    ChocolateBomb();
                    break;
                default:
                    Debug.LogWarning("스테이지 넘버에 맞는 특수 스킬이 없음");
                    break;
            }
        }
    }
    private void ChocolateBomb()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 4f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                collider.GetComponent<EnemyController>().EatingChocolate();
            }
        }
    }
    
    public void TakeDamage(int damage)
    {
        if (!onDamageCooldown)
        {
            currentHealth -= damage;
            healthUI.GetComponent<HealthUI>().UpdateHealth();
            StartCoroutine(DamageCooldown());
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Debug.Log("플레이어 사망");
                //사망 처리
            }
        }
    }

    private IEnumerator DamageCooldown()
    {
        onDamageCooldown = true;
        yield return new WaitForSeconds(0.5f);
        onDamageCooldown = false;
    }

    private IEnumerator FeelMind()
    {
        float passedTime = 0f;
        Vector3 goalDirection = (goalTransform.position - transform.position).normalized;
        Vector3 feelMindPosition = transform.position;

        while (passedTime < 0.05f && Vector3.Distance(feelMindPosition, goalTransform.position) > 1.4f)
        {
            passedTime += Time.deltaTime;
            feelMindPosition += goalDirection * 1.5f;
            StartCoroutine(FeelMindParticle(feelMindPosition, goalDirection));
            yield return new WaitForSeconds(0.15f);
        }
    }
    private IEnumerator FeelMindParticle(Vector3 feelMindPosition, Vector3 goalDirection)
    {
        GameObject feelMindParticle = Instantiate(feelMindPrefab, feelMindPosition, Quaternion.identity);
        feelMindParticle.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(goalDirection.y, goalDirection.x) * Mathf.Rad2Deg);
        SpriteRenderer FM_sr = feelMindParticle.GetComponent<SpriteRenderer>();

        for (float i = 0; i < 1f; i += Time.deltaTime * 2f)
        {
            FM_sr.color = new Color(1f, 1f, 1f, Mathf.Lerp(0f, 1f, i));
            yield return null;
        }
        for (int i = 0; i < 5; i++)
        {
            for (float j = 0; j < 1f; j += Time.deltaTime * 2f)
            {
                FM_sr.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0.6f, j));
                yield return null;
            }
            for (float j = 0; j < 1f; j += Time.deltaTime * 2f)
            {
                FM_sr.color = new Color(1f, 1f, 1f, Mathf.Lerp(0.6f, 1f, j));
                yield return null;
            }
        }
        for (float i = 0; i < 1f; i += Time.deltaTime * 2f)
        {
            FM_sr.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, i));
            yield return null;
        }
        Destroy(feelMindParticle);
    }
    private IEnumerator FMTimer()
    {
        FMenable = false;
        yield return new WaitForSeconds(1f);
        FMenable = true;
    }
}
