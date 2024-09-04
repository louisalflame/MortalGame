using UnityEngine;

public class BuffEntity
{
    public string Id;

    public int Level;

    public PlayerEntity Owner;
    public PlayerEntity Caster;

    public BuffEntity(
        string id,
        int level,
        PlayerEntity owner,
        PlayerEntity caster) 
    {
    }
}
