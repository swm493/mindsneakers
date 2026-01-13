using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections; // 코루틴 사용을 위해 추가
using UnityEngine.InputSystem;

public class DoubleDialogueView : MonoBehaviour, IView
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI leftSpeakerNameText;
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

    private void OnEnable()
    {
        InputManager.Instance.UI.Submit.performed += OnSubmitPerformed;
    }

    private void OnDisable()
    {
        InputManager.Instance.UI.Submit.performed -= OnSubmitPerformed;
    }

    private void OnSubmitPerformed(InputAction.CallbackContext context)
    {
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
        leftSpeakerNameText.text = leftSpeakerName;
        rightSpeakerNameText.text = rightSpeakerName;
    }

    public void SetSpeakerImage(string leftSpeakerImage, string rightSpeakerImage)
    {
        if (!string.IsNullOrEmpty(leftSpeakerImage))
            leftPortraitImage.sprite = Resources.Load<Sprite>(leftSpeakerImage);

        if (!string.IsNullOrEmpty(rightSpeakerImage))
            rightPortraitImage.sprite = Resources.Load<Sprite>(rightSpeakerImage);
    }

    public void PlaySpeakerAnimation(bool isLeft, string animationTrigger)
    {
        Animator targetAnimator = isLeft ? leftPortraitAnimator : rightPortraitAnimator;

        if (targetAnimator != null && !string.IsNullOrEmpty(animationTrigger))
        {
            targetAnimator.SetTrigger(animationTrigger);
        }
    }

    public void HighlightSpeaker(bool isLeft)
    {
        if (leftPortraitImage != null)
            leftPortraitImage.color = isLeft ? activeColor : inactiveColor;

        if (rightPortraitImage != null)
            rightPortraitImage.color = !isLeft ? activeColor : inactiveColor;
    }
}