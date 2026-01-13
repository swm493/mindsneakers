using UnityEngine;
using System.Collections.Generic;

public class DialogueManager : MonoSingleton<DialogueManager>
{
    private IDialoguePresenter controller;

    public void RegisterController(IDialoguePresenter newController)
    {
        controller = newController;
    }

    public void StartDialogue()
    {
    }
}