using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "AllCardStatusScriptable", menuName = "Scriptable Objects/AllCardStatusScriptable")]
public class AllCardStatusScriptable : SerializedScriptableObject
{
    public CardStatusScriptable[] AllCardStatusData;
}
