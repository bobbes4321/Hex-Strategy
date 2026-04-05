using System.Collections.Generic;
using Runtime.Attributes;
using Runtime.UI;
using UnityEngine;

namespace Runtime.BetterNody
{
    public class BetterUINode : ScriptableObject
    {
        public string nodeID; // GUID recommended
        [IdentifierDropdown] public Identifier Identifier = new();
        public Rect position; // Position in the graph view
        public string nodeName;
    
        [System.Serializable]
        public class PanelConnection
        {
            public string portID;
            public bool isInput;
            public Identifier identifier = new Identifier();
        }
    
        [SerializeField] private List<PanelConnection> _dynamicConnections = new();
        public List<PanelConnection> DynamicConnections => _dynamicConnections;
    }
}