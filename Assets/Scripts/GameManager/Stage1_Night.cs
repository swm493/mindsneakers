using UnityEngine;
using System.Collections;

public class Stage1_Night : MonoBehaviour
{
    public static Stage1_Night Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private bool target2_1 = false;
    private bool target2_2 = false;
    [SerializeField] private GameObject target2_2Object;
    [SerializeField] private GameObject doorObject;

    public void Interact(PlayerController player, Collider2D collider)
    {
        switch (collider.gameObject.name)
        {
            case "EasterEgg1":
                Debug.Log("이스터에그1");
                break;

            case "EasterEgg2":
                Debug.Log("이스터에그2");
                break;

            case "EasterEgg3":
                Debug.Log("이스터에그3");
                break;

            case "Target1":
                StartCoroutine(NextArea(player));
                break;

            case "Target2-1":
                target2_1 = true;
                if (target2_2)
                    OpenDoor();
                break;

            case "Target2-3":
                
                break;
            
            default:
                Debug.Log("등록되지 않은 상호작용");
                break;
        }
    }

    private void OpenDoor()
    {
        doorObject.SetActive(false);
    }

    private void Update()
    {
        if (target2_2Object.GetComponent<EnemyController>().stunned)
        {
            target2_2 = true;
            if (target2_1)
                OpenDoor();
        }
    }

    private IEnumerator NextArea(PlayerController player)
    {
        yield return new WaitForSeconds(2f);
        DontDestroyOnLoad(gameObject);
        GameManager.Instance.NextScene();
    }
}
