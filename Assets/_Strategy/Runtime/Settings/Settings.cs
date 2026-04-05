using Sirenix.OdinInspector;

namespace _Strategy.Runtime.Settings
{
    public class Settings : SerializedScriptableObject
    {
        [HideInInlineEditors, HideInTables]
        public SettingsType Type;
    }

    [EnumToggleButtons]
    public enum SettingsType { Enemy, Player, Cards, Level }
}