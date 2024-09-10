using Unity.VisualScripting;
using UnityEngine;

public class BuffEntity
{
    public string Id;
    
    public string Identity;

    public int Level;

    public PlayerEntity Owner;
    public PlayerEntity Caster;

    public BuffEntity(
        string id,
        string identity,
        int level,
        PlayerEntity owner,
        PlayerEntity caster) 
    {
        Id = id;
        Identity = identity;
        Level = level;
        Owner = owner;
        Caster = caster;
    }

    public BuffInfo ToInfo()
    {
        return new BuffInfo() {
            BuffId = Id,
            BuffIdentity = Identity,
            Level = Level
        };
    } 
}
