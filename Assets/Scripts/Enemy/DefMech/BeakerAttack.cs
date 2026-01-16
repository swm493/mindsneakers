using UnityEngine;
using System.Collections;
using System;

public class BeakerAttack : MonoBehaviour
{
    [SerializeField] private GameObject acid;
    [SerializeField] private GameObject acidParticle;
    [SerializeField] private GameObject beakerParticle;

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
        yield return new WaitForSeconds(0.5f); //선딜

        anim.SetTrigger("Attack1");

        yield return new WaitForSeconds(0.1f);
        Vector3 spawnPos = transform.position + new Vector3(1f, 0f);
        GameObject acidInstance = Instantiate(acid, spawnPos, Quaternion.identity);
        
        for (int i = 0; i < 3; i++)
        {
            GameObject particle = Instantiate(acidParticle, spawnPos, Quaternion.identity);
            particle.GetComponent<Rigidbody2D>().linearVelocity = speed / 2 * 
                    new Vector2(Mathf.Sqrt(2 - Mathf.Sqrt(4 - 4 * Mathf.Pow((gravity+10*i) * (player.transform.position.x - spawnPos.x) / Mathf.Pow(speed, 2), 2))) * (player.transform.position.x > transform.position.x ? 1 : -1), 
                                Mathf.Sqrt(2 + Mathf.Sqrt(4 - 4 * Mathf.Pow((gravity+10*i) * (player.transform.position.x - spawnPos.x) / Mathf.Pow(speed, 2), 2))));
        }

        yield return new WaitForSeconds(0.3f);
        Destroy(acidInstance);
        yield return new WaitForSeconds(0.7f); //후딜
        anim.SetTrigger("ChangeForm");
        attacklevel = 2;
        dm.StopAttack(3f);
    }

    public IEnumerator Attack2(Collider2D player)
    {
        anim.SetTrigger("Attack2");

        yield return new WaitForSeconds(0.5f); //선딜

        GameObject particle = Instantiate(beakerParticle, transform.position, Quaternion.identity);
        particle.GetComponent<Rigidbody2D>().linearVelocity = speed / 2 * 
                new Vector2(Mathf.Sqrt(2 + Mathf.Sqrt(4 - 4 * Mathf.Pow((gravity+20) * (player.transform.position.x - transform.position.x) / Mathf.Pow(speed, 2), 2))) * (player.transform.position.x > transform.position.x ? 1 : -1), 
                            Mathf.Sqrt(2 - Mathf.Sqrt(4 - 4 * Mathf.Pow((gravity+20) * (player.transform.position.x - transform.position.x) / Mathf.Pow(speed, 2), 2))));

        yield return new WaitForSeconds(1f); //후딜
        attacklevel = 0;
        dm.StopAttack(0f);
    }
}
