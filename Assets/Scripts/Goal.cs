using System.Collections;
using UnityEngine;

public class Goal : MonoBehaviour
{
    float currentSpeed = -2f;
    private float acceleration = 4f;

    private Transform Circle;
    private Transform Vortex;
    private void Awake()
    {
        Circle = transform.Find("Circle");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMove player = collision.GetComponentInParent<PlayerMove>();
            player.StopMovement();

            Vortex = player.transform.Find("Vortex");
            StartCoroutine(FinishGame(player));
        }
    }

    private IEnumerator FinishGame(PlayerMove player)
    {
        //매우 비효율적인 애니메이션
        player.rb.gravityScale = 0f;
        player.cc.enabled = false;
        player.rb.linearVelocityY = 0.2f;
        for (int i = 0; i < 40; i++)
        {
            player.transform.localScale -= 0.1f * Vector3.one;
            Vortex.localScale += 0.01f * Vector3.one;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.2f);
        player.rb.linearVelocityY = 0f;

        while (Vector3.Distance(transform.position, player.transform.position) > 0.3f)
        {
            Vector3 direction = (transform.position - player.transform.position).normalized;
            currentSpeed += acceleration * Time.deltaTime;
            player.transform.position += direction * currentSpeed * Time.deltaTime; ;
            yield return new WaitForSeconds(0.001f);
        }

        for (int i = 0; i < 20; i++)
        {
            Circle.localScale += new Vector3(0.05f, 0.05f, 0);
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 20; i++)
        {
            Circle.localScale -= new Vector3(0.05f, 0.05f, 0);
            yield return new WaitForSeconds(0.01f);
        }

        Debug.Log("Cleared!");
    }
}
