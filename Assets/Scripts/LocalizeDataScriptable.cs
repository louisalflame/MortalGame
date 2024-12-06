using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizeData", menuName = "Scriptable Objects/Localize Data")]
public class LocalizeDataScriptable : SerializedScriptableObject
{
    public LocalizeData Data;
}

public class LocalizeData : Dictionary<string, string>
{
}
