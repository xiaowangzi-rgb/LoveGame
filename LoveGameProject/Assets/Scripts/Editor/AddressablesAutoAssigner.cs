using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using System.IO;

/// <summary>
///  自动生成Addressables标记脚本
/// </summary>
public class AddressablesAutoAssigner : EditorWindow
{
    private const string BaseResourcesPath = "Assets/ResBase/BaseResources";
    private const string DownloadableResourcesPath = "Assets/ResBase/DownloadableResources";
    private const string BaseResourcesGroupName = "BaseResourcesGroup";
    private const string DownloadableResourcesGroupName = "DownloadableResourcesGroup";

    [MenuItem("Tools/Addressables/Auto Assign Addressables")]
    public static void AutoAssignAddressables()
    {
        // 检查 Addressables Settings 是否存在
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
        if (settings == null)
        {
            Debug.LogError("AddressableAssetSettings not found. Please ensure Addressables is set up in the project.");
            return;
        }

        // 获取或创建分组
        AddressableAssetGroup baseGroup = GetOrCreateGroup(settings, BaseResourcesGroupName);
        AddressableAssetGroup downloadableGroup = GetOrCreateGroup(settings, DownloadableResourcesGroupName);

        // 处理基础资源
        AssignFolderToGroup(BaseResourcesPath, baseGroup, settings);

        // 处理可下载资源
        AssignFolderToGroup(DownloadableResourcesPath, downloadableGroup, settings);

        Debug.Log("Addressables assignment completed!");
    }

    private static AddressableAssetGroup GetOrCreateGroup(AddressableAssetSettings settings, string groupName)
    {
        AddressableAssetGroup group = settings.FindGroup(groupName);
        if (group == null)
        {
            group = settings.CreateGroup(groupName, false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            Debug.Log($"Created new Addressable Group: {groupName}");
        }
        return group;
    }

    private static void AssignFolderToGroup(string folderPath, AddressableAssetGroup group, AddressableAssetSettings settings)
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning($"Folder not found: {folderPath}. Skipping.");
            return;
        }

        string[] assetPaths = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
        foreach (string assetPath in assetPaths)
        {
            if (assetPath.EndsWith(".meta")) continue; // 忽略 .meta 文件

            string relativePath = assetPath.Replace("\\", "/");
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(relativePath), group);

            // 设置资源的地址为相对于根文件夹的路径
            entry.address = relativePath.Substring(folderPath.Length + 1);
        }
    }
}