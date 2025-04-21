using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterBuffDataScriptable", menuName = "Scriptable Objects/CharacterBuffDataScriptable")]
public class CharacterBuffDataScriptable : SerializedScriptableObject
{
    public CharacterBuffData Data;
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (Data == null) return;
        Data.OnValidate();
    }
    #endif
}
