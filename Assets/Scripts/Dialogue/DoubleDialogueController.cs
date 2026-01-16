using UnityEngine;
using System;

public class DoubleDialogueController : BaseDialogueController<DoubleDialogueView, DoubleDialogueModel>
{
    [SerializeField] private DoubleDialogue dialogueData;
    [SerializeField] private int dialogueGroupId;

    private void OnEnable()
    {
        view.OnNextButtonClicked += HandleNextDialogue;
    }

    private void OnDisable()
    {
        view.OnNextButtonClicked -= HandleNextDialogue;
    }

    protected override void Initialize()
    {
        base.Initialize();

        OnDialogueStart += StartDialogue;
        DialogueManager.Instance.RegisterController(this);

        model = new DoubleDialogueModel(dialogueData);
    }

    public override void SetDialogueId(int groupId)
    {
        dialogueGroupId = groupId;
    }

    public void StartDialogue()
    {
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
        if (view.IsTyping)
        {
            view.SkipTyping();
            return;
        }

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
        view.HighlightSpeaker(currentData.Activeside);

        // string animTrigger = currentData.Activeside == "Left" ? currentData.LeftAnim : currentData.RightAnim;
        // view.PlaySpeakerAnimation(currentData.Activeside == "Left", animTrigger);
    }
}