using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public static class DropdownHelper
{
    const string BuffFolderPath = "Assets/ScriptableObjects/Buff";
    const string CardStatusFolderPath = "Assets/ScriptableObjects/CardStatus";
    const string AssetExtension = "*.asset";

    public static IEnumerable<ValueDropdownItem> BuffNames
    {
        get
        {
            if (Directory.Exists(BuffFolderPath))
            {
                var assetPaths = Directory.GetFiles(BuffFolderPath, AssetExtension);
                foreach (var assetPath in assetPaths)
                {
                    var asset = AssetDatabase.LoadAssetAtPath<PlayerBuffDataScriptable>(assetPath);
                    if (asset != null && string.IsNullOrEmpty(asset.Data.ID) == false)
                    {
                        yield return new ValueDropdownItem(asset.Data.ID, asset.Data.ID);
                    }
                }
            }  
        }
    }

    public static IEnumerable<ValueDropdownItem> CardStatusNames
    {
        get
        {
            if (Directory.Exists(CardStatusFolderPath))
            {
                var assetPaths = Directory.GetFiles(CardStatusFolderPath, AssetExtension);
                foreach (var assetPath in assetPaths)
                {
                    var asset = AssetDatabase.LoadAssetAtPath<CardStatusScriptable>(assetPath);
                    if (asset != null && string.IsNullOrEmpty(asset.Data.ID) == false)
                    {
                        yield return new ValueDropdownItem(asset.Data.ID, asset.Data.ID);
                    }
                }
            }  
        }
    }
}