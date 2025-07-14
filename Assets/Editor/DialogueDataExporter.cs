using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(DialogueScriptable))]
public class DialogueDataExporter : Editor
{
    private readonly string prefix = "Assets/Editor/";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DialogueScriptable data = (DialogueScriptable)target;

        if (GUILayout.Button("Export to JSON"))
        {
            string dataPath = AssetDatabase.GetAssetPath(data);

            if (dataPath.StartsWith(prefix, System.StringComparison.OrdinalIgnoreCase))
            {
                dataPath = Path.Combine(
                    Application.streamingAssetsPath,
                    dataPath[prefix.Length..^Path.GetFileName(dataPath).Length]
                );

                if (!Directory.Exists(dataPath))
                    Directory.CreateDirectory(dataPath);
            }
            else
            {
                Debug.LogWarning($"❌ 올바른 경로가 아닙니다. {prefix} 로 옮겨주세요");
                return;
            }

            string path = EditorUtility.SaveFilePanel(
                "Save JSON file",
                dataPath,
                data.name,
                "json"
            );

            if (!string.IsNullOrEmpty(path))
            {
                string json = JsonConvert.SerializeObject(data.dialogueData, Formatting.Indented);
                File.WriteAllText(path, json);
                Debug.Log($"✅ JSON 저장 완료: {path}");
                AssetDatabase.Refresh();
            }
        }
    }
}
#endif