using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleSceneController : MonoBehaviour
{
    public Button button;

    public void Start()
    {
        if (SaveManager.Instance.playerData.day > 1)
            button.interactable = false;
    }

    public void OnStartButtonClicked()
    {
        SaveManager.Instance.ResetData();

        SceneTransitionManager.Instance.LoadScene("CutScene");
    }

    public void OnSaveButtonClicked()
    {
        SceneTransitionManager.Instance.LoadScene("DialogueScene");
    }
}