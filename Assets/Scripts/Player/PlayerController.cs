using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private bool isClearing = false;
    public bool onNeuron = false;
    public bool isInteractioning = false;
    private int maxEgo = 10;
    private int currentEgo;
    private int maxHRM = 5;
    private int currentHRM;
    private bool onDamageCool = false;
    private bool onSpecialSkillCool = false;
    private bool onEACool = false;
    private bool onCortisolCool = false;
    private bool onFMCool = false;
    private Collider2D target = null;

    private Image egoUI;
    private Image hormoneUI;
    private Image specialSkillUI;
    private Image electroAnestheisaUI;
    private Image cortisolUI;
    private Image feelMindUI;
    [SerializeField] private GameObject spark;
    [SerializeField] private GameObject feelMindPrefab;
    [SerializeField] private GameObject electricEffect;
    [SerializeField] private GameObject chocolateBombPrefab;
    [SerializeField] private Transform goalTransform;
    [SerializeField] private AudioClip specialSkillSfx;
    [SerializeField] private AudioClip electricalSfx;

    private BoxCollider2D bc;
    private PlayerMove player;
    private Rigidbody2D rb;
    private Animator anim;


    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        player = GetComponent<PlayerMove>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        if (goalTransform == null)
        {
            goalTransform = GameObject.FindGameObjectWithTag("Goal").transform;
        }
        currentEgo = maxEgo;
        currentHRM = maxHRM;

        egoUI = UIManager.Instance.egoUI;
        hormoneUI = UIManager.Instance.hormoneUI;
        specialSkillUI = UIManager.Instance.specialSkillUI;
        electroAnestheisaUI = UIManager.Instance.electroAnestheisaUI;
        cortisolUI = UIManager.Instance.cortisolUI;
        feelMindUI = UIManager.Instance.feelMindUI;

        egoUI.fillAmount = (float) currentEgo / maxEgo;
        hormoneUI.fillAmount = (float) currentHRM / maxHRM;
        specialSkillUI.fillAmount = 0f;
        electroAnestheisaUI.fillAmount = 0f;
        cortisolUI.fillAmount = 0f;
        feelMindUI.fillAmount = 0f;
    }
    
    private void Update()
    {
        ElectricalAnesthesia();
    }

    public void TakeDamage(int damage)
    {
        if (!onDamageCool)
        {
            currentEgo -= damage;
            egoUI.fillAmount = (float) currentEgo / maxEgo;
            StartCoroutine(DamageCooldown());
            if (currentEgo <= 0)
            {
                currentEgo = 0;
                StartCoroutine(Death());
            }
        }
    }
    private IEnumerator DamageCooldown()
    {
        onDamageCool = true;
        yield return new WaitForSeconds(0.5f);
        onDamageCool = false;
    }
    private IEnumerator Death()
    {
        player.DisableMovement();
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(2f);
        GameManager.Instance.RestartScene();
    }


    //전기 마취 (LMB)
    public void OnLMB(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (target != null && !onEACool)
            {
                target.GetComponent<EnemyController>().GetShocked();

                StartCoroutine(EAAnimation(target));
                StartCoroutine(EATimer());
                AudioManager.Instance.Play(electricalSfx, 0.5f);
            }
            else if (isInteractioning)
            {
                EndInteraction();
            }
        }
    }
    private void ElectricalAnesthesia() //전기 마취 타겟 감지
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(GameManager.Instance.GetMousePos(), 0.5f, LayerMask.GetMask("Enemy"));
        EnemyController ec;

        float max_distance = 6f;
        Vector2 enemyHeadPos;
        Vector2 playerHeadPos = transform.position + new Vector3(0, bc.size.y * transform.localScale.y / 4f, 0);
        Collider2D closest_enemy = null;
        foreach (Collider2D enemy in enemies)
        {
            ec = enemy.GetComponent<EnemyController>();
            if (ec.stunned) continue;

            enemyHeadPos = ec.headPos.position;
            float distance = Vector2.Distance(GameManager.Instance.GetMousePos(), enemyHeadPos);
            if (max_distance > distance
                && Vector2.Dot(ec.lookingDirection, playerHeadPos - enemyHeadPos) < 0)
            {
                max_distance = distance;
                closest_enemy = enemy;
            }
        }
        foreach (Collider2D enemy in enemies)
        {
            if (enemy != closest_enemy)
                enemy.GetComponent<EnemyController>().isTargeted = false;
        }

        if (closest_enemy == null)
        {
            target = null;
            GameManager.Instance.CursorActive();
            return;
        }
        
        ec = closest_enemy.GetComponent<EnemyController>();
        enemyHeadPos = ec.headPos.position;

        RaycastHit2D hit = Physics2D.Raycast(playerHeadPos, enemyHeadPos - playerHeadPos, Vector2.Distance(playerHeadPos, enemyHeadPos),
            LayerMask.GetMask("Enemy", "Wall", "Ground"));

        if (hit.collider == closest_enemy)
        {
            ec.isTargeted = true;
            GameManager.Instance.CursorInactive();
            target = closest_enemy;
        }
        else
        {
            ec.isTargeted = false;
            GameManager.Instance.CursorActive();
            target = null;
        }
    }
    public IEnumerator EAAnimation(Collider2D target) //전기마취 애니메이션
    {
        Vector2 init_velocity = rb.linearVelocity;
        player.StopMovement();

        bool sameDirection = (GameManager.Instance.GetMousePos().x - transform.position.x) * transform.localScale.x > 0 ? true : false;
        anim.SetBool("SameDirection", sameDirection);
        anim.SetTrigger("EA");

        yield return new WaitForSeconds(0.2f);
        Vector3 handPos = transform.position + new Vector3(0.5f * (transform.localScale.x > 0 ? 1 : -1), 0.2f);
        Vector3 direction = (target.GetComponent<EnemyController>().headPos.position - handPos).normalized;
        GameObject particle = Instantiate(electricEffect, handPos, Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
        
        float startTime = Time.time;
        while (Time.time - startTime < 0.1f)
        {
            particle.transform.localScale += new Vector3(Time.deltaTime * 4f, 0);
            yield return null;
        }
        Destroy(particle);
        yield return new WaitForSeconds(0.2f);

        rb.linearVelocity = init_velocity;
        player.EnableMovement();
    }
    private IEnumerator EATimer() //전기마취 쿨타임
    {
        onEACool = true;
        for (int i = 0; i < 100 ; i  ++)
        {
            electroAnestheisaUI.fillAmount = 1 - (float) i / 99;
            yield return new WaitForSeconds(0.02f);
        }
        onEACool = false;
    }


    //들어가기 (W)
    public void OnEnter(InputAction.CallbackContext context)
    {
        if (context.performed && !player.isDashing)
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, 0.95f * new Vector3(1, 2, 1), 0);
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
                else if (collider.gameObject.layer == LayerMask.NameToLayer("Interaction") && !isInteractioning) //상호작용 오브젝트
                {
                    Stage1_Night.Instance.Interact(this, collider);
                    player.DisableMovement();
                    anim.SetTrigger("Interaction");
                    isInteractioning = true;
                }
            }
        }
    }
    public void EndInteraction()
    {
        anim.SetTrigger("OffInteraction");
        player.EnableMovement();
        isInteractioning = false;
        Stage1_Night.Instance.OpenSkillUI();
    }


    //정신 감정 (출구 방향) (TAB)
    public void OnFeelMind(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (goalTransform == null)
            {
                Debug.LogWarning("플레이어 컨트롤러에 출구를 설정해주세요.");
            }
            else if (!onFMCool)
            {
                StartCoroutine(FeelMind());
                StartCoroutine(FMTimer());
            }
        }
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
        GameObject feelMindParticle = Instantiate(feelMindPrefab, feelMindPosition,
            Quaternion.Euler(0, 0, Mathf.Atan2(goalDirection.y, goalDirection.x) * Mathf.Rad2Deg));
        SpriteRenderer FM_sr = feelMindParticle.GetComponent<SpriteRenderer>();

        for (float i = 0; i < 1f; i += Time.deltaTime * 2f)
        {
            FM_sr.color = new Color(0, 1f, 0.95f, Mathf.Lerp(0f, 1f, i));
            yield return null;
        }
        for (int i = 0; i < 5; i++)
        {
            for (float j = 0; j < 1f; j += Time.deltaTime * 2f)
            {
                FM_sr.color = new Color(0, 1f, 0.95f, Mathf.Lerp(1f, 0.6f, j));
                yield return null;
            }
            for (float j = 0; j < 1f; j += Time.deltaTime * 2f)
            {
                FM_sr.color = new Color(0, 1f, 0.95f, Mathf.Lerp(0.6f, 1f, j));
                yield return null;
            }
        }
        for (float i = 0; i < 1f; i += Time.deltaTime * 2f)
        {
            FM_sr.color = new Color(0, 1f, 0.95f, Mathf.Lerp(1f, 0f, i));
            yield return null;
        }
        Destroy(feelMindParticle);
    }
    private IEnumerator FMTimer()
    {
        onFMCool = true;
        for (int i = 0; i < 100 ; i  ++)
        {
            feelMindUI.fillAmount = 1 - (float) i / 99;
            yield return new WaitForSeconds(0.05f);
        }
        onFMCool = false;
    }


    //코르티솔 (적 자취 드러내기) (F)
    public void OnCortisol(InputAction.CallbackContext context)
    {
        if (context.performed && !onCortisolCool)
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 13f, LayerMask.GetMask("Enemy"));
            foreach (Collider2D enemy in enemies)
            {
                if(enemy.GetComponent<DefMechanism>() != null) StartCoroutine(enemy.GetComponent<DefMechanism>().ShowCortisol());
            }
            StartCoroutine(CortisolTimer());
        }
    }
    private IEnumerator CortisolTimer()
    {
        onCortisolCool = true;
        for (int i = 0; i < 100 ; i  ++)
        {
            cortisolUI.fillAmount = 1 - (float) i / 99;
            yield return new WaitForSeconds(0.05f);
        }
        onCortisolCool = false;
    }


    //스테이지 특수 스킬 (E)
    public void OnSpecialSkill(InputAction.CallbackContext context)
    {
        if (context.performed && !onSpecialSkillCool)
        {
            if (currentHRM > 0)
            {
                currentHRM--;
                AudioManager.Instance.Play(specialSkillSfx, 0.5f);
                switch (GameManager.Instance.stageNumber)
                {
                    case 1:
                        StartCoroutine(ChocolateBomb());
                        anim.SetTrigger("SpecialSkill");
                        StartCoroutine(SpecialSkillTimer(5f));
                        break;
                    default:
                        Debug.LogWarning("스테이지 넘버에 맞는 특수 스킬이 없음");
                        currentHRM++;
                        break;
                }
                hormoneUI.fillAmount = (float) currentHRM / maxHRM;
            }
            else
            {
                Debug.Log("호르몬 부족");
            }
        }
    }
    private IEnumerator SpecialSkillTimer(float time)
    {
        onSpecialSkillCool = true;
        for (int i = 0; i < 100 ; i  ++)
        {
            specialSkillUI.fillAmount = 1 - (float) i / 99;
            yield return new WaitForSeconds(time / 100f);
        }
        onSpecialSkillCool = false;
    }

    private IEnumerator ChocolateBomb()
    {
        StartCoroutine(CBAnim());

        yield return new WaitForSeconds(0.5f);
        GameObject chocBomb = Instantiate(chocolateBombPrefab, transform.position, Quaternion.identity);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 4f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                collider.GetComponent<EnemyController>().EatingChocolate();
            }
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(chocBomb);
    }
    private IEnumerator CBAnim()
    {
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        player.DisableMovement();
        yield return new WaitForSeconds(1f);
        rb.gravityScale = GameManager.Instance.gravityScale;
        player.EnableMovement();
    }
}
