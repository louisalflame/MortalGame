using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

public interface ICardPlayResultValueCondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, CardPlayResultSource cardPlayResultSource);
}

[Serializable]
public class CardPlayEffectResultCondition : ICardPlayResultValueCondition
{
    [ShowInInspector]
    [HorizontalGroup("1")]
    public List<IEffectResultCondition> Conditions = new();

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, CardPlayResultSource cardPlayResultSource)
    {
        return cardPlayResultSource.EffectResults.All(effectResult => Conditions.All(c => c.Eval(gameWatcher, source, effectResult)));
    }
}

public interface IEffectResultCondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IEffectResultAction effectResult);
}

[Serializable]
public class EffectResultTypeCondition : IEffectResultCondition
{
    public enum EffectType
    {
        Damage,
        Heal,
        DrawCard,
        Buff,    
        Energy
    }

    [ShowInInspector]
    [HorizontalGroup("1")]
    public EffectType Type;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IEffectResultAction effectResult)
    {
        return Type switch
        {
            EffectType.Damage => effectResult is DamageResultAction,
            EffectType.Heal => effectResult is HealResultAction,
            EffectType.DrawCard => effectResult is DrawCardResultAction,
            EffectType.Buff => effectResult is AddPlayerBuffResultAction or
                RemovePlayerBuffResultAction or
                AddCardBuffResultAction or
                RemoveCardBuffResultAction,
            EffectType.Energy => effectResult is GainEnergyResultAction or LoseEnergyResultAction,
            _ => false
        };
    }
}
