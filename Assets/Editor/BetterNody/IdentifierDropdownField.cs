using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class IdentifierDropdownField : VisualElement
{
    public IdentifierDropdownField(SerializedProperty property)
    {
        // Create IMGUI container to draw the dropdown using the existing attribute
        Add(new IMGUIContainer(() => {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(property, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }
        }));
    }
}