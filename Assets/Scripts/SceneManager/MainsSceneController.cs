using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using TMPro;
using System.Collections;

public class MainSceneController : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private string nextSceneName = "TitleScene";

    [Header("페이드 효과")]
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private float fadeDuration = 1.0f;

    private bool isTransitioning = false;

    private void Start()
    {
        if (fadePanel != null)
        {
            fadePanel.alpha = 1f;
            StartCoroutine(FadeInRoutine());
        }

        InputSystem.onAnyButtonPress.CallOnce(ctrl => OnAnyKeyPress());
    }

    private void OnAnyKeyPress()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        StartCoroutine(FadeOutAndLoad());
    }

    private IEnumerator FadeInRoutine()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        fadePanel.alpha = 0f;
    }

    private IEnumerator FadeOutAndLoad()
    {
        if (fadePanel != null)
        {
            float startAlpha = fadePanel.alpha;
            float timer = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                fadePanel.alpha = Mathf.Lerp(startAlpha, 1f, timer / fadeDuration);
                yield return null;
            }
            fadePanel.alpha = 1f;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}