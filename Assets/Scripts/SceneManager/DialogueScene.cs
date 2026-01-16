using UnityEngine;
using UnityEngine.Events;

public class DialogueScene : MonoBehaviour
{
    public UnityEvent OnSceneStart;
    public UnityEvent OnDialogueEnd;

    private void Start()
    {
        OnSceneStart?.Invoke();
        int currentLevel = SaveManager.Instance.playerData.level;
        DialogueManager.Instance.PlayDialogue(currentLevel);
    }

    private void OnEnable()
    {
        DialogueManager.Instance.OnDialogueEnded += HandleDialogueEnd;
    }

    private void OnDisable()
    {
        DialogueManager.Instance.OnDialogueEnded -= HandleDialogueEnd;
    }

    public void StartDialouge()
    {
        SetInputMode(true);
    }

    private void HandleDialogueEnd()
    {
        SetInputMode(false);
        OnDialogueEnd?.Invoke();
    }

    private void SetInputMode(bool isDialogueMode)
    {
        var input = InputManager.Instance;

        if (isDialogueMode)
        {
            input.player.Disable();
            input.UI.Enable();
        }
        else
        {
            input.player.Enable();
            input.UI.Disable();
        }
    }
}