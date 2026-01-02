using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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
    private bool onGround = false;
    private bool onLadder = false;
    private bool isClimbing = false;
    private bool isUpping = false;
    private bool isDowning = false;
    private bool isDashing = false;
    private bool Dashenable = true;

    public Rigidbody2D rb;
    private SpriteRenderer sr;
    public BoxCollider2D bc;
    public Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        rb.gravityScale = GameManager.Instance.gravityScale;
    }

    private void FixedUpdate()
    {
        //바닥 체크
        if (Physics2D.OverlapBox(transform.position - new Vector3(0, bc.size.y * transform.localScale.y / 2, 0),
                    new Vector3(bc.size.x * transform.localScale.x * 0.95f, 0.05f, 0), 0, LayerMask.GetMask("Ground", "Platform")))
                    //원점 : 플레이어의 발, 크기 : 가로는 플레이어 크기의 0.9배, 세로는 0.1칸
        {
            onGround = true;
            if (Mathf.Abs(rb.linearVelocityY) < 0.1f) currentJumpCount = maxJumpCount;
        }
        else
        {
            onGround = false;
        }
        anim.SetBool("OnGround", onGround);

        //사다리 체크
        if (Physics2D.OverlapBox(transform.position - new Vector3(0, bc.size.y * transform.localScale.y * 0.1f, 0),
                    bc.size * transform.localScale * 0.75f, 0, LayerMask.GetMask("Ladder")))
                    //원점 : 플레이어의 0.1배 크기 아래, 크기 : 플레이어 크기의 0.75배
        {
            onLadder = true;
        }
        else
        {
            onLadder = false;
            if (isClimbing && moveEnabled) OffLadder();
        }

        //이동하기 & 애니메이션
        if (moveEnabled)
        {
            if (!isDashing) rb.linearVelocityX = moveVector.x * speed * (isClimbing ? 0.7f : 1f);
            PlayerAnimation();
        }

        //낙하 속도 제한
        if (rb.linearVelocityY < -20f)
            rb.linearVelocityY = -20f;
    }

    private void PlayerAnimation() //moveEnabled가 true일 때만 호출
    {
        if (moveVector.x > 0) //좌우반전
            sr.flipX = false;
        else if (moveVector.x < 0)
            sr.flipX = true;

        if (Mathf.Abs(moveVector.x) > 0.01f) //걷기 애니메이션
            anim.SetBool("IsMoving", true);
        else
            anim.SetBool("IsMoving", false);

        if (isClimbing) //사다리 애니메이션
        {
            if (isUpping && isDowning)
            {
                rb.linearVelocityY = 0f;
                anim.speed = 0f;
            }
            else if (isUpping)
            {
                rb.linearVelocityY = speed;
                anim.speed = 1f;
            }    
            else if (isDowning)
            {
                rb.linearVelocityY = -speed;
                anim.speed = 1f;
            }
            else
            {
                rb.linearVelocityY = 0f;
                anim.speed = 0f;
            }

            if (onGround && isDowning)
                OffLadder();
        }
        else //추락 애니메이션
        {
            if (rb.linearVelocityY < -0.1f && !onGround)
                anim.SetBool("IsFalling", true);
            else
                anim.SetBool("IsFalling", false);
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
        if (moveEnabled && context.performed)
        {
            if (isDowning) //플랫폼 아래로 점프
            {
                Collider2D platform = Physics2D.OverlapBox(transform.position - new Vector3(0, bc.size.y * transform.localScale.y, 0) / 2,
                            new Vector3(bc.size.x * transform.localScale.x, 0.1f, 0), 0, LayerMask.GetMask("Platform"));
                if (platform)
                    StartCoroutine(DropfromPlatform(platform));
            }
            else if (isClimbing) //사다리에서 내리기
            {
                OffLadder();
                if (!isDowning)
                {
                    rb.linearVelocityY = jumpForce;
                    anim.SetTrigger("Jump");
                }
            }
            else if (currentJumpCount >= 1) //일반 점프
            {
                rb.linearVelocityY = jumpForce;
                anim.SetTrigger("Jump");
                currentJumpCount--;
            }
        }
    }
    private IEnumerator DropfromPlatform(Collider2D platform) //플랫폼에서 떨어지기
    {
        float init_pos = transform.position.y;
        Physics2D.IgnoreCollision(bc, platform, true);
        while (true)
        {
            if (init_pos - transform.position.y > bc.size.y * transform.localScale.y / 2)
            {
                Physics2D.IgnoreCollision(bc, platform, false);
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
            if (onLadder && moveEnabled)
                ClimbLadder();
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
            if (onLadder && moveEnabled)
                ClimbLadder();
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
            if (Dashenable)
            {
                if (isClimbing)
                    OffLadder();
                StartCoroutine(Dash());
            }
            else
                Debug.Log("대시 쿨타임...");
        }
    }
    private IEnumerator Dash()
    {
        isDashing = true;
        StartCoroutine(DashTimer());
        rb.gravityScale = 0f;
        rb.linearVelocityY = 0f;
        if (sr.flipX)
            rb.linearVelocityX = -15f;
        else
            rb.linearVelocityX = 15f;
        yield return new WaitForSeconds(0.2f);
        rb.gravityScale = GameManager.Instance.gravityScale;
        isDashing = false;
    }
    private IEnumerator DashTimer()
    {
        Dashenable = false;
        yield return new WaitForSeconds(dashCool);
        Dashenable = true;
    }


    private void ClimbLadder() //사다리 타기
    {
        rb.gravityScale = 0f;
        rb.linearVelocityY = 0f;
        isClimbing = true;
        anim.SetBool("IsFalling", false);
        anim.SetBool("IsClimbing", true);
    }

    private void OffLadder() //사다리 내리기
    {
        rb.gravityScale = GameManager.Instance.gravityScale;
        anim.speed = 1f;
        isClimbing = false;
        anim.SetBool("IsClimbing", false);
    }

    public void StopMovement() //이동 정지
    {
        moveVector = Vector2.zero;
        rb.linearVelocityX = 0;
        moveEnabled = false;
    }

    public void EnableMovement() //이동 활성화
    {
        rb.gravityScale = GameManager.Instance.gravityScale;
        bc.enabled = true;
        moveEnabled = true;
    }
}
