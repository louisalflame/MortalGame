using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyData
{
    [BoxGroup("EnemyOnly")]
    public int Level;
    [BoxGroup("EnemyOnly")]
    public int EnergyRecoverPoint;

    public PlayerData PlayerData;
}
