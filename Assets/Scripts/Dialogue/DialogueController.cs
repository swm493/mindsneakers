using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private string _dialoguePath;
    [SerializeField] private TMP_Text _contentText;
    [SerializeField] private TMP_Text _nameText1;
    [SerializeField] private TMP_Text _nameText2;

    private DialogueData _dialogueData;
    private Dialogue _currentDialogue;

    private bool _isDialogueEnd = false;
    private int _currentDialogueTextIndex = 0;
    private int _currentDialogueIndex = 0;
    private int _dialogueCount = 0;

    private void Awake()
    {
        _dialogueData = JsonLoader<DialogueData>.LoadJson(_dialoguePath);
        _dialogueCount = _dialogueData.dialogues.Length;
    }

    private void OnEnable()
    {
        InputManager.Instance.UI.Submit.performed += NextDialogue;
        ChangeDialogue();
        DisplayDialogue();
    }

    private void OnDisable()
    {
        InputManager.Instance.UI.Submit.performed -= NextDialogue;
    }

    public void NextDialogue(InputAction.CallbackContext context)
    {
        if (context.canceled)
            return;

        if (!_isDialogueEnd){
            DisplayDialogue();
            return;
        }

        if (++_currentDialogueIndex < _dialogueCount)
            ChangeDialogue();
        else
            EndDialogue();
    }

    private void DisplayDialogue()
    {
        _contentText.text = _currentDialogue.texts[_currentDialogueTextIndex++];

        if (_currentDialogueTextIndex >= _currentDialogue.texts.Length)
            _isDialogueEnd = true;
    }

    private void ChangeDialogue()
    {
        _currentDialogue = _dialogueData.dialogues[_currentDialogueIndex];
        _currentDialogueTextIndex = 0;

        _isDialogueEnd = false;
    }

    private void EndDialogue()
    {
        gameObject.SetActive(false);
    }
}