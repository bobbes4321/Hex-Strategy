using _Strategy.Runtime.Settings;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class CreateNewEnemySettings
{
    [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
    public EnemySettings previewObject;

    public CreateNewEnemySettings()
    {
        previewObject = ScriptableObject.CreateInstance<EnemySettings>();
        previewObject.Type = SettingsType.Enemy;
        previewObject.Name = "New Enemy";
        previewObject.Description = "Enter enemy description here...";
    }

    [Button("Create Enemy Settings", ButtonSizes.Large)]
    [PropertyOrder(-1)]
    private void CreateNewData()
    {
        string path = EditorUtility.SaveFilePanel("Create new Enemy Settings", "Assets/Settings", previewObject.Name, "asset");
        
        if (!string.IsNullOrEmpty(path))
        {
            path = FileUtil.GetProjectRelativePath(path);
            var newSettings = ScriptableObject.CreateInstance<EnemySettings>();
            newSettings.Type = previewObject.Type;
            newSettings.Name = previewObject.Name;
            newSettings.Description = previewObject.Description;

            AssetDatabase.CreateAsset(newSettings, path);
            AssetDatabase.SaveAssets();

            Selection.activeObject = newSettings;
            EditorGUIUtility.PingObject(newSettings);
        }
    }
}