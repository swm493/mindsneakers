using UnityEngine;
using System.Collections.Generic;

public class DialogueManager : MonoSingleton<DialogueManager>
{
    [Header("QuickSheet Data")]
    public DoubleDialogue dialogueDB;

    private List<DoubleDialogueData> dataList;
    private int currentIndex = 0;

    private DialogueController controller;

    public void RegisterController(DialogueController newController)
    {
        controller = newController;
        StartDialogue();
    }

    public void StartDialogue()
    {
        if (dialogueDB != null) dataList = new List<DoubleDialogueData>(dialogueDB.dataArray);

        currentIndex = 0;
        DisplayCurrentDialogue();
    }

    public void NextDialogue()
    {
        currentIndex++;
        DisplayCurrentDialogue();
    }

    private void DisplayCurrentDialogue()
    {
        if (dataList == null || controller == null) return;

        if (currentIndex >= dataList.Count)
        {
            EndDialogue();
            return;
        }

        DoubleDialogueData currentData = dataList[currentIndex];
        controller.SetDialogueUI(currentData);
    }

    private void EndDialogue()
    {
        MyDebug.Log("대화 종료");
    }
}