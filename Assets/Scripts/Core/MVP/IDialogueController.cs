using System;

public interface IDialogueController
{
    event Action OnDialogueComplete;

    void SetDialogueId(int id); 
    void ShowDialogue();
    void CloseDialogue();
}