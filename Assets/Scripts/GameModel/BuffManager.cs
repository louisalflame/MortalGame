using System.Collections.Generic;
using UnityEngine;

public class BuffManager
{
    public List<BuffEntity> Buffs = new List<BuffEntity>();

    public BuffManager AddBuff(BuffEntity buff)
    { 
        Buffs.Add(buff);
        return this;
    }
}
