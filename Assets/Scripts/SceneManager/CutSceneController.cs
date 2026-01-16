using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneController : MonoBehaviour
{
    [SerializeField] private GameObject[] cutScenes;
    private void Start()
    {
        int currentDay = SaveManager.Instance.playerData.day;
        switch (currentDay)
        {
            case 0:
                cutScenes[0].SetActive(true);
                break;
            case 1:
                cutScenes[1].SetActive(true);
                break;
            default:
                Debug.LogError("No cutscene available for this day.");
                break;
        }
    }

    public void EndCutScene()
    {
        int currentDay = SaveManager.Instance.playerData.day;
        switch (currentDay)
        {
            case 0:
                SaveManager.Instance.playerData.day += 1;
                SceneTransitionManager.Instance.LoadScene("DialogueScene");
                break;
            case 1:
                SceneTransitionManager.Instance.LoadScene("Stage1_Night_Pt1");
                break;
            default:
                Debug.LogError("No cutscene available for this day.");
                break;
        }
    }
}