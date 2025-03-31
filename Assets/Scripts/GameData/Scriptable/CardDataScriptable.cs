using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDataScriptable", menuName = "Scriptable Objects/CardDataScriptable")]
public class CardDataScriptable : SerializedScriptableObject
{
    public CardData Data;

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (Data == null) return;
        Data.OnValidate();
    }
    #endif
}