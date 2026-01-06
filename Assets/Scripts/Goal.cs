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

    public void GameClear(Collider2D collision, PlayerMove player)
    {
        if (collision.CompareTag("Player"))
        {
            player.DisableMovement();
            Vortex = player.transform.Find("Vortex");
            StartCoroutine(ClearAnim(player));
        }
    }

    private IEnumerator ClearAnim(PlayerMove player)
    {
        player.rb.gravityScale = 0f;
        player.bc.enabled = false;
        player.rb.linearVelocityY = 0.2f;
        for (int i = 0; i < 40; i++)
        {
            player.transform.localScale -= 0.1f * Vector3.one;
            Vortex.localScale += 0.01f * Vector3.one;
            yield return new WaitForSeconds(0.05f);
        }
        player.rb.linearVelocityY = 0f;

        while (Vector3.Distance(transform.position, player.transform.position) > 0.1f)
        {
            Vector3 direction = (transform.position - player.transform.position).normalized;
            currentSpeed += acceleration * Time.deltaTime;
            player.transform.position += direction * currentSpeed * Time.deltaTime;
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
