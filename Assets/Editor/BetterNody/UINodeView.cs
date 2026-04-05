using System.Collections.Generic;
using Runtime.BetterNody;
using Runtime.UI;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class UINodeView : Node
{
    public BetterUINode NodeData { get; }
    private Dictionary<string, Port> _ports = new();

    private SerializedObject _serializedNode;
    private SerializedProperty _identifierProp;
    
    private VisualElement _connectionContainer;
    private Button _addButton;
    public UINodeView(BetterUINode node)
    {
        if (node == null)
            return;

        NodeData = node;
        viewDataKey = node.nodeID;
        SetPosition(node.position);

        CreatePorts();

        SetupIdentifierField();
        RefreshExpandedState();
        CreateDynamicConnectionUI();
    }
    
    private void CreateDynamicConnectionUI()
    {
        _connectionContainer = new VisualElement();
        mainContainer.Add(_connectionContainer);

        // Header
        var header = new VisualElement();
        header.style.flexDirection = FlexDirection.Row;
        header.style.justifyContent = Justify.SpaceBetween;
        header.Add(new Label("Connections"));
        
        // Add button
        _addButton = new(AddNewConnection) { text = "+" };
        header.Add(_addButton);
        
        mainContainer.Insert(0, header);

        RefreshConnections();
    }
    
    private void RefreshConnections()
    {
        // Clear existing connection UI
        _connectionContainer.Clear();

        // Create UI for each connection
        for (int i = 0; i < NodeData.DynamicConnections.Count; i++)
        {
            int index = i; // Capture index for closure
            var connection = NodeData.DynamicConnections[index];
            
            // Create row container
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.marginBottom = 2;

            // Identifier dropdown
            var identifierField = new IdentifierDropdownField(_identifierProp);
            identifierField.style.flexGrow = 1;
            
            // Input port
            var port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            port.portName = connection.portID;
            port.name = connection.portID;
            
            // Delete button
            var deleteButton = new Button(() => RemoveConnection(NodeData.DynamicConnections[index])) { 
                text = "×",
                style = { 
                    width = 20,
                    height = 20,
                    marginLeft = 4
                }
            };

            row.Add(identifierField);
            row.Add(port);
            row.Add(deleteButton);
            
            _connectionContainer.Add(row);
        }
    }
    
    private void AddNewConnection()
    {
        var newConnection = new BetterUINode.PanelConnection
        {
            portID = GUID.Generate().ToString(),
            identifier = new() { Category = "", Value = "" }
        };

        NodeData.DynamicConnections.Add(newConnection);
        RefreshConnections();
        EditorUtility.SetDirty(NodeData);
    }

    private void RemoveConnection(BetterUINode.PanelConnection connection)
    {
        NodeData.DynamicConnections.Remove(connection);
        RefreshConnections();
        EditorUtility.SetDirty(NodeData);
        
       // // Remove any connections using this port
       // var edgesToRemove = edges.ToList()
       //     .Where(e => e.input?.portName == connection.portID)
       //     .ToList();
//
       // foreach (var edge in edgesToRemove)
       // {
       //     edge.input.Disconnect(edge);
       //     edge.output.Disconnect(edge);
       //     RemoveElement(edge);
       // }
    }

    private void SetupIdentifierField()
    {
        // Create serialized object for the node data
        _serializedNode = new SerializedObject(NodeData);
        _identifierProp = _serializedNode.FindProperty("Identifier");

        // Create container for the identifier
        var identifierContainer = new VisualElement();
        identifierContainer.style.marginTop = 5;
        identifierContainer.style.marginBottom = 5;

        // Add label and dropdown
        identifierContainer.Add(new Label("UI Identifier"));
        identifierContainer.Add(new IdentifierDropdownField(_identifierProp));

        // Add to main node body
        mainContainer.Add(identifierContainer);

        var identifier = _identifierProp.boxedValue as Identifier;
        title = $"{identifier.Category} - {identifier.Value}";
    }

    private void CreatePorts()
    {
        foreach (var port in NodeData.DynamicConnections)
        {
            var direction = port.isInput ? Direction.Input : Direction.Output;
            var capacity = port.isInput ? Port.Capacity.Single : Port.Capacity.Multi;

            var portView = InstantiatePort(
                Orientation.Horizontal,
                direction,
                capacity,
                typeof(bool)); // Use a common type for all ports

            portView.portName = port.portID;
            portView.name = port.portID;
            _ports.Add(port.portID, portView);

            if (port.isInput)
            {
                inputContainer.Add(portView);
            }
            else
            {
                outputContainer.Add(portView);
            }
        }
    }

    public Port GetPort(string portID)
    {
        return _ports.TryGetValue(portID, out var port) ? port : null;
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        
        if (_serializedNode != null)
            _serializedNode.Update();
        
        NodeData.position = newPos;
    }
}