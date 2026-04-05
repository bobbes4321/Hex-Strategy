using Runtime.BetterNody;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class NodeEditorWindow : EditorWindow
{
    private UIGraphView graphView;
    public UINodeGraph currentGraph;

    [MenuItem("Window/UI Node Editor")]
    public static void ShowWindow() => GetWindow<NodeEditorWindow>("UI Node Editor");

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }

    private void ConstructGraphView()
    {
        graphView = new UIGraphView(this)
        {
            name = "UI Graph"
        };

        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();

        toolbar.Add(new Button(() => SaveGraph()) { text = "Save" });
        toolbar.Add(new Button(() => LoadGraph()) { text = "Load" });

        rootVisualElement.Add(toolbar);
    }

    private void SaveGraph()
    {
        if (currentGraph == null) return;

        EditorUtility.SetDirty(currentGraph);
        AssetDatabase.SaveAssets();
    }

    private void LoadGraph()
    {
        string path = EditorUtility.OpenFilePanel("Load Graph", "Assets/", "asset");
        if (string.IsNullOrEmpty(path)) return;

        path = "Assets" + path.Substring(Application.dataPath.Length);
        currentGraph = AssetDatabase.LoadAssetAtPath<UINodeGraph>(path);

        graphView.ClearGraph();
        graphView.PopulateGraph(currentGraph);
    }
}