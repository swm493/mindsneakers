using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueSceneController : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent onDialogueSequenceStarted;
    [SerializeField] private UnityEvent onDialogueSequenceEnded;

    [Header("UI")]
    [SerializeField] private Image background;

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

    public void SetBackground()
    {
        switch (SaveManager.Instance.playerData.level)
        {
            case 0:
                background.sprite = Resources.Load<Sprite>("Backgrounds/LivingRoom");
                break;
            case 1:
                background.sprite = Resources.Load<Sprite>("Backgrounds/Kitchen");
                break;
            case 2:
                background.sprite = Resources.Load<Sprite>("Backgrounds/LivingRoom");
                break;
            case 6:
                background.sprite = Resources.Load<Sprite>("Backgrounds/LivingRoom");
                break;
            case 7:
                background.sprite = Resources.Load<Sprite>("Backgrounds/LivingRoom");
                break;
            default:
                MyDebug.Log("No background set for this level.");
                break;
        }
    }
}