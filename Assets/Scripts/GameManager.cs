using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private Texture2D cursorTexture;
    private Texture2D redDot;

    public int stageNumber = 1;
    public int gravityScale = 6;

    private void Start()
    {
        cursorTexture = Resources.Load<Texture2D>("MouseCursor");
        redDot = Resources.Load<Texture2D>("RedDot");
        CursorActive();
    }

    public void CursorActive()
    {
        Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width / 2f, cursorTexture.height / 2f), CursorMode.Auto);
    }

    public void CursorInactive()
    {
        Cursor.SetCursor(redDot, new Vector2(cursorTexture.width / 2f, cursorTexture.height / 2f), CursorMode.Auto);
    }
}
