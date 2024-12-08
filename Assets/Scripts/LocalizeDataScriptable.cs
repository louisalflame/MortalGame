using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizeData", menuName = "Scriptable Objects/Localize Data")]
public class LocalizeDataScriptable : SerializedScriptableObject
{
    public Dictionary<string, string> Data;
}