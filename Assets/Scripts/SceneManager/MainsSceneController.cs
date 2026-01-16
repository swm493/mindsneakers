using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class MainSceneController : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private string nextSceneName = "TitleScene";

    private bool isTransitioning = false;

    private void Start()
    {
        SceneTransitionManager.Instance.gameObject.SetActive(true);
        InputSystem.onAnyButtonPress.CallOnce(ctrl => OnAnyKeyPress());
    }

    private void OnAnyKeyPress()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        SceneTransitionManager.Instance.LoadScene(nextSceneName);
    }
}