using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionManager : MonoSingleton<SceneTransitionManager>
{
    private static readonly WaitForSeconds _waitForSeconds0_1 = new(0.1f);

    [Header("Settings")]
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private Color fadeColor = new Color32(15, 15, 15, 255);

    protected override void Awake()
    {
        base.Awake();
        if (fadePanel == null)
        {
            CreateFadeUI();
        }
    }

    private void Start()
    {
        if (fadePanel != null)
        {
            fadePanel.blocksRaycasts = false;
            StartCoroutine(FadeRoutine(1f, 0f));
        }
    }

    private void CreateFadeUI()
    {
        GameObject canvasObj = new("TransitionCanvas");
        canvasObj.transform.SetParent(transform);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767;

        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject panelObj = new("FadePanel");
        panelObj.transform.SetParent(canvasObj.transform, false);

        RectTransform rect = panelObj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        Image image = panelObj.AddComponent<Image>();
        image.color = fadeColor;
        image.raycastTarget = true;

        fadePanel = panelObj.AddComponent<CanvasGroup>();
        fadePanel.alpha = 0f;
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        InputManager.Instance.DisableInput();
        if (fadePanel != null)
        {
            fadePanel.blocksRaycasts = true;
            yield return StartCoroutine(FadeRoutine(0f, 1f));
        }

        SceneManager.LoadScene(sceneName);

        yield return _waitForSeconds0_1;

        if (fadePanel != null)
        {
            yield return StartCoroutine(FadeRoutine(1f, 0f));
            fadePanel.blocksRaycasts = false;
        }
        InputManager.Instance.EnableInput();
    }

    private IEnumerator FadeRoutine(float startAlpha, float endAlpha)
    {
        float timer = 0f;
        fadePanel.alpha = startAlpha;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
            yield return null;
        }

        fadePanel.alpha = endAlpha;
    }
}