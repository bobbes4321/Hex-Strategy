using System.Collections.Generic;
using UnityEngine;

namespace Runtime.BetterNody
{
    [CreateAssetMenu(fileName = "NewUINodeGraph", menuName = "UI/Node Graph")]
    public class UINodeGraph : ScriptableObject
    {
        public List<BetterUINode> nodes = new List<BetterUINode>();
        public List<NodeConnection> connections = new List<NodeConnection>();
    }


    [System.Serializable]
    public class NodeConnection
    {
        public string sourceNodeID;
        public string sourcePortID;
        public string targetNodeID;
        public string targetPortID;
    }
}