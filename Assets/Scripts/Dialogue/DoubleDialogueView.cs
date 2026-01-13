using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEngine.InputSystem;

public class DoubleDialogueView : MonoBehaviour, IView
{
    [Header("UI Components")]
    [SerializeField] private GameObject leftNameBox;
    [SerializeField] private TextMeshProUGUI leftSpeakerNameText;
    [SerializeField] private GameObject rightNameBox;
    [SerializeField] private TextMeshProUGUI rightSpeakerNameText;
    [SerializeField] private TextMeshProUGUI contentText;

    [Header("Portraits")]
    [SerializeField] private Image leftPortraitImage;
    [SerializeField] private Image rightPortraitImage;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = new(0.5f, 0.5f, 0.5f, 1f);

    [Header("Typewriter Settings")]
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("Animation Components")]
    [SerializeField] private Animator leftPortraitAnimator;
    [SerializeField] private Animator rightPortraitAnimator;

    public event Action OnNextButtonClicked;

    public bool IsTyping { get; private set; }
    private Coroutine typingCoroutine;
    private string currentFullContent;

    private string currentLeftImageName;
    private string currentRightImageName;

    private void OnEnable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.UI.Submit.performed += OnSubmitPerformed;
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.UI.Submit.performed -= OnSubmitPerformed;
    }

    private void OnSubmitPerformed(InputAction.CallbackContext context)
    {
        if (IsTyping)
            SkipTyping();
        else
            OnNextButtonClicked?.Invoke();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetContent(string content)
    {
        currentFullContent = content;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeWriterEffect(content));
    }

    private IEnumerator TypeWriterEffect(string content)
    {
        IsTyping = true;
        contentText.text = "";

        foreach (char c in content)
        {
            contentText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        IsTyping = false;
    }

    public void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        contentText.text = currentFullContent;
        IsTyping = false;
    }

    public void SetSpeakerName(string leftSpeakerName, string rightSpeakerName)
    {
        UpdateNameBox(leftNameBox, leftSpeakerNameText, leftSpeakerName);
        UpdateNameBox(rightNameBox, rightSpeakerNameText, rightSpeakerName);
    }

    private void UpdateNameBox(GameObject nameBox, TextMeshProUGUI nameText, string name)
    {
        bool show = !string.IsNullOrEmpty(name) && name != "None";

        if (nameBox != null)
            nameBox.SetActive(show);

        if (show && nameText != null)
            nameText.text = name;
    }

    public void SetSpeakerImage(string leftSpeakerImage, string rightSpeakerImage)
    {
        UpdatePortrait(leftPortraitImage, leftSpeakerImage, ref currentLeftImageName);
        UpdatePortrait(rightPortraitImage, rightSpeakerImage, ref currentRightImageName);
    }

    private void UpdatePortrait(Image portrait, string imageName, ref string currentImageName)
    {
        if (portrait == null) return;

        bool show = !string.IsNullOrEmpty(imageName) && imageName != "None";

        if (!show)
        {
            portrait.gameObject.SetActive(false);
            return;
        }

        if (imageName == currentImageName)
        {
            if (portrait.sprite != null)
                portrait.gameObject.SetActive(true);
            return;
        }

        currentImageName = imageName;
        string folderPath = "Characters/";
        Sprite sprite = Resources.Load<Sprite>(folderPath + imageName);

        if (sprite != null)
        {
            portrait.sprite = sprite;
            portrait.gameObject.SetActive(true);
        }
        else
        {
            MyDebug.LogError($"Image not found at path: {folderPath + imageName}");
            portrait.gameObject.SetActive(false);
        }
    }

    public void PlaySpeakerAnimation(bool isLeft, string animationTrigger)
    {
        if (isLeft && (leftPortraitImage == null || !leftPortraitImage.gameObject.activeSelf)) return;
        if (!isLeft && (rightPortraitImage == null || !rightPortraitImage.gameObject.activeSelf)) return;

        Animator targetAnimator = isLeft ? leftPortraitAnimator : rightPortraitAnimator;

        if (targetAnimator != null && !string.IsNullOrEmpty(animationTrigger) && animationTrigger != "None")
            targetAnimator.SetTrigger(animationTrigger);
    }

    public void HighlightSpeaker(string activeSide)
    {
        bool isLeftActive = activeSide == "Left";
        bool isRightActive = activeSide == "Right";

        if (leftPortraitImage != null && leftPortraitImage.gameObject.activeSelf)
            leftPortraitImage.color = isLeftActive ? activeColor : inactiveColor;

        if (rightPortraitImage != null && rightPortraitImage.gameObject.activeSelf)
            rightPortraitImage.color = isRightActive ? activeColor : inactiveColor;
    }
}