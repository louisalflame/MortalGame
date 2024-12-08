using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "AllBuffScriptable", menuName = "Scriptable Objects/AllBuffScriptable")]
public class AllBuffScriptable : SerializedScriptableObject
{
    public BuffDataScriptable[] AllBuffData;
}
