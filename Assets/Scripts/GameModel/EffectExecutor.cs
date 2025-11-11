using System.Collections.Generic;
using System.Linq;
using Optional;
using UnityEngine;

public static class EffectExecutor
{
    public record EffectResult(
        List<IGameEvent> Events,
        List<BaseResultAction> ResultActions
    );

    private record EffectContext(
        IGameplayStatusWatcher GameplayWatcher,
        IGameplayReactor GameplayReactor,
        IActionSource ActionSource,
        ITriggerSource Trigger
    );

    #region CardEffect
    public static EffectResult ApplyCardEffect(
        IGameplayStatusWatcher gameplayWatcher,
        IGameplayReactor gameplayReactor,
        IActionSource actionSource,
        ITriggerSource trigger,
        ICardEffect cardEffect)
    {
        var context = new EffectContext(gameplayWatcher, gameplayReactor, actionSource, trigger);
        
        return cardEffect switch
        {
            // Damage Effects
            DamageEffect damageEffect =>
                ApplyDamageEffect(context, damageEffect.Targets, damageEffect.Value, DamageType.Normal,
                    GameFormula.NormalDamagePoint),

            PenetrateDamageEffect penetrateDamageEffect =>
                ApplyDamageEffect(context, penetrateDamageEffect.Targets, penetrateDamageEffect.Value, DamageType.Penetrate,
                    GameFormula.PenetrateDamagePoint),

            AdditionalAttackEffect additionalAttackEffect =>
                ApplyDamageEffect(context, additionalAttackEffect.Targets, additionalAttackEffect.Value, DamageType.Additional,
                    GameFormula.AdditionalDamagePoint),

            EffectiveAttackEffect effectiveAttackEffect =>
                ApplyDamageEffect(context, effectiveAttackEffect.Targets, effectiveAttackEffect.Value, DamageType.Effective,
                    GameFormula.EffectiveDamagePoint),

            // Character Health Effects
            HealEffect healEffect =>
                ApplyHealEffect(context, healEffect),

            ShieldEffect shieldEffect =>
                ApplyShieldEffect(context, shieldEffect),

            // Energy Effects
            GainEnergyEffect gainEnergyEffect =>
                ApplyGainEnergyEffect(context, gainEnergyEffect),

            LoseEnegyEffect loseEnergyEffect =>
                ApplyLoseEnergyEffect(context, loseEnergyEffect),

            // Disposition Effects
            IncreaseDispositionEffect increaseDispositionEffect =>
                ApplyIncreaseDispositionEffect(context, increaseDispositionEffect),

            DecreaseDispositionEffect decreaseDispositionEffect =>
                ApplyDecreaseDispositionEffect(context, decreaseDispositionEffect),

            // Player Buff Effects
            AddPlayerBuffEffect addBuffEffect =>
                ApplyAddPlayerBuffEffect(context, addBuffEffect),

            RemovePlayerBuffEffect removeBuffEffect =>
                ApplyRemovePlayerBuffEffect(context, removeBuffEffect),

            // Card Effects
            DrawCardEffect drawCardEffect =>
                ApplyDrawCardEffect(context, drawCardEffect),

            DiscardCardEffect discardCardEffect =>
                ApplyDiscardCardEffect(context, discardCardEffect),

            ConsumeCardEffect consumeCardEffect =>
                ApplyConsumeCardEffect(context, consumeCardEffect),

            DisposeCardEffect disposeCardEffect =>
                ApplyDisposeCardEffect(context, disposeCardEffect),

            CreateCardEffect createCardEffect =>
                ApplyCreateCardEffect(context, createCardEffect),

            CloneCardEffect cloneCardEffect =>
                ApplyCloneCardEffect(context, cloneCardEffect),

            AddCardBuffEffect addCardBuffEffect =>
                ApplyAddCardBuffEffect(context, addCardBuffEffect),

            _ => new EffectResult(new List<IGameEvent>(), new List<BaseResultAction>())
        };
    }

