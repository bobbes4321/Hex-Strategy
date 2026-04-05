using Runtime.DataBinding;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BindableValue), true)]
public class BindableValueDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var lineHeight = EditorGUIUtility.singleLineHeight;
        var spacing = EditorGUIUtility.standardVerticalSpacing;

        Rect rect = new Rect(position.x, position.y, position.width, lineHeight);

        var valueTypeProp = property.FindPropertyRelative("ValueType");

        EditorGUI.PropertyField(rect, valueTypeProp);
        rect.y += lineHeight + spacing;

        var transformersProp = property.FindPropertyRelative("Transformers");
        EditorGUI.PropertyField(rect, transformersProp);
        rect.y += EditorGUI.GetPropertyHeight(transformersProp) + spacing;
        
        ValueType targetType = (ValueType)valueTypeProp.enumValueIndex;
        EditorGUI.PropertyField(rect, property.FindPropertyRelative($"{targetType}Value"));
        rect.y += lineHeight + spacing;
        
       

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var valueTypeProp = property.FindPropertyRelative("ValueType");
        ValueType targetType = (ValueType)valueTypeProp.enumValueIndex;
        var reflectedValueProp = property.FindPropertyRelative($"{targetType}Value");
        return 2 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) + EditorGUI.GetPropertyHeight(reflectedValueProp) + EditorGUI.GetPropertyHeight(property.FindPropertyRelative($"Transformers"));
    }
}