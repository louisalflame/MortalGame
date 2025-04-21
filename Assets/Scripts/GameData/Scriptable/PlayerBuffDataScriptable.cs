using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBuffDataScriptable", menuName = "Scriptable Objects/PlayerBuffDataScriptable")]
public class PlayerBuffDataScriptable : SerializedScriptableObject
{
    public PlayerBuffData Data;
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (Data == null) return;
        Data.OnValidate();
    }
    #endif
}
