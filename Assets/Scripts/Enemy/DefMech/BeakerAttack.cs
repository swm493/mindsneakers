using UnityEngine;
using System.Collections;
using System;

public class BeakerAttack : MonoBehaviour
{
    [SerializeField] private GameObject acidPrefab;

    public float gravity = 3f;
    public float speed = 5f;

    public int attacklevel = 1;

    private Animator anim;
    private DefMechanism dm;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        dm = GetComponent<DefMechanism>();
    }

    public IEnumerator Attack1(Collider2D player)
    {
        anim.SetTrigger("Attack1");

        yield return new WaitForSeconds(0.5f);

        /* time contant version (g, v = 7, 2)
        acidParticle.GetComponent<Rigidbody2D>().linearVelocity = speed * new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y + gravity);
        */

        GameObject acidParticle = Instantiate(acidPrefab, transform.position, Quaternion.identity);
        acidParticle.GetComponent<Rigidbody2D>().linearVelocity = speed / 2 * 
                new Vector2(Mathf.Sqrt(2 - Mathf.Sqrt(4 - 4 * Mathf.Pow(gravity * (player.transform.position.x - transform.position.x) / Mathf.Pow(speed, 2), 2))) * (player.transform.position.x > transform.position.x ? 1 : -1), 
                            Mathf.Sqrt(2 + Mathf.Sqrt(4 - 4 * Mathf.Pow(gravity * (player.transform.position.x - transform.position.x) / Mathf.Pow(speed, 2), 2))));

        yield return new WaitForSeconds(1f);
        anim.SetTrigger("ChangeForm");
        attacklevel = 2;
        dm.StopAttack(3f);
    }

    public IEnumerator Attack2(Collider2D player)
    {
        anim.SetTrigger("Attack2");

        yield return new WaitForSeconds(0.5f);

        /* time contant version (g, v = 7, 2)
        beakerParticle.GetComponent<Rigidbody2D>().linearVelocity = speed * new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y + gravity);
        */

        GameObject beakerParticle = Instantiate(acidPrefab, transform.position, Quaternion.identity);
        beakerParticle.GetComponent<SpriteRenderer>().color = new Color(1f, 0.5f, 0f); //오렌지색
        beakerParticle.GetComponent<Rigidbody2D>().linearVelocity = speed / 2 * 
                new Vector2(Mathf.Sqrt(2 - Mathf.Sqrt(4 - 4 * Mathf.Pow(gravity * (player.transform.position.x - transform.position.x) / Mathf.Pow(speed, 2), 2))) * (player.transform.position.x > transform.position.x ? 1 : -1), 
                            Mathf.Sqrt(2 + Mathf.Sqrt(4 - 4 * Mathf.Pow(gravity * (player.transform.position.x - transform.position.x) / Mathf.Pow(speed, 2), 2))));

        yield return new WaitForSeconds(1f);
        anim.SetTrigger("NothingForm");
        attacklevel = 0;
        dm.StopAttack(0f);
    }
}
