using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Image egoUI;
    public Image hormoneUI;
    public Image specialSkillUI;
    public Image electroAnestheisaUI;
    public Image cortisolUI;
    public Image feelMindUI;
}
