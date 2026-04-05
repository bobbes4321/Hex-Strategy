using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using Runtime.DataBinding;

[CustomPropertyDrawer(typeof(ReflectedValue<>), true)]
public class ReflectedValueDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        var targetTypeProp = property.FindPropertyRelative("TargetType");
        var componentProp = property.FindPropertyRelative("TargetComponent");
        var memberNameProp = property.FindPropertyRelative("MemberName");
        var targetProp = property.FindPropertyRelative("Target");

        var lineHeight = EditorGUIUtility.singleLineHeight;
        var spacing = EditorGUIUtility.standardVerticalSpacing;

        Rect rect = new Rect(position.x, position.y, position.width, lineHeight);

        EditorGUI.PropertyField(rect, targetProp);
        rect.y += lineHeight + spacing;

        TargetType targetType = (TargetType)targetTypeProp.enumValueIndex;

        if (targetProp.objectReferenceValue != null)
        {
            if (targetProp.objectReferenceValue is GameObject)
                targetTypeProp.enumValueIndex = (int)TargetType.GameObject;
            else if (targetProp.objectReferenceValue is ScriptableObject)
                targetTypeProp.enumValueIndex = (int)TargetType.ScriptableObject;
            else 
                targetTypeProp.enumValueIndex = (int)TargetType.Object;
        }

        UnityEngine.Object targetObject = null;
        Type reflectedType = null;

        if (targetType == TargetType.GameObject)
        {
            GameObject go = targetProp.objectReferenceValue as GameObject;
            if (go != null)
            {
                Component[] components = go.GetComponents<Component>();
                string[] options = components.Select(c => c.GetType().Name).ToArray();

                int currentIndex = Array.IndexOf(components, componentProp.objectReferenceValue as Component);
                if (currentIndex < 0) currentIndex = 0;

                int selectedIndex = EditorGUI.Popup(rect, "Component", currentIndex, options);
                rect.y += lineHeight + spacing;

                componentProp.objectReferenceValue = components[selectedIndex];
                targetObject = components[selectedIndex];
                reflectedType = components[selectedIndex].GetType();
            }
        }
        else 
        {
            if (targetProp.objectReferenceValue != null)
            {
                targetObject = targetProp.objectReferenceValue;
                reflectedType = targetObject.GetType();
            }
        }

        if (targetObject != null && reflectedType != null)
        {
            var members = reflectedType
                .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m =>
                    (m.MemberType == MemberTypes.Field && ((FieldInfo)m).FieldType == fieldInfo.FieldType.GenericTypeArguments[0]) ||
                    (m.MemberType == MemberTypes.Property && ((PropertyInfo)m).PropertyType == fieldInfo.FieldType.GenericTypeArguments[0])
                )
                .Select(m => m.Name)
                .ToArray();

            int selectedIndex = Array.IndexOf(members, memberNameProp.stringValue);
            if (selectedIndex < 0) selectedIndex = 0;

            selectedIndex = EditorGUI.Popup(rect, "Member", selectedIndex, members);
            rect.y += lineHeight + spacing;

            if (members.Length > 0)
                memberNameProp.stringValue = members[selectedIndex];
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var targetProp = property.FindPropertyRelative("Target");

        float lines = -1f;
        if (targetProp.objectReferenceValue != null)
            lines += 1f;

        var targetType = (TargetType)property.FindPropertyRelative("TargetType").enumValueIndex;
        if (targetType == TargetType.GameObject)
            lines += 2f; // GameObject + Component
        else 
            lines += 1f;

        return lines * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
    }
}