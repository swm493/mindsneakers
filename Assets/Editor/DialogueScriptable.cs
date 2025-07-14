using UnityEngine;

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/Dialogue Data")]
public class DialogueScriptable : ScriptableObject
{
    public DialogueData dialogueData;
}
#endif