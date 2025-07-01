using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    private Vector2 moveVector;
    [SerializeField] private bool onground;

    public bool OnGround
    {
        get { return onground; }
        private set { onground = value; anim.SetBool("OnGround", value); }
    }

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private CapsuleCollider2D cc;
    private Animator anim;

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
        moveVector = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (OnGround && context.performed)
            rb.linearVelocityY = jumpForce;
            anim.SetTrigger("Jump");
    }
}
