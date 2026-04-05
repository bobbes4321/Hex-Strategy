using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.BetterNody;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class UIGraphView : GraphView
{
    private NodeEditorWindow editorWindow;

    public UIGraphView(NodeEditorWindow window)
    {
        editorWindow = window;

        AddGridBackground();
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new ContentZoomer() { minScale = 0.7f, maxScale = 1.5f });

        AddElement(new Group());

        graphViewChanged += HandleGraphViewChanged;
    }

    private GraphViewChange HandleGraphViewChanged(GraphViewChange changes)
    {
        if (changes.edgesToCreate != null)
        {
            foreach (Edge edge in changes.edgesToCreate)
            {
                AddConnectionToData(edge);
            }
        }

        if (changes.elementsToRemove != null)
        {
            foreach (GraphElement element in changes.elementsToRemove)
            {
                if (element is Edge edge)
                {
                    RemoveConnectionFromData(edge);
                }
            }
        }

        return changes;
    }
    
    private void AddConnectionToData(Edge edge)
    {
        var outputNode = edge.output.node as UINodeView;
        var inputNode = edge.input.node as UINodeView;

        if (outputNode != null && inputNode != null)
        {
            var connection = new NodeConnection
            {
                sourceNodeID = outputNode.NodeData.nodeID,
                sourcePortID = edge.output.portName,
                targetNodeID = inputNode.NodeData.nodeID,
                targetPortID = edge.input.portName
            };

            if (!editorWindow.currentGraph.connections.Contains(connection))
            {
                editorWindow.currentGraph.connections.Add(connection);
                EditorUtility.SetDirty(editorWindow.currentGraph);
            }
        }
    }

    private void RemoveConnectionFromData(Edge edge)
    {
        var outputNode = edge.output.node as UINodeView;
        var inputNode = edge.input.node as UINodeView;

        if (outputNode != null && inputNode != null)
        {
            var connection = editorWindow.currentGraph.connections.Find(c =>
                c.sourceNodeID == outputNode.NodeData.nodeID &&
                c.sourcePortID == edge.output.portName &&
                c.targetNodeID == inputNode.NodeData.nodeID &&
                c.targetPortID == edge.input.portName);

            if (connection != null)
            {
                editorWindow.currentGraph.connections.Remove(connection);
                EditorUtility.SetDirty(editorWindow.currentGraph);
            }
        }
    }

    private void AddGridBackground()
    {
        GridBackground grid = new GridBackground();
        grid.StretchToParentSize();
        Insert(0, grid);
    }

    public void ClearGraph()
    {
        DeleteElements(graphElements.ToList());
    }
    
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction &&
            endPort.node != startPort.node).ToList();
    }

    public void PopulateGraph(UINodeGraph graph)
    {
        // Temporarily remove callback to prevent duplicate entries
        graphViewChanged -= HandleGraphViewChanged;
        
        ClearGraph();
        
        if (graph == null) return;

        // Create nodes
        foreach (var node in graph.nodes)
        {
            var nodeView = new UINodeView(node);
            AddElement(nodeView);
        }

        // Create connections
        foreach (var connection in graph.connections)
        {
            var sourceNode = GetNodeByGuid(connection.sourceNodeID) as UINodeView;
            var targetNode = GetNodeByGuid(connection.targetNodeID) as UINodeView;

            if (sourceNode == null || targetNode == null) continue;

            var outputPort = sourceNode.GetPort(connection.sourcePortID);
            var inputPort = targetNode.GetPort(connection.targetPortID);

            if (outputPort == null || inputPort == null) continue;

            try
            {
                var edge = outputPort.ConnectTo(inputPort);
                AddElement(edge);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create connection: {e.Message}");
            }
        }

        // Re-enable callback
        graphViewChanged += HandleGraphViewChanged;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        base.BuildContextualMenu(evt);

        Vector2 mousePos = evt.localMousePosition;

        evt.menu.AppendAction("Create Panel Node",
            _ => CreateNode("Panel Node", mousePos));
    }

    private void CreateNode(string nodeName, Vector2 position)
    {
        if (editorWindow.currentGraph == null)
        {
            Debug.LogWarning("No graph selected!");
            return;
        }

        BetterUINode newNode = ScriptableObject.CreateInstance<BetterUINode>();
        newNode.name = string.Format("{0} {1}", nodeName, editorWindow.currentGraph.nodes.Count);
        AssetDatabase.AddObjectToAsset(newNode, editorWindow.currentGraph);
        AssetDatabase.SaveAssets();
        
        newNode.nodeID = GUID.Generate().ToString();
        newNode.position = new Rect(position, new Vector2(200, 100));
        newNode.nodeName = nodeName;

        editorWindow.currentGraph.nodes.Add(newNode);
        var nodeView = new UINodeView(newNode);
        AddElement(nodeView);

        Undo.RegisterCompleteObjectUndo(editorWindow.currentGraph, "Create Node");
        EditorUtility.SetDirty(editorWindow.currentGraph);
    }
}