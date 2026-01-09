using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class AcidParticle : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Collider2D collision = Physics2D.OverlapBox(transform.position, transform.localScale * GetComponent<CapsuleCollider2D>().size, 0f, LayerMask.GetMask("Player"));
        if (collision != null)
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(2);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        rb.linearVelocityX = 0f;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")
            || collision.gameObject.layer == LayerMask.NameToLayer("Platform")
            && Mathf.Abs(rb.linearVelocityY) < 0.1f)
        {
            StartCoroutine(Destroy());
        }
    }

    private IEnumerator Destroy()
    {
        anim.SetTrigger("Destroy");
        yield return new WaitForSeconds(5f);

        Destroy(gameObject);
    }
}