    #region Damage Effect Handlers
    private static EffectResult ApplyDamageEffect(
        EffectContext context,
        ITargetCharacterCollectionValue targets,
        IIntegerValue value,
        DamageType damageType,
        System.Func<IGameplayStatusWatcher, int, DamageIntentTargetAction, int> formulaFunc)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new DamageIntentAction(context.ActionSource, damageType);
        var targetEntities = targets.Eval(context.GameplayWatcher, context.Trigger, intent);

        foreach (var target in targetEntities)
        {
            var characterTarget = new CharacterTarget(target);
            var targetIntent = new DamageIntentTargetAction(context.ActionSource, characterTarget, damageType);
            var damagePoint = value.Eval(context.GameplayWatcher, context.Trigger, targetIntent);
            var damageFormulaPoint = formulaFunc(context.GameplayWatcher, damagePoint, targetIntent);

            var damageResult = target.HealthManager.TakeDamage(damageFormulaPoint, context.GameplayWatcher.ContextManager.Context, damageType);
            var damageStyle = DamageStyle.None; // TODO: pass style from action source

            var damageResultAction = new DamageResultAction(context.ActionSource, characterTarget, damageResult);
            var reactorEvents = context.GameplayReactor.UpdateReactorSessionAction(damageResultAction);

            resultActions.Add(damageResultAction);
            cardEffectEvents.AddRange(reactorEvents);
            cardEffectEvents.Add(new DamageEvent(target.Faction(context.GameplayWatcher), target, damageResult, damageStyle));
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }
    #endregion

