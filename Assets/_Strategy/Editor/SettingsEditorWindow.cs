using System;
using System.Collections.Generic;
using System.Linq;
using _Strategy.Runtime.Settings;
using Plugins.Editor.Extensions;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class SettingsEditorWindow : OdinMenuEditorWindow
{
    [MenuItem("Tools/Settings Manager")]
    private static void OpenWindow()
    {
        GetWindow<SettingsEditorWindow>().Show();
    }

    private CreateNewEnemySettings _createNewEnemySettings;
    private Dictionary<Type, SettingsOverview> _settingsOverviews;

    protected override void OnEnable()
    {
        base.OnEnable();
        _createNewEnemySettings = new CreateNewEnemySettings();
        _settingsOverviews = new Dictionary<Type, SettingsOverview>();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree(supportsMultiSelect: true)
        {
            { "Create New", _createNewEnemySettings, EditorIcons.Plus }
        };

        var settingsAssetsByType = GetSettingsAssetsByType();
        CreateSettingsOverview(settingsAssetsByType, tree);

        var settingsAssets = GetSettingsBySettingsType();
        AddSettingsToMenuTree(settingsAssets, tree);

        return tree;
    }

    private void AddSettingsToMenuTree(IOrderedEnumerable<IGrouping<SettingsType, Settings>> settingsAssets, OdinMenuTree tree)
    {
        foreach (var typeGroup in settingsAssets)
        {
            foreach (var setting in typeGroup.OrderBy(x => x.name))
            {
                var menuItems = tree.Add($"Settings/{typeGroup.Key}/{setting.name}", setting);
                foreach (var odinMenuItem in menuItems)
                {
                    AddDragHandles(odinMenuItem);
                }
            }
        }
    }

    private static IOrderedEnumerable<IGrouping<SettingsType, Settings>> GetSettingsBySettingsType()
    {
        return AssetDatabase.FindAssets($"t:{typeof(Settings).Name}")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Settings>)
            .Where(x => x != null)
            .GroupBy(x => x.Type)
            .OrderBy(group => group.Key);
    }

    private void CreateSettingsOverview(IEnumerable<IGrouping<Type, Settings>> settingsAssetsByType, OdinMenuTree tree)
    {
        foreach (var typeGroup in settingsAssetsByType)
        {
            var typeName = typeGroup.Key;

            // Create or get the overview for this type
            if (!_settingsOverviews.ContainsKey(typeName))
            {
                _settingsOverviews[typeName] = new SettingsOverview(typeName);
            }

            // Add the overview for this type
            tree.Add($"Settings Overview/{typeName.Name}", _settingsOverviews[typeName], EditorIcons.List);
        }
    }

    private static IEnumerable<IGrouping<Type, Settings>> GetSettingsAssetsByType()
    {
        return AssetDatabase.FindAssets($"t:{typeof(Settings).Name}")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Settings>)
            .Where(x => x != null)
            .GroupBy(x => x.GetType());
    }

    private void AddDragHandles(OdinMenuItem menuItem)
    {
        menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
    }

    protected override void OnBeginDrawEditors()
    {
        var selected = MenuTree.Selection.FirstOrDefault();
        var toolbarHeight = MenuTree.Config.SearchToolbarHeight;

        SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
        {
            if (selected != null)
            {
                GUILayout.Label(selected.SmartName, SirenixGUIStyles.BoldTitle);
            }

            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
            {
                ForceMenuTreeRebuild();
            }
        }
        SirenixEditorGUI.EndHorizontalToolbar();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (_createNewEnemySettings != null)
        {
            DestroyImmediate(_createNewEnemySettings.previewObject);
        }
    }
}

[Serializable]
public class SettingsOverview
{
    [SerializeField] [TableList(ShowIndexLabels = true, AlwaysExpanded = true, IsReadOnly = false)]
    private List<Settings> _settings;

    private Type _settingsType;

    public SettingsOverview(Type type)
    {
        _settingsType = type;

        RefreshList();
    }

    [Button("Refresh List", ButtonSizes.Medium)]
    [PropertyOrder(-1)]
    private void RefreshList()
    {
        _settings = AssetDatabase.FindAssets($"t:{typeof(Settings).Name}")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Settings>)
            .Where(x => x != null && x.GetType() == _settingsType)
            .OrderBy(x => x.name)
            .ToList();
    }
}