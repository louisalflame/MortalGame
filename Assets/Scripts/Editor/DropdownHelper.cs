using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public static class DropdownHelper
{
    const string BuffFolderPath = "Assets/ScriptableObjects/Buff";
    const string BuffExtension = "*.asset";

    public static IEnumerable<ValueDropdownItem> BuffNames
    {
        get
        {
            if (Directory.Exists(BuffFolderPath))
            {
                var assetPaths = Directory.GetFiles(BuffFolderPath, BuffExtension);
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
}