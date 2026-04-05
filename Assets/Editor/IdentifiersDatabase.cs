using System.Collections.Generic;
using Doozy.Editor.Common.ScriptableObjects;
using Runtime.UI;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Create IdentifiersDatabase", fileName = "IdentifiersDatabase", order = 0)]
[FilePath("Settings/IdentifiersDatabase", FilePathAttribute.Location.PreferencesFolder)]
public class IdentifiersDatabase : SingletonEditorScriptableObject<IdentifiersDatabase>
{
    public List<Identifier> Identifiers = new List<Identifier>();
}