using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class AcidParticle : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
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
