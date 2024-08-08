using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerScriptable", menuName = "Scriptable Objects/PlayerScriptable")]
public class PlayerScriptable : SerializedScriptableObject
{
    public PlayerData Player;
}
