using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //적이 공통으로 가질 속성들을 관리 (피격 판정 등)
    public Vector2 lookingDirection = Vector2.left;

    public bool isTargeted = false; // 전기마취 타겟팅 여부
    public bool isEA = false; // 전기마취 당했는지 여부

    public Animator anim;
    private Rigidbody2D rb;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb.gravityScale = GameManager.Instance.gravityScale;
    }

    public int SightXInt()
    {
        return lookingDirection.x > 0 ? 1 : -1;
    }

    private void Update()
    {
        if (isTargeted)
        {
            if (Physics2D.OverlapCircleAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.5f, LayerMask.GetMask("Enemy")).Length == 0)
            {
                isTargeted = false;
                GameManager.Instance.CursorActive();
            }
            else
            {
                anim.SetBool("OnTargeted", true);
            }
        }
        else
        {
            anim.SetBool("OnTargeted", false);
        }
        if (lookingDirection.x > 0)
            anim.SetBool("FacingRight", true);
        else if (lookingDirection.x < 0)
            anim.SetBool("FacingRight", false);

        if (Mathf.Abs(rb.linearVelocityX) > 0.01f) //움직임 애니메이션
            anim.SetBool("IsMoving", true);
        else
            anim.SetBool("IsMoving", false);
    }

    //전기마취
    public void GetShocked()
    {
        if (isEA == false)
        {
            isEA = true;
            isTargeted = false;
            rb.simulated = false;
            Debug.Log(gameObject.name + "(이)가 전기마취 당함!");
            anim.SetTrigger("ElectricalAnesthesia");
        }
    }

    //스페셜 스킬1
    public void EatingChocolate()
    {
        Debug.Log("냠냠");
        anim.SetTrigger("EatingChocolate");
    }
}
