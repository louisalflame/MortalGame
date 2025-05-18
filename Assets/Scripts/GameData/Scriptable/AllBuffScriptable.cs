using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "AllBuffScriptable", menuName = "Scriptable Objects/AllBuffScriptable")]
public class AllBuffScriptable : SerializedScriptableObject
{
    public PlayerBuffDataScriptable[] AllBuffData = new PlayerBuffDataScriptable[0];
}
