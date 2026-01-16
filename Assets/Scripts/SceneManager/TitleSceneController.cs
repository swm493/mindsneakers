using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleSceneController : MonoBehaviour
{
    public Button button;

    public void Start()
    {
        if (SaveManager.Instance.playerData.level > 1)
            button.interactable = false;
    }

    public void OnStartButtonClicked()
    {
        SaveManager.Instance.ResetData();

        SceneManager.LoadScene("GameScene");
    }

    public void OnSaveButtonClicked()
    {
        SceneManager.LoadScene("GameScene");
    }
}