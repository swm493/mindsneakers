using System.Collections.Generic;

public class DoubleDialogueModel : IModel
{
    private readonly DoubleDialogueData[] dialogueList;
    private int currentIndex = 0;

    public DoubleDialogueModel(DoubleDialogue data)
    {
        dialogueList = data.dataArray;
        currentIndex = 0;
    }

    public bool HasDialougeData()
    {
        return dialogueList != null && currentIndex < dialogueList.Length;
    }

    public DoubleDialogueData GetCurrentDialogue()
    {
        if (HasDialougeData())
        {
            return dialogueList[currentIndex];
        }
        return null;
    }

    public void MoveNext()
    {
        currentIndex++;
    }
}