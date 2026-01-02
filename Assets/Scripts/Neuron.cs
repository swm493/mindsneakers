using System.Collections;
using UnityEngine;

/*
    뉴런 관리 방침
     본 스크립트는 뉴런의 머리에 부착되어야 한다.
     뉴런은 크게 타일맵, 머리, 코너, 꼬리로 구성된다.
     타일맵은 뉴런의 실질적인 외형을 나타내며, 이외의 기능은 없다.
     나머지 세 부분은 타일맵의 자식노드에 있으며 위치, 충돌 등의 기능을 구현하기 위해 필요하다.
     이들은 새로운 뉴런을 만들 때 마다 일일이 타일맵에 맞게 위치를 바꿔주어야 하며, 생김새에 따라 코너의 갯수도 조절되어야 한다.
     또한 꼬리와 모든 코너들은 머리 Inpector의 본 스크립트에 부착되어야 한다.
     꼬리는 NeuronTail에, 코너는 NeuronCorners에 "플레이어가 도달할 순서에 맞게" 각각 할당되어야 한다.
*/

public class Neuron : MonoBehaviour
{
    [SerializeField] private GameObject NeuronTail;
    [SerializeField] private GameObject[] NeuronCorners;

    private Vector3 direction;
    private float speed = 0.1f;

    
    public void SendSignal(PlayerMove player, GameObject spark)
    {
        if (NeuronTail == null)
        {
            Debug.LogError("뉴런 꼬리가 올바르게 할당되지 않았습니다.");
            return;
        }
        else
        {
            player.StopMovement();
            StartCoroutine(SendSignalAnim(player, spark));
        }
    }

    private IEnumerator SendSignalAnim(PlayerMove player, GameObject spark)
    {
        player.rb.gravityScale = 0f;
        player.bc.enabled = false;
        player.rb.linearVelocityY = 0f;

        //머리
        for (int i = 0; i < 80; i++)
        {
            player.transform.localScale -= 0.07f * Vector3.one; //단순히 작아지는 게 아니라 전기로 변하는 애니메이션이 있어도 괜찮을 듯
            if (Vector3.Distance(transform.position, player.transform.position) > 0.01f)
            {
                direction = (transform.position - player.transform.position).normalized;
                player.transform.position += direction * 0.015f;
            }
            yield return new WaitForSeconds(0.01f);
        }
        player.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        spark.SetActive(true);

        //코너링
        for (int i = 0; i < NeuronCorners.Length; i++)
        {
            while (Vector3.Distance(NeuronCorners[i].transform.position, player.transform.position) > 0.07f)
            {
                direction = (NeuronCorners[i].transform.position - player.transform.position).normalized;
                player.transform.position += direction * speed;
                yield return new WaitForSeconds(0.01f);
            }
        }

        //꼬리
        while (Vector3.Distance(NeuronTail.transform.position, player.transform.position) > 0.07f)
        {
            direction = (NeuronTail.transform.position - player.transform.position).normalized;
            player.transform.position += direction * speed;
            yield return new WaitForSeconds(0.01f);
        }
        spark.SetActive(false);
        player.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        player.EnableMovement();
        player.GetComponent<PlayerController>().onNeuron = false;
        for (int i = 0; i < 80; i++)
        {
            player.transform.localScale += 0.07f * Vector3.one;
            yield return new WaitForSeconds(0.003f);
        }
    }
}
