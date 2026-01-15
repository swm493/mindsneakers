using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 씬 관리 기능 사용을 위해 추가

public class TitleScene : MonoBehaviour
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

        // 데이터 초기화 후 게임 씬(예: "GameScene")으로 전환
        // Build Settings에 등록된 씬 이름이어야 합니다.
        SceneManager.LoadScene("GameScene");
    }

    public void OnSaveButtonClicked()
    {
        // TODO: 저장된 데이터를 불러온 후 씬 전환
        // 예: 저장된 레벨에 따라 다른 씬을 로드하거나, 메인 게임 씬으로 이동
        // string sceneName = "Level" + SaveManager.Instance.playerData.level;
        // SceneManager.LoadScene(sceneName);

        // 여기서는 예시로 "GameScene"으로 이동합니다.
        SceneManager.LoadScene("GameScene");
    }
}