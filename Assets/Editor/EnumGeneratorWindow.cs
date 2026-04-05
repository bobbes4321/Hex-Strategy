using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class EnumGeneratorWindow : OdinEditorWindow
{
    [Serializable]
    public class Identifier
    {
        public string Category;
        public string Value;
    }

    [Title("Identifiers")]
    [ListDrawerSettings(ShowFoldout = true, ShowPaging = false)]
    public List<Identifier> Identifiers = new List<Identifier>();

    [Title("Settings")]
    [FolderPath(AbsolutePath = true, RequireExistingPath = true)]
    [Tooltip("The folder where the generated enums will be saved.")]
    public string OutputFolder = "Assets/Scripts/GeneratedEnums";

    [Button("Generate Enums", ButtonSizes.Large)]
    public void GenerateEnums()
    {
        if (Identifiers.Count == 0)
        {
            Debug.LogError("No identifiers provided!");
            return;
        }

        var groupedCategories = Identifiers
            .GroupBy(id => id.Category)
            .ToDictionary(g => g.Key, g => g.Select(id => id.Value).Distinct().ToList());

        if (!Directory.Exists(OutputFolder))
        {
            Directory.CreateDirectory(OutputFolder);
        }

        foreach (var category in groupedCategories)
        {
            GenerateEnumFile(category.Key, category.Value);
        }

        AssetDatabase.Refresh();
        Debug.Log("Enums generated successfully!");
    }

    private void GenerateEnumFile(string category, List<string> values)
    {
        string enumName = MakeValidIdentifier(category);
        string fileName = $"{enumName}.cs";
        string filePath = Path.Combine(OutputFolder, fileName);

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("// Auto-generated enum");
            writer.WriteLine("using System;");
            writer.WriteLine();
            writer.WriteLine($"public enum {enumName}");
            writer.WriteLine("{");

            for (int i = 0; i < values.Count; i++)
            {
                string validValue = MakeValidIdentifier(values[i]);
                writer.WriteLine($"    {validValue} = {i},");
            }

            writer.WriteLine("}");
        }
    }

    private string MakeValidIdentifier(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input cannot be null or whitespace.");

        // Remove invalid characters and capitalize first letters
        string sanitized = string.Concat(input
            .Where(char.IsLetterOrDigit)
            .Select((ch, index) => index == 0 ? char.ToUpper(ch) : ch));

        if (char.IsDigit(sanitized[0]))
        {
            sanitized = "_" + sanitized; // Prefix with underscore if it starts with a digit
        }

        return sanitized;
    }

    [MenuItem("Tools/Enum Generator")]
    private static void OpenWindow()
    {
        GetWindow<EnumGeneratorWindow>("Enum Generator").Show();
    }
}
