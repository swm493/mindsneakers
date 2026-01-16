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

        pm = player.GetComponent<PlayerMove>();
    }

    private bool target2_1 = false;
    private bool target2_2 = false;
    private int dialogue_count = 0;
    [SerializeField] private GameObject target2_2Object;
    [SerializeField] private GameObject doorObject;
    [SerializeField] private GameObject SkillUI;
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private GameObject player;

    private PlayerMove pm;

    private void Start()
    {
        Dialogue_Start();
    }

    private void Dialogue_Start()
    {
        SkillUI.SetActive(false);
        dialogueUI.SetActive(true);
        pm.DisableMovement();
        dialogue_count++;
    }
    public void Dialogue_End()
    {
        SkillUI.SetActive(true);
        dialogueUI.SetActive(false);
        pm.EnableMovement();

        if (dialogue_count == 2)
        {
            StartCoroutine(NextArea());
        }

        if (dialogue_count == 3)
        {
            Debug.Log("낮 전환");
        }
    }

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
                Dialogue_Start();
                break;

            case "Target2-1":
                target2_1 = true;
                if (target2_2)
                    OpenDoor();
                break;

            case "Target2-3":
                Dialogue_Start();
                break;
        }
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

    private void OpenDoor()
    {
        if (doorObject != null) doorObject.SetActive(false);
    }

    

    private IEnumerator NextArea()
    {
        yield return new WaitForSeconds(2f);
        DontDestroyOnLoad(gameObject);
        GameManager.Instance.NextScene();
    }
}
