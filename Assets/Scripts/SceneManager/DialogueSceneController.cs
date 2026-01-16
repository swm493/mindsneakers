using UnityEngine;
using UnityEngine.Events;

public class DialogueSceneController : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent onDialogueSequenceStarted;
    [SerializeField] private UnityEvent onDialogueSequenceEnded;

    private void Start()
    {
        OnDialogueSequenceStarted();
    }

    public void OnDialogueSequenceStarted()
    {
        onDialogueSequenceStarted?.Invoke();
    }

    public void OnDialogueSequenceEnded()
    {
        onDialogueSequenceEnded?.Invoke();
    }

    public void CheckDialogue()
    {
        int currentLevel = SaveManager.Instance.playerData.level;
        switch (currentLevel)
        {
            case 3:
                SceneTransitionManager.Instance.LoadScene("CutScene");
                break;
            case 4:
                return;
            case 5:
                return;
            case 6:
                return;
            default:
                MyDebug.Log("No specific action for this level. Restarting dialogue sequence.");
                SceneTransitionManager.Instance.LoadScene("DialogueScene");
                break;
        }
    }

    public void SetInputActive(bool active)
    {
        if (active)
            InputManager.Instance.EnableInput();
        else
            InputManager.Instance.DisableInput();
    }
}