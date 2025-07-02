using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    private Vector2 moveVector;
    private bool onground;
    private bool moveEnabled = true;

    public bool OnGround
    {
        get { return onground; }
        private set { onground = value; anim.SetBool("OnGround", value); }
    }

    public Rigidbody2D rb;
    private SpriteRenderer sr;
    public CapsuleCollider2D cc;
    public Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        cc = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (Physics2D.BoxCast(transform.position, cc.size, 0, Vector2.down, 1f, LayerMask.GetMask("Ground")))
                OnGround = true;
            else
                OnGround = false;

        rb.linearVelocityX = moveVector.x * speed;
        PlayerAnimation();
    }

    private void PlayerAnimation()
    {
        if (moveVector.x > 0)
            sr.flipX = false;
        else if (moveVector.x < 0)
            sr.flipX = true;

        if (moveVector.x != 0)
            anim.SetBool("IsMoving", true);
        else
            anim.SetBool("IsMoving", false);

        if (rb.linearVelocityY < -0.1f && !OnGround)
            anim.SetBool("IsFalling", true);
        else
            anim.SetBool("IsFalling", false);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (moveEnabled)
            moveVector = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (moveEnabled && OnGround && context.performed)
        {
        rb.linearVelocityY = jumpForce;
        anim.SetTrigger("Jump");
        }
    }

    public void StopMovement()
    {
        moveVector = Vector2.zero;
        rb.linearVelocityX = 0;
        moveEnabled = false;
    }

    public void EnableMovement()
    {
        moveEnabled = true;
    }
}
