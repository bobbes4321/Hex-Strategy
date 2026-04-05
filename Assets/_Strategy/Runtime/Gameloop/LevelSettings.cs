using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Strategy.Runtime.Gameloop
{
    [CreateAssetMenu(menuName = "Strategy/Create LevelSettings", fileName = "LevelSettings", order = 0)]
    public class LevelSettings : Settings.Settings
    {
        [InlineEditor(InlineEditorModes.FullEditor, Expanded = true)]
        public List<LevelData> Levels;
    }
}