    #region Character Health Effect Handlers
    private static EffectResult ApplyHealEffect(EffectContext context, HealEffect healEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new HealIntentAction(context.ActionSource);
        var targets = healEffect.Targets.Eval(context.GameplayWatcher, context.Trigger, intent);

        foreach (var target in targets)
        {
            var characterTarget = new CharacterTarget(target);
            var targetIntent = new HealIntentTargetAction(context.ActionSource, characterTarget);
            var healPoint = healEffect.Value.Eval(context.GameplayWatcher, context.Trigger, targetIntent);
            var healResult = target.HealthManager.GetHeal(healPoint, context.GameplayWatcher.ContextManager.Context);

            var healResultAction = new HealResultAction(context.ActionSource, characterTarget, healResult);
            var reactorEvents = context.GameplayReactor.UpdateReactorSessionAction(healResultAction);

            resultActions.Add(healResultAction);
            cardEffectEvents.AddRange(reactorEvents);
            cardEffectEvents.Add(new GetHealEvent(target.Faction(context.GameplayWatcher), target, healResult));
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }

    private static EffectResult ApplyShieldEffect(EffectContext context, ShieldEffect shieldEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new ShieldIntentAction(context.ActionSource);
        var targets = shieldEffect.Targets.Eval(context.GameplayWatcher, context.Trigger, intent);

        foreach (var target in targets)
        {
            var characterTarget = new CharacterTarget(target);
            var targetIntent = new ShieldIntentTargetAction(context.ActionSource, characterTarget);
            var shieldPoint = shieldEffect.Value.Eval(context.GameplayWatcher, context.Trigger, targetIntent);
            var shieldResult = target.HealthManager.GetShield(shieldPoint, context.GameplayWatcher.ContextManager.Context);

            var shieldResultAction = new ShieldResultAction(context.ActionSource, characterTarget, shieldResult);
            var reactorEvents = context.GameplayReactor.UpdateReactorSessionAction(shieldResultAction);

            resultActions.Add(shieldResultAction);
            cardEffectEvents.AddRange(reactorEvents);
            cardEffectEvents.Add(new GetShieldEvent(target.Faction(context.GameplayWatcher), target, shieldResult));
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }
    #endregion

    #region Energy Effect Handlers
    private static EffectResult ApplyGainEnergyEffect(EffectContext context, GainEnergyEffect gainEnergyEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new GainEnergyIntentAction(context.ActionSource);
        var targets = gainEnergyEffect.Targets.Eval(context.GameplayWatcher, context.Trigger, intent);

        foreach (var target in targets)
        {
            var playerTarget = new PlayerTarget(target);
            var targetIntent = new GainEnergyIntentTargetAction(context.ActionSource, playerTarget);
            var gainEnergy = gainEnergyEffect.Value.Eval(context.GameplayWatcher, context.Trigger, targetIntent);
            var gainEnergyResult = target.EnergyManager.GainEnergy(gainEnergy);

            var gainEnergyResultAction = new GainEnergyResultAction(context.ActionSource, playerTarget, gainEnergyResult);
            var reactorEvents = context.GameplayReactor.UpdateReactorSessionAction(gainEnergyResultAction);

            resultActions.Add(gainEnergyResultAction);
            cardEffectEvents.AddRange(reactorEvents);
            cardEffectEvents.Add(new GainEnergyEvent(target.Faction, target.EnergyManager.ToInfo(), gainEnergyResult));
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }

    private static EffectResult ApplyLoseEnergyEffect(EffectContext context, LoseEnegyEffect loseEnergyEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new LoseEnergyIntentAction(context.ActionSource);
        var targets = loseEnergyEffect.Targets.Eval(context.GameplayWatcher, context.Trigger, intent);

        foreach (var target in targets)
        {
            var playerTarget = new PlayerTarget(target);
            var targetIntent = new LoseEnergyIntentTargetAction(context.ActionSource, playerTarget);
            var loseEnergy = loseEnergyEffect.Value.Eval(context.GameplayWatcher, context.Trigger, targetIntent);
            var loseEnergyResult = target.EnergyManager.LoseEnergy(loseEnergy);

            var loseEnergyResultAction = new LoseEnergyResultAction(context.ActionSource, playerTarget, loseEnergyResult);
            var reactorEvents = context.GameplayReactor.UpdateReactorSessionAction(loseEnergyResultAction);

            resultActions.Add(loseEnergyResultAction);
            cardEffectEvents.AddRange(reactorEvents);
            cardEffectEvents.Add(new LoseEnergyEvent(target.Faction, target.EnergyManager.ToInfo(), loseEnergyResult));
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }
    #endregion

    #region Disposition Effect Handlers
    private static EffectResult ApplyIncreaseDispositionEffect(EffectContext context, IncreaseDispositionEffect increaseDispositionEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new IncreaseDispositionIntentAction(context.ActionSource);
        var targets = increaseDispositionEffect.Targets.Eval(context.GameplayWatcher, context.Trigger, intent);

        foreach (var target in targets)
        {
            if (target is AllyEntity ally)
            {
                var playerTarget = new PlayerTarget(ally);
                var targetIntent = new IncreaseDispositionIntentTargetAction(context.ActionSource, playerTarget);
                var increaseValue = increaseDispositionEffect.Value.Eval(context.GameplayWatcher, context.Trigger, targetIntent);
                var increaseResult = ally.DispositionManager.IncreaseDisposition(increaseValue);

                var increaseDispositionResultAction = new IncreaseDispositionResultAction(context.ActionSource, playerTarget, increaseResult);
                var reactorEvents = context.GameplayReactor.UpdateReactorSessionAction(increaseDispositionResultAction);

                resultActions.Add(increaseDispositionResultAction);
                cardEffectEvents.AddRange(reactorEvents);
                cardEffectEvents.Add(new IncreaseDispositionEvent(ally.DispositionManager.ToInfo(), increaseResult.DeltaDisposition));
            }
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }
    private static EffectResult ApplyDecreaseDispositionEffect(EffectContext context, DecreaseDispositionEffect decreaseDispositionEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new DecreaseDispositionIntentAction(context.ActionSource);
        var targets = decreaseDispositionEffect.Targets.Eval(context.GameplayWatcher, context.Trigger, intent);

        foreach (var target in targets)
        {
            if (target is AllyEntity ally)
            {
                var playerTarget = new PlayerTarget(ally);
                var targetIntent = new DecreaseDispositionIntentTargetAction(context.ActionSource, playerTarget);
                var decreaseValue = decreaseDispositionEffect.Value.Eval(context.GameplayWatcher, context.Trigger, targetIntent);
                var decreaseResult = ally.DispositionManager.DecreaseDisposition(decreaseValue);

                var decreaseDispositionResultAction = new DecreaseDispositionResultAction(context.ActionSource, playerTarget, decreaseResult);
                var reactorEvents = context.GameplayReactor.UpdateReactorSessionAction(decreaseDispositionResultAction);

                resultActions.Add(decreaseDispositionResultAction);
                cardEffectEvents.AddRange(reactorEvents);
                cardEffectEvents.Add(new DecreaseDispositionEvent(ally.DispositionManager.ToInfo(), decreaseResult.DeltaDisposition));
            }
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }
    #endregion

    #region Player Buff Effect Handlers
    private static EffectResult ApplyAddPlayerBuffEffect(EffectContext context, AddPlayerBuffEffect addBuffEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new AddPlayerBuffIntentAction(context.ActionSource);
        var targets = addBuffEffect.Targets.Eval(context.GameplayWatcher, context.Trigger, intent);

        foreach (var target in targets)
        {
            var playerTarget = new PlayerTarget(target);
            var targetIntent = new AddPlayerBuffIntentTargetAction(context.ActionSource, playerTarget);
            var level = addBuffEffect.Level.Eval(context.GameplayWatcher, context.Trigger, targetIntent);
            var addResult = target.BuffManager.AddBuff(
                context.GameplayWatcher.ContextManager.BuffLibrary,
                context.GameplayWatcher,
                context.Trigger,
                targetIntent,
                addBuffEffect.BuffId,
                level);

            var addPlayerBuffResultAction = new AddPlayerBuffResultAction(context.ActionSource, playerTarget, addResult);
            var reactorEvents = context.GameplayReactor.UpdateReactorSessionAction(addPlayerBuffResultAction);

            resultActions.Add(addPlayerBuffResultAction);
            cardEffectEvents.AddRange(reactorEvents);
            
            IGameEvent buffEvent = addResult.IsNewBuff ?
                new AddPlayerBuffEvent(target, addResult.Buff.ToInfo(context.GameplayWatcher)) :
                new GeneralUpdateEvent(addResult.Buff.ToInfo(context.GameplayWatcher));
            cardEffectEvents.Add(buffEvent);
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }

    private static EffectResult ApplyRemovePlayerBuffEffect(EffectContext context, RemovePlayerBuffEffect removeBuffEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new RemovePlayerBuffIntentAction(context.ActionSource);
        var targets = removeBuffEffect.Targets.Eval(context.GameplayWatcher, context.Trigger, intent);

        foreach (var target in targets)
        {
            var playerTarget = new PlayerTarget(target);
            var targetIntent = new RemovePlayerBuffIntentTargetAction(context.ActionSource, playerTarget);
            var removeResult = target.BuffManager.RemoveBuff(
                context.GameplayWatcher.ContextManager.BuffLibrary,
                context.GameplayWatcher,
                targetIntent,
                removeBuffEffect.BuffId);

            if (removeResult.Buffs.Count > 0)
            {
                var removePlayerBuffResultAction = new RemovePlayerBuffResultAction(context.ActionSource, playerTarget, removeResult);
                var reactorEvents = context.GameplayReactor.UpdateReactorSessionAction(removePlayerBuffResultAction);

                resultActions.Add(removePlayerBuffResultAction);
                cardEffectEvents.AddRange(reactorEvents);
                foreach (var resultBuff in removeResult.Buffs)
                    cardEffectEvents.Add(new RemovePlayerBuffEvent(target, resultBuff.ToInfo(context.GameplayWatcher)));
            }
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }
    #endregion

    #region Card Effect Handlers
    private static EffectResult ApplyDrawCardEffect(EffectContext context, DrawCardEffect drawCardEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new DrawCardIntentAction(context.ActionSource);
        var targets = drawCardEffect.Targets.Eval(context.GameplayWatcher, context.Trigger, intent);

        foreach (var target in targets)
        {
            var playerTarget = new PlayerTarget(target);
            var targetIntent = new DrawCardIntentTargetAction(context.ActionSource, playerTarget);
            var drawCount = drawCardEffect.Value.Eval(context.GameplayWatcher, context.Trigger, targetIntent);
            var drawResult = DrawCards(context.GameplayWatcher, context.GameplayReactor, context.ActionSource, target, drawCount);
            
            cardEffectEvents.AddRange(drawResult.Events);
            resultActions.AddRange(drawResult.ResultActions);
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }

    private static EffectResult ApplyDiscardCardEffect(EffectContext context, DiscardCardEffect discardCardEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new DiscardCardIntentAction(context.ActionSource);
        var cards = discardCardEffect.TargetCards.Eval(context.GameplayWatcher, context.Trigger, intent).ToList();

        foreach (var card in cards)
        {
            card.Owner(context.GameplayWatcher).MatchSome(cardOwner =>
            {
                if (cardOwner.CardManager.TryDiscardCard(
                    card.Identity, out var discardedCard, out var start, out var destination))
                {
                    var cardTarget = new CardTarget(card);
                    var discardCardResultAction = new DiscardCardResultAction(context.ActionSource, cardTarget, discardedCard);
                    var reactorEvents = context.GameplayReactor.UpdateReactorSessionAction(discardCardResultAction);

                    resultActions.Add(discardCardResultAction);
                    cardEffectEvents.AddRange(reactorEvents);
                    cardEffectEvents.Add(new MoveCardEvent(discardedCard, context.GameplayWatcher, start, destination));
                }
            });
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }

    private static EffectResult ApplyConsumeCardEffect(EffectContext context, ConsumeCardEffect consumeCardEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new ConsumeCardIntentAction(context.ActionSource);
        var cards = consumeCardEffect.TargetCards.Eval(context.GameplayWatcher, context.Trigger, intent).ToList();

        foreach (var card in cards)
        {
            card.Owner(context.GameplayWatcher).MatchSome(cardOwner =>
            {
                if (cardOwner.CardManager.TryConsumeCard(
                    card.Identity, out var consumedCard, out var start, out var destination))
                {
                    var cardTarget = new CardTarget(card);
                    var consumeCardResultAction = new ConsumeCardResultAction(context.ActionSource, cardTarget, consumedCard);
                    var reactorEvents = context.GameplayReactor.UpdateReactorSessionAction(consumeCardResultAction);

                    resultActions.Add(consumeCardResultAction);
                    cardEffectEvents.AddRange(reactorEvents);
                    cardEffectEvents.Add(new MoveCardEvent(consumedCard, context.GameplayWatcher, start, destination));
                }
            });
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }

    private static EffectResult ApplyDisposeCardEffect(EffectContext context, DisposeCardEffect disposeCardEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new DisposeCardIntentAction(context.ActionSource);
        var cards = disposeCardEffect.TargetCards.Eval(context.GameplayWatcher, context.Trigger, intent).ToList();

        foreach (var card in cards)
        {
            card.Owner(context.GameplayWatcher).MatchSome(cardOwner =>
            {
                if (cardOwner.CardManager.TryDisposeCard(
                    card.Identity, out var disposedCard, out var start, out var destination))
                {
                    var cardTarget = new CardTarget(card);
                    var disposeCardResultAction = new DisposeCardResultAction(context.ActionSource, cardTarget, disposedCard);
                    var reactorEvents = context.GameplayReactor.UpdateReactorSessionAction(disposeCardResultAction);

                    resultActions.Add(disposeCardResultAction);
                    cardEffectEvents.AddRange(reactorEvents);
                    cardEffectEvents.Add(new MoveCardEvent(disposedCard, context.GameplayWatcher, start, destination));
                }
            });
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }

    private static EffectResult ApplyCreateCardEffect(EffectContext context, CreateCardEffect createCardEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new CreateCardIntentAction(context.ActionSource);
        var target = createCardEffect.Target.Eval(context.GameplayWatcher, context.Trigger, intent);
        
        target.MatchSome(targetPlayer =>
        {
            foreach (var cardData in createCardEffect.CardDatas)
            {
                var playerTarget = new PlayerTarget(targetPlayer);
                var targetIntent = new CreateCardIntentTargetAction(context.ActionSource, playerTarget);

                var createResult = targetPlayer
                    .CardManager.CreateNewCard(
                        context.GameplayWatcher,
                        context.Trigger,
                        targetIntent,
                        cardData.Data,
                        createCardEffect.CreateDestination,
                        context.GameplayWatcher.ContextManager.CardBuffLibrary,
                        createCardEffect.AddCardBuffDatas);

                var createCardResultAction = new CreateCardResultAction(context.ActionSource, playerTarget, createResult);
                var reactorEvents = context.GameplayReactor.UpdateReactorSessionAction(createCardResultAction);

                resultActions.Add(createCardResultAction);
                cardEffectEvents.AddRange(reactorEvents);
                cardEffectEvents.Add(new AddCardEvent(Option.None<ICardEntity>(), createResult.Card, context.GameplayWatcher, createResult.Zone));
            }
        });

        return new EffectResult(cardEffectEvents, resultActions);
    }

    private static EffectResult ApplyCloneCardEffect(EffectContext context, CloneCardEffect cloneCardEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new CloneCardIntentAction(context.ActionSource);
        var target = cloneCardEffect.Target.Eval(context.GameplayWatcher, context.Trigger, intent);
        
        target.MatchSome(targetPlayer =>
        {
            var playerTarget = new PlayerTarget(targetPlayer);
            var targetIntent = new CloneCardIntentTargetAction(context.ActionSource, playerTarget);
            var cards = cloneCardEffect.ClonedCards.Eval(context.GameplayWatcher, context.Trigger, intent);
            
            foreach (var card in cards)
            {
                var playerCardTarget = new PlayerAndCardTarget(targetPlayer, card);
                targetIntent = new CloneCardIntentTargetAction(context.ActionSource, playerCardTarget);
                var cloneResult = targetPlayer
                    .CardManager.CloneNewCard(
                        context.GameplayWatcher,
                        context.Trigger,
                        targetIntent,
                        card,
                        cloneCardEffect.CloneDestination,
                        context.GameplayWatcher.ContextManager.CardBuffLibrary,
                        cloneCardEffect.AddCardBuffDatas);

                var cloneCardResultAction = new CloneCardResultAction(context.ActionSource, playerCardTarget, cloneResult);
                var reactorEvents = context.GameplayReactor.UpdateReactorSessionAction(cloneCardResultAction);

                resultActions.Add(cloneCardResultAction);
                cardEffectEvents.AddRange(reactorEvents);
                cardEffectEvents.Add(new AddCardEvent(Option.Some(cloneResult.OriginCard), cloneResult.Card, context.GameplayWatcher, cloneResult.Zone));
            }
        });

        return new EffectResult(cardEffectEvents, resultActions);
    }

    private static EffectResult ApplyAddCardBuffEffect(EffectContext context, AddCardBuffEffect addCardBuffEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new AddCardBuffIntentAction(context.ActionSource);
        var cards = addCardBuffEffect.TargetCards.Eval(context.GameplayWatcher, context.Trigger, intent).ToList();

        foreach (var card in cards)
        {
            foreach (var addCardBuff in addCardBuffEffect.AddCardBuffDatas)
            {
                var cardTarget = new CardTarget(card);
                var targetIntent = new AddCardBuffIntentTargetAction(context.ActionSource, cardTarget);
                var addLevel = addCardBuff.Level.Eval(context.GameplayWatcher, context.Trigger, targetIntent);
                var addBuffResult = card.BuffManager.AddBuff(
                    context.GameplayWatcher.ContextManager.CardBuffLibrary,
                    context.GameplayWatcher,
                    context.Trigger,
                    targetIntent,
                    addCardBuff.CardBuffId,
                    addLevel);

                var addCardBuffResultAction = new AddCardBuffResultAction(context.ActionSource, cardTarget, addBuffResult);
                var reactorEvents = context.GameplayReactor.UpdateReactorSessionAction(addCardBuffResultAction);

                resultActions.Add(addCardBuffResultAction);
                cardEffectEvents.AddRange(reactorEvents);
                cardEffectEvents.Add(new AddCardBuffEvent(card, context.GameplayWatcher));
            }
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }
    #endregion
    #endregion

    #region PlayBuffEffect
    public static EffectResult ApplyPlayerBuffEffect(
        IGameplayStatusWatcher gameplayWatcher,
        IGameplayReactor gameplayReactor,
        IActionSource actionSource,
        ITriggerSource triggerSource,
        IPlayerBuffEffect buffEffect)
    {
        var appleBuffEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        
        var updateTimingAction = new UpdateTimingAction(GameTiming.TriggerBuffStart, actionSource);
        appleBuffEffectEvents.AddRange(
            gameplayReactor.UpdateReactorSessionAction(updateTimingAction));

        switch (buffEffect)
        {
            case AdditionalDamagePlayerBuffEffect additionalDamageBuffEffect:
            {
                var intent = new DamageIntentAction(actionSource, DamageType.Additional);
                var targets = additionalDamageBuffEffect.Targets.Eval(gameplayWatcher, triggerSource, intent);
                foreach (var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var targetIntent = new DamageIntentTargetAction(actionSource, characterTarget, DamageType.Additional);
                    var damagePoint = additionalDamageBuffEffect.Value.Eval(gameplayWatcher, triggerSource, targetIntent);
                    var damageFormulaPoint = GameFormula.AdditionalDamagePoint(gameplayWatcher, damagePoint, targetIntent);

                    var damageResult = target.HealthManager.TakeDamage(damageFormulaPoint, gameplayWatcher.ContextManager.Context, DamageType.Additional);
                    var damageStyle = DamageStyle.None;

                    appleBuffEffectEvents.AddRange(
                        gameplayReactor.UpdateReactorSessionAction(
                            new DamageResultAction(actionSource, characterTarget, damageResult)));
                    appleBuffEffectEvents.Add(new DamageEvent(target.Faction(gameplayWatcher), target, damageResult, damageStyle));
                }
                break;
            }
            case EffectiveDamagePlayerBuffEffect effectiveDamageBuffEffect:
            {
                var intent = new DamageIntentAction(actionSource, DamageType.Effective);
                var targets = effectiveDamageBuffEffect.Targets.Eval(gameplayWatcher, triggerSource, intent);
                foreach (var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var targetIntent = new DamageIntentTargetAction(actionSource, characterTarget, DamageType.Effective);
                    var damagePoint = effectiveDamageBuffEffect.Value.Eval(gameplayWatcher, triggerSource, targetIntent);
                    var damageFormulaPoint = GameFormula.EffectiveDamagePoint(gameplayWatcher, damagePoint, targetIntent);

                    var damageResult = target.HealthManager.TakeDamage(damageFormulaPoint, gameplayWatcher.ContextManager.Context, DamageType.Effective);
                    var damageStyle = DamageStyle.None;

                    appleBuffEffectEvents.AddRange(
                        gameplayReactor.UpdateReactorSessionAction(
                            new DamageResultAction(actionSource, characterTarget, damageResult)));
                    appleBuffEffectEvents.Add(new DamageEvent(target.Faction(gameplayWatcher), target, damageResult, damageStyle));
                }
                break;
            }
            case AddCardBuffPlayerBuffEffect addCardBuffPlayerBuffEffect:
            {
                var intent = new AddCardBuffIntentAction(actionSource);
                var cards = addCardBuffPlayerBuffEffect.Targets.Eval(gameplayWatcher, triggerSource, intent);
                foreach (var card in cards)
                {
                    var cardTarget = new CardTarget(card);
                    var targetIntent = new AddCardBuffIntentTargetAction(actionSource, cardTarget);
                    foreach (var addCardBuffData in addCardBuffPlayerBuffEffect.AddCardBuffDatas)
                    {
                        var addLevel = addCardBuffData.Level.Eval(gameplayWatcher, triggerSource, targetIntent);
                        var addResult = card.BuffManager.AddBuff(
                            gameplayWatcher.ContextManager.CardBuffLibrary,
                            gameplayWatcher,
                            triggerSource,
                            targetIntent,
                            addCardBuffData.CardBuffId,
                            addLevel);

                        appleBuffEffectEvents.AddRange(
                            gameplayReactor.UpdateReactorSessionAction(
                                new AddCardBuffResultAction(actionSource, cardTarget, addResult)));
                        appleBuffEffectEvents.Add(new AddCardBuffEvent(card, gameplayWatcher));
                    }
                }
                break;
            }
            case CardPlayEffectAttributeAdditionPlayerBuffEffect cardPlayEffectAttributeBuffEffect:
            {
                if (actionSource is CardPlaySource cardPlaySource)
                {
                    var intent = new CardPlayEffectAttributeIntentAction(actionSource);
                    var cardPlayAttribute = cardPlaySource.Attribute;
                    var value = cardPlayEffectAttributeBuffEffect.Value.Eval(gameplayWatcher, triggerSource, intent);

                    cardPlayAttribute.ApplyModify(cardPlayEffectAttributeBuffEffect.Type, value);
                }
                break;
            }
        }

        return new EffectResult(appleBuffEffectEvents, resultActions);
    }
    #endregion

    public static EffectResult DrawCards(
        IGameplayStatusWatcher gameplayWatcher,
        IGameplayReactor gameplayReactor,
        IActionSource source,
        IPlayerEntity player,
        int drawCount)
    {
        var drawCardEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        
        for (int i = 0; i < drawCount; i++)
        {
            if (player.CardManager.Deck.Cards.Count == 0 &&
                player.CardManager.Graveyard.Cards.Count > 0)
            {
                var graveyardCards = player.CardManager.Graveyard.PopAllCards();
                player.CardManager.Deck.EnqueueCardsThenShuffle(graveyardCards);

                var recycleDeckResultAction = new RecycleDeckResultAction(new PlayerTarget(player));
                var recycleEvents = gameplayReactor.UpdateReactorSessionAction(recycleDeckResultAction);

                resultActions.Add(recycleDeckResultAction);
                drawCardEvents.AddRange(recycleEvents);
                drawCardEvents.Add(new RecycleGraveyardEvent(
                    Faction: player.Faction,
                    CardManagerInfo: player.CardManager.ToInfo(gameplayWatcher)
                ));
            }

            if (player.CardManager.Deck.Cards.Count > 0)
            {
                var drawResult = _DrawCard(gameplayWatcher, gameplayReactor, source, player);
                drawCardEvents.AddRange(drawResult.Events);
                resultActions.AddRange(drawResult.ResultActions);
            }
        }

        return new EffectResult(drawCardEvents, resultActions);
    }
    
    private static EffectResult _DrawCard(
        IGameplayStatusWatcher gameplayWatcher,
        IGameplayReactor gameplayReactor,
        IActionSource source,
        IPlayerEntity player)
    {
        var drawCardEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        
        if (player.CardManager.Deck.PopCard(out ICardEntity newCard))
        {
            player.CardManager.HandCard.AddCard(newCard);

            var drawCardResultAction = new DrawCardResultAction(source, new PlayerTarget(player), newCard);
            var reactorEvents = gameplayReactor.UpdateReactorSessionAction(drawCardResultAction);

            resultActions.Add(drawCardResultAction);
            drawCardEvents.AddRange(reactorEvents);

            drawCardEvents.Add(new DrawCardEvent(
                Faction: player.Faction,
                NewCardInfo: newCard.ToInfo(gameplayWatcher),
                CardManagerInfo: player.CardManager.ToInfo(gameplayWatcher)
            ));
        }
        
        return new EffectResult(drawCardEvents, resultActions);
    }
}
