using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using System;
using System.Collections.Generic;
using static UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema;

public class AddressablesGroupManager : EditorWindow
{
    private int maxAssetsPerGroup = 5;
    private BundleNamingStyle bundleNamingStyle = BundleNamingStyle.OnlyHash;

    [MenuItem("Window/Addressables Group Manager")]
    public static void ShowWindow()
    {
        GetWindow<AddressablesGroupManager>("Addressables Group Manager");
    }

    private void OnGUI()
    {
        GUILayout.Label("Group Assets", EditorStyles.boldLabel);

        maxAssetsPerGroup = EditorGUILayout.IntField("Max Assets Per Group", maxAssetsPerGroup);
        bundleNamingStyle = (BundleNamingStyle)EditorGUILayout.EnumPopup("Bundle Naming Style", bundleNamingStyle);

        if (GUILayout.Button("Create Groups"))
        {
            CreateGroups();
        }
    }

    private void CreateGroups()
    {
        try
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

            // Loop through all groups in the Addressable Asset Settings
            foreach (var group in settings.groups)
            {
                if (group.entries.Count > maxAssetsPerGroup)
                {
                    int groupCount = 0;

                    // Find the number suffix of the latest group
                    foreach (var g in settings.groups)
                    {
                        if (g.name.Contains(group.name))
                        {
                            string[] splitName = g.name.Split('_');
                            if (splitName.Length == 2 && int.TryParse(splitName[1], out int number))
                            {
                                if (number > groupCount)
                                {
                                    groupCount = number;
                                }
                            }
                        }
                    }

                    AddressableAssetGroup newGroup = settings.CreateGroup(group.name + "_" + (groupCount + 1).ToString(), false, false, true, null, null);
                    newGroup.AddSchema<BundledAssetGroupSchema>();
                    newGroup.GetSchema<BundledAssetGroupSchema>().BundleNaming = bundleNamingStyle;

                    int assetCount = group.entries.Count;

                    // Loop through all assets in the group
                    List<AddressableAssetEntry> entries = new List<AddressableAssetEntry>(group.entries);
                    for (int i = maxAssetsPerGroup; i < assetCount; i++)
                    {
                        AddressableAssetEntry assetEntry = entries[i];
                        settings.MoveEntry(assetEntry, newGroup, false);
                    }
                }
            }

            Debug.Log("Group Assets: Groups created successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError("Group Assets: " + e.Message);
        }
    }
}