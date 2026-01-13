using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
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

    public event Action OnNextButtonClicked;

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
        contentText.text = content;
    }

    public void SetSpeakerName(string leftSpeakerName, string rightSpeakerName)
    {
        leftSpeakerNameText.text = leftSpeakerName;
        rightSpeakerNameText.text = rightSpeakerName;
    }

    public void SetSpeakerImage(string leftSpeakerImage, string rightSpeakerImage)
    {
        leftPortraitImage.sprite = Resources.Load<Sprite>(leftSpeakerImage);
        rightPortraitImage.sprite = Resources.Load<Sprite>(rightSpeakerImage);
    }

    public void HighlightSpeaker(bool isLeft)
    {
        if (leftPortraitImage != null)
            leftPortraitImage.color = isLeft ? activeColor : inactiveColor;
            
        if (rightPortraitImage != null)
            rightPortraitImage.color = !isLeft ? activeColor : inactiveColor;
    }
}