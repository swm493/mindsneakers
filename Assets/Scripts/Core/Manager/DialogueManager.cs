using UnityEngine;
using System;

public class DialogueManager : MonoSingleton<DialogueManager>
{
    private IDialogueController activeController;
    public event Action OnDialogueEnded;

    public void RegisterController(IDialogueController newController)
    {
        if (activeController != null)
        {
            activeController.OnDialogueComplete -= HandleDialogueComplete;
        }

        activeController = newController;

        if (activeController != null)
        {
            activeController.OnDialogueComplete += HandleDialogueComplete;
        }
    }

    public void UnregisterController(IDialogueController existingController)
    {
        if (activeController == existingController)
        {
            activeController.OnDialogueComplete -= HandleDialogueComplete;
            activeController = null;
        }
    }

    public void PlayDialogue(int groupId)
    {
        if (activeController == null)
        {
            MyDebug.LogError("등록된 다이얼로그 컨트롤러가 없습니다!");
            return;
        }

        activeController.SetDialogueId(groupId);
        activeController.ShowDialogue();
    }

    private void HandleDialogueComplete()
    {
        OnDialogueEnded?.Invoke();
    }
}