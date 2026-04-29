using UnityEngine;
using UnityEngine.Events;

public class DoubleDialogueController : BaseDialogueController<DoubleDialogueView, DoubleDialogueModel>
{
    [Header("Data")]
    [SerializeField] private DoubleDialogue dialogueData;

    [Header("Events")]
    public UnityEvent OnDialogueStart;
    public UnityEvent OnDialogueEnd;

    private int dialogueGroupId;

    private void OnEnable()
    {
        if (view != null) view.OnNextButtonClicked += HandleNextDialogue;
        SetInputMode(true);
        OnDialogueStart?.Invoke();
        Initialize();
        StartDialogue();
    }

    private void OnDisable()
    {
        if (view != null) view.OnNextButtonClicked -= HandleNextDialogue;
        SetInputMode(false);
    }

    protected override void Initialize()
    {
        base.Initialize();
        model = new DoubleDialogueModel(dialogueData);
        SetDialogueId(SaveManager.Instance.playerData.level);
        DialogueManager.Instance.RegisterController(this);
    }

    protected override void Release()
    {
        DialogueManager.Instance.UnregisterController(this);
        base.Release();
    }

    public override void SetDialogueId(int groupId) => dialogueGroupId = groupId;

    private void StartDialogue()
    {
        while (model.HasDialougeData() && model.GetCurrentDialogue()?.Groupid != dialogueGroupId) model.MoveNext();

        if (model.HasDialougeData())
        {
            RefreshView();
            ShowDialogue();
        }
        else
        {
            MyDebug.LogWarning($"No dialogue found for Group ID: {dialogueGroupId}");
            EndDialogueSequence();
        }
    }

    private void HandleNextDialogue()
    {
        if (view.IsTyping) { view.SkipTyping(); return; }

        do { model.MoveNext(); }
        while (model.HasDialougeData() && model.GetCurrentDialogue()?.Groupid != dialogueGroupId);

        if (model.HasDialougeData()) RefreshView();
        else { CloseDialogue(); EndDialogueSequence(); }
    }

    private void RefreshView()
    {
        DoubleDialogueData currentData = model.GetCurrentDialogue();
        if (currentData == null) return;
        view.Show();
        view.SetContent(currentData.Context);
        view.SetSpeakerName(currentData.Leftname, currentData.Rightname);
        view.SetSpeakerImage(currentData.Leftimage, currentData.Rightimage);
        view.HighlightSpeaker(currentData.Activeside);
    }

    private void EndDialogueSequence()
    {
        SaveManager.Instance.playerData.level += 1;
        SetInputMode(false);
        OnDialogueEnd?.Invoke();
    }

    private void SetInputMode(bool isDialogueMode)
    {
        var input = InputManager.Instance;
        if (input == null) return;
        if (isDialogueMode) { input.player.Disable(); input.UI.Enable(); }
        else { input.player.Enable(); input.UI.Disable(); }
    }
}