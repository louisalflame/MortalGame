using System.Collections.Generic;
using UnityEngine;

public interface IBuffManager
{
    IReadOnlyCollection<BuffEntity> Buffs { get; }
    BuffEntity AddBuff(string buffId, int level);
}

public class BuffManager : IBuffManager
{
    private GameContextManager _gameContextManager;
    private List<BuffEntity> _buffs;

    public IReadOnlyCollection<BuffEntity> Buffs => _buffs;

    public BuffManager()
    {
        _buffs = new List<BuffEntity>();
    }

    public BuffEntity AddBuff(string buffId, int level)
    {
        var buff = new BuffEntity(buffId, level, _gameContextManager.Context.Caster, _gameContextManager.Context.EffectTarget);
        _buffs.Add(buff);
        return buff;
    }
}
