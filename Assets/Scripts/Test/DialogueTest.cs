using UnityEngine;
using UnityEngine.Events;

public class DialogueTest : MonoBehaviour
{
    [SerializeField] private UnityEvent onTestEvent;

    private void Start()
    {
        onTestEvent?.Invoke();
    }
}
