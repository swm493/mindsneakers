using UnityEngine;

public class Stage1_Night : MonoBehaviour
{
    private int count = 0;

    [SerializeField] private GameObject object1;
    [SerializeField] private GameObject object2;
    [SerializeField] private GameObject door;

    private EnterTrigger et_object1;
    private EnterTrigger et_object2;

    private void Awake()
    {
        et_object1 = object1.GetComponent<EnterTrigger>();
        et_object2 = object2.GetComponent<EnterTrigger>();
    }

    private void Update()
    {
        if (et_object1.GetCollider() != null && et_object1.GetCollider().CompareTag("Player"))
        {
            count++;
            Debug.Log("목표1 도달");
            object1.SetActive(false); 
        }
        if (et_object2.GetCollider() != null && et_object2.GetCollider().CompareTag("Player"))
        {
            count++;
            Debug.Log("목표2 도달");
            object2.SetActive(false); 
        }
        if (count == 2)
        {
            door.SetActive(false);
        }
    }
}
