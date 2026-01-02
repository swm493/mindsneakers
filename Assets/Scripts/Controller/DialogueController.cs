using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private DoubleDialogue dialogueData;

    [Header("UI Text Components")]
    [SerializeField] private TextMeshProUGUI leftNameText;
    [SerializeField] private TextMeshProUGUI rightNameText;
    [SerializeField] private TextMeshProUGUI bodyText;

    [Header("UI Image Components")]
    [SerializeField] private Image leftPortrait;
    [SerializeField] private Image rightPortrait;

    [Header("Display Settings")]
    private readonly Color inactiveColor = new(0.5f, 0.5f, 0.5f, 1f);
    private readonly Color activeColor = Color.white;

    private float lastInputTime = 0f;
    private const float inputCooldown = 0.2f;

    void Start()
    {
        if (dialogueData != null)
        {
            DialogueManager.Instance.dialogueDB = dialogueData;
        }

        DialogueManager.Instance.RegisterController(this);

        SetupInput();
    }

    private void SetupInput()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.UI.Submit.performed += OnNextDialogue;
            InputManager.Instance.UI.Click.performed += OnNextDialogue;
        }
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.UI.Submit.performed -= OnNextDialogue;
            InputManager.Instance.UI.Click.performed -= OnNextDialogue;
        }
    }

    private void OnNextDialogue(InputAction.CallbackContext context)
    {
        if (Time.time - lastInputTime < inputCooldown) return;
        lastInputTime = Time.time;
        DialogueManager.Instance.NextDialogue();
    }

    public void SetDialogueUI(DoubleDialogueData data)
    {
        bodyText.text = data.Context;
        leftNameText.text = data.Leftname;
        rightNameText.text = data.Rightname;

        if (!string.IsNullOrEmpty(data.Leftimage))
        {
            Sprite leftSprite = Resources.Load<Sprite>(data.Leftimage);
            if (leftSprite != null) leftPortrait.sprite = leftSprite;
        }

        if (!string.IsNullOrEmpty(data.Rightimage))
        {
            Sprite rightSprite = Resources.Load<Sprite>(data.Rightimage);
            if (rightSprite != null) rightPortrait.sprite = rightSprite;
        }

        if (data.Activeside == "Left")
        {
            SetActiveState(isLeftActive: true);
        }
        else if (data.Activeside == "Right")
        {
            SetActiveState(isLeftActive: false);
        }
        else
        {
            SetAllInactive();
        }
    }

    private void SetActiveState(bool isLeftActive)
    {
        if (isLeftActive)
        {
            leftPortrait.color = activeColor;
            leftNameText.color = activeColor;
            leftPortrait.transform.SetAsLastSibling();

            rightPortrait.color = inactiveColor;
            rightNameText.color = inactiveColor;
        }
        else
        {
            rightPortrait.color = activeColor;
            rightNameText.color = activeColor;
            rightPortrait.transform.SetAsLastSibling();

            leftPortrait.color = inactiveColor;
            leftNameText.color = inactiveColor;
        }
    }

    private void SetAllInactive()
    {
        leftPortrait.color = inactiveColor;
        leftNameText.color = inactiveColor;

        rightPortrait.color = inactiveColor;
        rightNameText.color = inactiveColor;
    }
}