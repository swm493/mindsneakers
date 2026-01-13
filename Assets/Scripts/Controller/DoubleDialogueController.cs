using UnityEngine;

public class DoubleDialogueController : BaseUIController<DoubleDialogueView, DoubleDialogueModel>
{
    [SerializeField] private DoubleDialogue dialogueData;
    private int dialogueGroupId;

    protected override void OnInitialize()
    {
        base.OnInitialize();
        model = new DoubleDialogueModel(dialogueData);
        view.OnNextButtonClicked += HandleNextDialogue;
    }

    protected override void Release()
    {
        view.OnNextButtonClicked -= HandleNextDialogue;
        base.Release();
    }

    public void StartDialogue(int groupId)
    {
        dialogueGroupId = groupId;
        while (model.HasDialougeData() && model.GetCurrentDialogue()?.Groupid != dialogueGroupId)
        {
            model.MoveNext();
        }

        if (model.HasDialougeData())
        {
            RefreshView();
        }
        else
        {
            MyDebug.LogWarning($"No dialogue found for Group ID: {dialogueGroupId}");
        }
    }

    private void HandleNextDialogue()
    {
        do
        {
            model.MoveNext();
        }
        while (model.HasDialougeData() && model.GetCurrentDialogue()?.Groupid != dialogueGroupId);

        if (model.HasDialougeData())
        {
            RefreshView();
        }
        else
        {
            CloseDialogue();
        }
    }

    private void RefreshView()
    {
        DoubleDialogueData currentData = model.GetCurrentDialogue();
        if (currentData == null) return;

        view.Show();

        view.SetContent(currentData.Context);
        view.SetSpeakerName(currentData.Leftname, currentData.Rightname);
        view.SetSpeakerImage(currentData.Leftimage, currentData.Rightimage);
        view.HighlightSpeaker(currentData.Activeside == "Left");
    }

    public void CloseDialogue()
    {
        view.Hide();
    }
}