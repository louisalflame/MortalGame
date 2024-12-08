using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "CardStatusScriptable", menuName = "Scriptable Objects/CardStatusScriptable")]
public class CardStatusScriptable : SerializedScriptableObject
{
    public CardStatusData Data;
}
