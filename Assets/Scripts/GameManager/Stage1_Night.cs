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

    public bool target2_1 = false;
    public bool target2_2 = false;
    [SerializeField] private GameObject target2_2Object;
    [SerializeField] private GameObject doorObject;
    [SerializeField] private GameObject skillUI;
    [SerializeField] private GameObject easterEgg1;
    [SerializeField] private GameObject easterEgg2;
    [SerializeField] private GameObject easterEgg3;

    public void Interact(PlayerController player, Collider2D collider)
    {
        switch (collider.gameObject.name)
        {
            case "EasterEgg1":
                OpenEasterEgg1(player);
                break;

            case "EasterEgg2":
                OpenEasterEgg2(player);
                break;

            case "EasterEgg3":
                OpenEasterEgg3(player);
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
        }
    }

    private void OpenEasterEgg1(PlayerController player)
    {
        skillUI.SetActive(false);
        easterEgg1.SetActive(true);
    }

    private void OpenEasterEgg2(PlayerController player)
    {
        skillUI.SetActive(false);
        easterEgg2.SetActive(true);
    }

    private void OpenEasterEgg3(PlayerController player)
    {
        skillUI.SetActive(false);
        easterEgg3.SetActive(true);
    }

    private void OpenDoor()
    {
        if (doorObject != null) doorObject.SetActive(false);
    }

    public void OpenSkillUI()
    {
        skillUI.SetActive(true);
        easterEgg1.SetActive(false);
        easterEgg2.SetActive(false);
        easterEgg3.SetActive(false);
    }

    private void Update()
    {
        if (target2_2Object != null && target2_2Object.GetComponent<EnemyController>().stunned)
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
