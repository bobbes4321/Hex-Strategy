using _Strategy.Runtime.Deck;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Strategy.Runtime.Settings
{
    public class EnemySettings : Settings
    {
        [BoxGroup("Basic Info")]
        [PropertyOrder(0)]
        [LabelWidth(100)]
        public string Name;
    
        [BoxGroup("Basic Info")]
        [PropertyOrder(1)]
        [MultiLineProperty(3)]
        [LabelWidth(100)]
        public string Description;
    
        [BoxGroup("Basic Info")]
        [PropertyOrder(2)]
        [MinValue(0)]
        [LabelWidth(100)]
        public int Health = 100;
        
        [BoxGroup("Commands")]
        [PropertyOrder(3)]
        [LabelWidth(100)]
        [SerializeReference]
        public AttackCommand AttackCommand;
    
        [BoxGroup("Commands")]
        [PropertyOrder(4)]
        [LabelWidth(100)]
        [SerializeReference]
        public MoveCommand MoveCommand;
    }
}