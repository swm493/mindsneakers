using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VisualScripting;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private int maxJumpCount = 2;
    [SerializeField] private float dashCool = 1f;
    private Vector2 moveVector;

    // 상태 변수
    private bool moveEnabled = true;
    public int currentJumpCount = 0;
    [SerializeField]private bool onGround = false;
    public bool onBush = false;
    private bool isUpping = false;
    private bool isDowning = false;
    public bool isDashing = false;
    private bool isFalling = false;
    private bool onDashCooldown = false;

    public Rigidbody2D rb;
    public CapsuleCollider2D cc;
    public Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        rb.gravityScale = GameManager.Instance.gravityScale;
    }

    private void FixedUpdate()
    {
        //바닥 체크
        if (Physics2D.OverlapBox(transform.position - new Vector3(0, cc.size.y * transform.localScale.y / 2, 0),
                    new Vector3(0.95f, 0.1f, 0), 0, LayerMask.GetMask("Ground", "Platform")))
                    //원점 : 플레이어의 발, 크기 : 가로는 0.95칸, 세로는 0.1칸
        {
            onGround = true;
            isFalling = false;
            if (Mathf.Abs(rb.linearVelocityY) < 0.11f) currentJumpCount = maxJumpCount;
        }
        else
        {
            onGround = false;
            if (currentJumpCount == maxJumpCount)
                currentJumpCount--;
        }
        anim.SetBool("OnGround", onGround);

        //엄폐물 체크
        onBush = Physics2D.OverlapBox(transform.position, new Vector3(0.9f, 1.8f, 1), 0, LayerMask.GetMask("Bush")) != null;

        //이동하기 & 애니메이션
        if (moveEnabled)
        {
            if (!isDashing) 
            {
                if (onGround)
                    rb.linearVelocityX = moveVector.x * speed;
                else if (!onGround)
                    rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, moveVector.x * speed, 20f * Time.fixedDeltaTime);
            }
            PlayerAnimation();
        }

        //낙하 속도 제한
        if (rb.linearVelocityY < -20f)
            rb.linearVelocityY = -20f;
    }

    private void PlayerAnimation() //moveEnabled가 true일 때만 호출
    {
        if (moveVector.x > 0) //좌우반전
            transform.localScale = new Vector3(6, 6, 6);
        else if (moveVector.x < 0)
            transform.localScale = new Vector3(-6, 6, 6);

        if (Mathf.Abs(moveVector.x) > 0.01f) //걷기 애니메이션
            anim.SetBool("IsMoving", true);
        else
            anim.SetBool("IsMoving", false);
        
        if (rb.linearVelocityY < -0.1f && !onGround && !isFalling)
        {
            anim.SetTrigger("Fall");
            isFalling = true;
        }
    }


    //좌우 이동 (A, D)
    public void OnMove(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
    }


    //점프 (Space)
    public void OnJump(InputAction.CallbackContext context)
    {
        if (moveEnabled && context.performed && !isDashing)
        {
            if (isDowning) //플랫폼 아래로 점프
            {
                Collider2D[] platforms = Physics2D.OverlapBoxAll(transform.position - new Vector3(0, cc.size.y * transform.localScale.y / 2, 0),
                            new Vector3(cc.size.x * Mathf.Abs(transform.localScale.x), 0.1f, 0), 0, LayerMask.GetMask("Platform"));
                if (platforms.Length != 0)
                    StartCoroutine(DropfromPlatform(platforms));
            }
            else if (currentJumpCount >= 1) //일반 점프
            {
                isFalling = false;
                rb.linearVelocityY = jumpForce;
                anim.SetTrigger("Jump");
                currentJumpCount--;
            }
        }
    }
    private IEnumerator DropfromPlatform(Collider2D[] platforms) //플랫폼에서 떨어지기
    {
        float init_pos = transform.position.y;
        foreach (Collider2D platform in platforms) Physics2D.IgnoreCollision(cc, platform, true);
        while (true)
        {
            if (init_pos - transform.position.y > cc.size.y * transform.localScale.y / 2)
            {
                foreach (Collider2D platform in platforms) Physics2D.IgnoreCollision(cc, platform, false);
                break;
            }
            yield return null;
        }
    }


    //위 이동 (W)
    public void OnUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isUpping = true;
        }
        else if (context.canceled)
        {
            isUpping = false;
        }
    }


    //아래 이동 (S)
    public void OnDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isDowning = true;
        }
        else if (context.canceled)
        {
            isDowning = false;
        }
    }


    //대시 (Shift)
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && moveEnabled)
        {
            if (!onDashCooldown && moveEnabled)
            {
                StartCoroutine(Dash());
            }
            else
                Debug.Log("대시 쿨타임...");
        }
    }
    private IEnumerator Dash()
    {
        isDashing = true;
        anim.SetTrigger("Dash");
        StartCoroutine(DashTimer());
        rb.gravityScale = 0f;
        rb.linearVelocityY = 0f;
        transform.position += new Vector3(0, 0.1f, 0); //충돌 문제 방지용
        if (transform.localScale.x > 0)
            rb.linearVelocityX = 15f;
        else if (transform.localScale.x < 0)
            rb.linearVelocityX = -15f;
        yield return new WaitForSeconds(0.2f);
        rb.gravityScale = GameManager.Instance.gravityScale;
        if (rb.linearVelocityX > 13f)
            rb.linearVelocityX = 13f;
        else if (rb.linearVelocityX < -13f)
            rb.linearVelocityX = -13f;
        isDashing = false;
    }
    private IEnumerator DashTimer()
    {
        onDashCooldown = true;
        yield return new WaitForSeconds(dashCool);
        onDashCooldown = false;
    }


    public void DisableMovement() //이동 정지
    {
        rb.linearVelocityX = 0;
        moveEnabled = false;
    }

    public void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;
        moveEnabled = false;
    }

    public void EnableMovement() //이동 활성화
    {
        rb.gravityScale = GameManager.Instance.gravityScale;
        cc.enabled = true;
        moveEnabled = true;
    }
}
