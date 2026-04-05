using System.Linq;
using Runtime.Attributes;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IdentifierDropdownAttribute))]
public class IdentifierDropdownDrawer : PropertyDrawer
{
    private IdentifiersDatabase database;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Load the IdentifierDatabase (ensure it's in a Resources folder or singleton instance)
        if (database == null)
        {
            database = IdentifiersDatabase.instance;
        }

        // Get the Category and Value fields
        var categoryProp = property.FindPropertyRelative("Category");
        var valueProp = property.FindPropertyRelative("Value");

        // Get unique categories and values
        var categories = database.Identifiers.Select(i => i.Category).Distinct().ToList();
        var valuesForCategory = database.Identifiers
            .Where(i => i.Category == categoryProp.stringValue)
            .Select(i => i.Value)
            .Distinct()
            .ToList();

        // Calculate heights for proper spacing
        float singleLineHeight = EditorGUIUtility.singleLineHeight;
        float padding = EditorGUIUtility.standardVerticalSpacing;

        // Ensure enough space for the custom property
        position.height = singleLineHeight * 2 + padding * 3;

        EditorGUI.BeginProperty(position, label, property);

        // Label
        Rect labelRect = new Rect(position.x, position.y, position.width, singleLineHeight);
        EditorGUI.LabelField(labelRect, label);

        // "+" Button
        float buttonWidth = 20f;
        Rect buttonRect = new Rect(position.x, position.y + singleLineHeight + padding, buttonWidth, singleLineHeight);
        if (GUI.Button(buttonRect, "+"))
        {
            Selection.activeObject = database;
        }

        // Category Dropdown
        Rect categoryRect = new Rect(position.x + buttonWidth + 2, position.y + singleLineHeight + padding, position.width / 2, singleLineHeight);
        int categoryIndex = Mathf.Max(0, categories.IndexOf(categoryProp.stringValue));
        categoryIndex = EditorGUI.Popup(categoryRect, categoryIndex, categories.ToArray());
        if (categoryIndex <= categories.Count - 1)
            categoryProp.stringValue = categories[categoryIndex];

        // Value Dropdown
        Rect valueRect = new Rect(position.x + buttonWidth + position.width / 2 + 5, position.y + singleLineHeight + padding, (position.width - buttonWidth) / 2 - 15, singleLineHeight);
        int valueIndex = Mathf.Max(0, valuesForCategory.IndexOf(valueProp.stringValue));
        valueIndex = EditorGUI.Popup(valueRect, valueIndex, valuesForCategory.ToArray());
        if (valueIndex <= valuesForCategory.Count - 1)
            valueProp.stringValue = valuesForCategory[valueIndex];

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Adjust property height to accommodate two dropdowns
        float singleLineHeight = EditorGUIUtility.singleLineHeight;
        float padding = EditorGUIUtility.standardVerticalSpacing;
        return singleLineHeight * 2 + padding * 3;
    }
}