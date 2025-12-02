using System.Collections.Generic;
using System.Linq;
using Optional;
using Sirenix.Windows.Beta;
using UnityEngine;

public static class EffectExecutor
{
    public record EffectResult(
        List<IGameEvent> Events,
        List<BaseResultAction> ResultActions
    );

    private record EffectContext(
        IGameplayModel Model,
        IActionSource Action,
        ITriggeredSource Triggered
    );

    #region CardEffect
    public static EffectResult ApplyCardEffect(
        TriggerContext context,
        ICardEffect cardEffect)
    {
        
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
        TriggerContext context,
        ITargetCharacterCollectionValue targets,
        IIntegerValue value,
        DamageType damageType,
        System.Func<TriggerContext, int, int> formulaFunc)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new DamageIntentAction(context.Action.Source, damageType);
        var triggerContext = context with { Action = intent };
        var targetEntities = targets.Eval(triggerContext);

        foreach (var target in targetEntities)
        {
            var characterTarget = new CharacterTarget(target);
            var targetIntent = new DamageIntentTargetAction(context.Action.Source, characterTarget, damageType);
            var targetTriggerContext = triggerContext with { Action = targetIntent };

            var damagePoint = value.Eval(targetTriggerContext);
            var damageFormulaPoint = formulaFunc(targetTriggerContext, damagePoint);

            var damageResult = target.HealthManager.TakeDamage(damageFormulaPoint, context.Model.ContextManager.Context, damageType);
            var damageStyle = DamageStyle.None; // TODO: pass style from action source

            var damageResultAction = new DamageResultAction(context.Action.Source, characterTarget, damageResult);
            var reactorEvents = context.Model.UpdateReactorSessionAction(damageResultAction);

            resultActions.Add(damageResultAction);
            cardEffectEvents.AddRange(reactorEvents);
            cardEffectEvents.Add(new DamageEvent(target.Faction(context.Model), target, damageResult, damageStyle));
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }
    #endregion

    #region Character Health Effect Handlers
    private static EffectResult ApplyHealEffect(TriggerContext context, HealEffect healEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new HealIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var targets = healEffect.Targets.Eval(triggerContext);

        foreach (var target in targets)
        {
            var characterTarget = new CharacterTarget(target);
            var targetIntent = new HealIntentTargetAction(context.Action.Source, characterTarget);
            var targetTriggerContext = triggerContext with { Action = targetIntent };

            var healPoint = healEffect.Value.Eval(targetTriggerContext);
            var healResult = target.HealthManager.GetHeal(healPoint, context.Model.ContextManager.Context);

            var healResultAction = new HealResultAction(context.Action.Source, characterTarget, healResult);
            var reactorEvents = context.Model.UpdateReactorSessionAction(healResultAction);

            resultActions.Add(healResultAction);
            cardEffectEvents.AddRange(reactorEvents);
            cardEffectEvents.Add(new GetHealEvent(target.Faction(context.Model), target, healResult));
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }

    private static EffectResult ApplyShieldEffect(TriggerContext context, ShieldEffect shieldEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new ShieldIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var targets = shieldEffect.Targets.Eval(triggerContext);

        foreach (var target in targets)
        {
            var characterTarget = new CharacterTarget(target);
            var targetIntent = new ShieldIntentTargetAction(context.Action.Source, characterTarget);
            var targetTriggerContext = triggerContext with { Action = targetIntent };
            
            var shieldPoint = shieldEffect.Value.Eval(targetTriggerContext);
            var shieldResult = target.HealthManager.GetShield(shieldPoint, context.Model.ContextManager.Context);

            var shieldResultAction = new ShieldResultAction(context.Action.Source, characterTarget, shieldResult);
            var reactorEvents = context.Model.UpdateReactorSessionAction(shieldResultAction);

            resultActions.Add(shieldResultAction);
            cardEffectEvents.AddRange(reactorEvents);
            cardEffectEvents.Add(new GetShieldEvent(target.Faction(context.Model), target, shieldResult));
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }
    #endregion

    #region Energy Effect Handlers
    private static EffectResult ApplyGainEnergyEffect(TriggerContext context, GainEnergyEffect gainEnergyEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new GainEnergyIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var targets = gainEnergyEffect.Targets.Eval(triggerContext);

        foreach (var target in targets)
        {
            var playerTarget = new PlayerTarget(target);
            var targetIntent = new GainEnergyIntentTargetAction(context.Action.Source, playerTarget);
            var targetTriggerContext = triggerContext with { Action = targetIntent };
            var gainEnergy = gainEnergyEffect.Value.Eval(targetTriggerContext);
            var gainEnergyResult = target.EnergyManager.GainEnergy(gainEnergy);

            var gainEnergyResultAction = new GainEnergyResultAction(context.Action.Source, playerTarget, gainEnergyResult);
            var reactorEvents = context.Model.UpdateReactorSessionAction(gainEnergyResultAction);

            resultActions.Add(gainEnergyResultAction);
            cardEffectEvents.AddRange(reactorEvents);
            cardEffectEvents.Add(new GainEnergyEvent(target.Faction, target.EnergyManager.ToInfo(), gainEnergyResult));
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }

    private static EffectResult ApplyLoseEnergyEffect(TriggerContext context, LoseEnegyEffect loseEnergyEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new LoseEnergyIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var targets = loseEnergyEffect.Targets.Eval(triggerContext);

        foreach (var target in targets)
        {
            var playerTarget = new PlayerTarget(target);
            var targetIntent = new LoseEnergyIntentTargetAction(context.Action.Source, playerTarget);
            var targetTriggerContext = triggerContext with { Action = targetIntent };
            var loseEnergy = loseEnergyEffect.Value.Eval(targetTriggerContext);
            var loseEnergyResult = target.EnergyManager.LoseEnergy(loseEnergy);

            var loseEnergyResultAction = new LoseEnergyResultAction(context.Action.Source, playerTarget, loseEnergyResult);
            var reactorEvents = context.Model.UpdateReactorSessionAction(loseEnergyResultAction);

            resultActions.Add(loseEnergyResultAction);
            cardEffectEvents.AddRange(reactorEvents);
            cardEffectEvents.Add(new LoseEnergyEvent(target.Faction, target.EnergyManager.ToInfo(), loseEnergyResult));
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }
    #endregion

    #region Disposition Effect Handlers
    private static EffectResult ApplyIncreaseDispositionEffect(TriggerContext context, IncreaseDispositionEffect increaseDispositionEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new IncreaseDispositionIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var targets = increaseDispositionEffect.Targets.Eval(triggerContext);

        foreach (var target in targets)
        {
            if (target is AllyEntity ally)
            {
                var playerTarget = new PlayerTarget(ally);
                var targetIntent = new IncreaseDispositionIntentTargetAction(context.Action.Source, playerTarget);
                var targetTriggerContext = triggerContext with { Action = targetIntent };
                var increaseValue = increaseDispositionEffect.Value.Eval(targetTriggerContext);
                var increaseResult = ally.DispositionManager.IncreaseDisposition(increaseValue);

                var increaseDispositionResultAction = new IncreaseDispositionResultAction(context.Action.Source, playerTarget, increaseResult);
                var reactorEvents = context.Model.UpdateReactorSessionAction(increaseDispositionResultAction);

                resultActions.Add(increaseDispositionResultAction);
                cardEffectEvents.AddRange(reactorEvents);
                cardEffectEvents.Add(new IncreaseDispositionEvent(ally.DispositionManager.ToInfo(), increaseResult.DeltaDisposition));
            }
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }
    private static EffectResult ApplyDecreaseDispositionEffect(TriggerContext context, DecreaseDispositionEffect decreaseDispositionEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new DecreaseDispositionIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var targets = decreaseDispositionEffect.Targets.Eval(triggerContext);

        foreach (var target in targets)
        {
            if (target is AllyEntity ally)
            {
                var playerTarget = new PlayerTarget(ally);
                var targetIntent = new DecreaseDispositionIntentTargetAction(context.Action.Source, playerTarget);
                var targetTriggerContext = triggerContext with { Action = targetIntent };
                var decreaseValue = decreaseDispositionEffect.Value.Eval(targetTriggerContext);
                var decreaseResult = ally.DispositionManager.DecreaseDisposition(decreaseValue);

                var decreaseDispositionResultAction = new DecreaseDispositionResultAction(context.Action.Source, playerTarget, decreaseResult);
                var reactorEvents = context.Model.UpdateReactorSessionAction(decreaseDispositionResultAction);

                resultActions.Add(decreaseDispositionResultAction);
                cardEffectEvents.AddRange(reactorEvents);
                cardEffectEvents.Add(new DecreaseDispositionEvent(ally.DispositionManager.ToInfo(), decreaseResult.DeltaDisposition));
            }
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }
    #endregion

    #region Player Buff Effect Handlers
    private static EffectResult ApplyAddPlayerBuffEffect(TriggerContext context, AddPlayerBuffEffect addBuffEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new AddPlayerBuffIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var targets = addBuffEffect.Targets.Eval(triggerContext);

        foreach (var target in targets)
        {
            var playerTarget = new PlayerTarget(target);
            var targetIntent = new AddPlayerBuffIntentTargetAction(context.Action.Source, playerTarget);
            var targetTriggerContext = triggerContext with { Action = targetIntent };
            var level = addBuffEffect.Level.Eval(targetTriggerContext);
            var addResult = target.BuffManager.AddBuff(
                targetTriggerContext,
                addBuffEffect.BuffId,
                level);

            var addPlayerBuffResultAction = new AddPlayerBuffResultAction(context.Action.Source, playerTarget, addResult);
            var reactorEvents = context.Model.UpdateReactorSessionAction(addPlayerBuffResultAction);

            resultActions.Add(addPlayerBuffResultAction);
            cardEffectEvents.AddRange(reactorEvents);
            
            IGameEvent buffEvent = addResult.IsNewBuff ?
                new AddPlayerBuffEvent(target, addResult.Buff.ToInfo(context.Model)) :
                new GeneralUpdateEvent(addResult.Buff.ToInfo(context.Model));
            cardEffectEvents.Add(buffEvent);
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }

    private static EffectResult ApplyRemovePlayerBuffEffect(TriggerContext context, RemovePlayerBuffEffect removeBuffEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new RemovePlayerBuffIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var targets = removeBuffEffect.Targets.Eval(triggerContext);

        foreach (var target in targets)
        {
            var playerTarget = new PlayerTarget(target);
            var targetIntent = new RemovePlayerBuffIntentTargetAction(context.Action.Source, playerTarget);
            var targetTriggerContext = triggerContext with { Action = targetIntent };
            var removeResult = target.BuffManager.RemoveBuff(
                targetTriggerContext,
                removeBuffEffect.BuffId);

            if (removeResult.Buffs.Count > 0)
            {
                var removePlayerBuffResultAction = new RemovePlayerBuffResultAction(context.Action.Source, playerTarget, removeResult);
                var reactorEvents = context.Model.UpdateReactorSessionAction(removePlayerBuffResultAction);

                resultActions.Add(removePlayerBuffResultAction);
                cardEffectEvents.AddRange(reactorEvents);
                foreach (var resultBuff in removeResult.Buffs)
                    cardEffectEvents.Add(new RemovePlayerBuffEvent(target, resultBuff.ToInfo(context.Model)));
            }
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }
    #endregion

    #region Card Effect Handlers
    private static EffectResult ApplyDrawCardEffect(TriggerContext context, DrawCardEffect drawCardEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new DrawCardIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var targets = drawCardEffect.Targets.Eval(triggerContext);

        foreach (var target in targets)
        {
            var playerTarget = new PlayerTarget(target);
            var targetIntent = new DrawCardIntentTargetAction(context.Action.Source, playerTarget);
            var targetTriggerContext = triggerContext with { Action = targetIntent };
            var drawCount = drawCardEffect.Value.Eval(targetTriggerContext);
            var drawResult = DrawCards(context.Model, context.Action.Source, target, drawCount);
            
            cardEffectEvents.AddRange(drawResult.Events);
            resultActions.AddRange(drawResult.ResultActions);
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }

    private static EffectResult ApplyDiscardCardEffect(TriggerContext context, DiscardCardEffect discardCardEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new DiscardCardIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var cards = discardCardEffect.TargetCards.Eval(triggerContext).ToList();

        foreach (var card in cards)
        {
            card.Owner(context.Model).MatchSome(cardOwner =>
            {
                if (cardOwner.CardManager.TryDiscardCard(
                    card.Identity, out var discardedCard, out var start, out var destination))
                {
                    var cardTarget = new CardTarget(card);
                    var discardCardResultAction = new DiscardCardResultAction(context.Action.Source, cardTarget, discardedCard);
                    var reactorEvents = context.Model.UpdateReactorSessionAction(discardCardResultAction);

                    resultActions.Add(discardCardResultAction);
                    cardEffectEvents.AddRange(reactorEvents);
                    cardEffectEvents.Add(new MoveCardEvent(discardedCard, context.Model, start, destination));
                }
            });
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }

    private static EffectResult ApplyConsumeCardEffect(TriggerContext context, ConsumeCardEffect consumeCardEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new ConsumeCardIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var cards = consumeCardEffect.TargetCards.Eval(triggerContext).ToList();

        foreach (var card in cards)
        {
            card.Owner(context.Model).MatchSome(cardOwner =>
            {
                if (cardOwner.CardManager.TryConsumeCard(
                    card.Identity, out var consumedCard, out var start, out var destination))
                {
                    var cardTarget = new CardTarget(card);
                    var consumeCardResultAction = new ConsumeCardResultAction(context.Action.Source, cardTarget, consumedCard);
                    var reactorEvents = context.Model.UpdateReactorSessionAction(consumeCardResultAction);

                    resultActions.Add(consumeCardResultAction);
                    cardEffectEvents.AddRange(reactorEvents);
                    cardEffectEvents.Add(new MoveCardEvent(consumedCard, context.Model, start, destination));
                }
            });
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }

    private static EffectResult ApplyDisposeCardEffect(TriggerContext context, DisposeCardEffect disposeCardEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new DisposeCardIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var cards = disposeCardEffect.TargetCards.Eval(triggerContext).ToList();

        foreach (var card in cards)
        {
            card.Owner(context.Model).MatchSome(cardOwner =>
            {
                if (cardOwner.CardManager.TryDisposeCard(
                    card.Identity, out var disposedCard, out var start, out var destination))
                {
                    var cardTarget = new CardTarget(card);
                    var disposeCardResultAction = new DisposeCardResultAction(context.Action.Source, cardTarget, disposedCard);
                    var reactorEvents = context.Model.UpdateReactorSessionAction(disposeCardResultAction);

                    resultActions.Add(disposeCardResultAction);
                    cardEffectEvents.AddRange(reactorEvents);
                    cardEffectEvents.Add(new MoveCardEvent(disposedCard, context.Model, start, destination));
                }
            });
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }

    private static EffectResult ApplyCreateCardEffect(TriggerContext context, CreateCardEffect createCardEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new CreateCardIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var target = createCardEffect.Target.Eval(triggerContext);
        
        target.MatchSome(targetPlayer =>
        {
            foreach (var cardDataId in createCardEffect.CardDataIds)
            {
                var playerTarget = new PlayerTarget(targetPlayer);
                var targetIntent = new CreateCardIntentTargetAction(context.Action.Source, playerTarget);
                var targetTriggerContext = triggerContext with { Action = targetIntent };

                var createResult = targetPlayer
                    .CardManager.CreateNewCard(
                        targetTriggerContext,
                        cardDataId,
                        createCardEffect.CreateDestination,
                        context.Model.ContextManager.CardLibrary,
                        context.Model.ContextManager.CardBuffLibrary,
                        createCardEffect.AddCardBuffDatas);

                var createCardResultAction = new CreateCardResultAction(context.Action.Source, playerTarget, createResult);
                var reactorEvents = context.Model.UpdateReactorSessionAction(createCardResultAction);

                resultActions.Add(createCardResultAction);
                cardEffectEvents.AddRange(reactorEvents);
                cardEffectEvents.Add(new AddCardEvent(Option.None<ICardEntity>(), createResult.Card, context.Model, createResult.Zone));
            }
        });

        return new EffectResult(cardEffectEvents, resultActions);
    }

    private static EffectResult ApplyCloneCardEffect(TriggerContext context, CloneCardEffect cloneCardEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new CloneCardIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var target = cloneCardEffect.Target.Eval(triggerContext);
        
        target.MatchSome(targetPlayer =>
        {
            var playerTarget = new PlayerTarget(targetPlayer);
            var targetIntent = new CloneCardIntentTargetAction(context.Action.Source, playerTarget);
            var targetTriggerContext = triggerContext with { Action = targetIntent };
            var cards = cloneCardEffect.ClonedCards.Eval(targetTriggerContext);
            
            foreach (var card in cards)
            {
                var playerCardTarget = new PlayerAndCardTarget(targetPlayer, card);
                targetIntent = new CloneCardIntentTargetAction(context.Action.Source, playerCardTarget);
                targetTriggerContext = triggerContext with { Action = targetIntent };

                var cloneResult = targetPlayer
                    .CardManager.CloneNewCard(
                        targetTriggerContext,
                        card,
                        cloneCardEffect.CloneDestination,
                        context.Model.ContextManager.CardBuffLibrary,
                        cloneCardEffect.AddCardBuffDatas);

                var cloneCardResultAction = new CloneCardResultAction(context.Action.Source, playerCardTarget, cloneResult);
                var reactorEvents = context.Model.UpdateReactorSessionAction(cloneCardResultAction);

                resultActions.Add(cloneCardResultAction);
                cardEffectEvents.AddRange(reactorEvents);
                cardEffectEvents.Add(new AddCardEvent(Option.Some(cloneResult.OriginCard), cloneResult.Card, context.Model, cloneResult.Zone));
            }
        });

        return new EffectResult(cardEffectEvents, resultActions);
    }

    private static EffectResult ApplyAddCardBuffEffect(TriggerContext context, AddCardBuffEffect addCardBuffEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        var intent = new AddCardBuffIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var cards = addCardBuffEffect.TargetCards.Eval(triggerContext).ToList();

        foreach (var card in cards)
        {
            foreach (var addCardBuff in addCardBuffEffect.AddCardBuffDatas)
            {
                var cardTarget = new CardTarget(card);
                var targetIntent = new AddCardBuffIntentTargetAction(context.Action.Source, cardTarget);
                var targetTriggerContext = triggerContext with { Action = targetIntent };
                var addLevel = addCardBuff.Level.Eval(targetTriggerContext);

                var addBuffResult = card.BuffManager.AddBuff(
                    context.Model.ContextManager.CardBuffLibrary,
                    targetTriggerContext,
                    addCardBuff.CardBuffId,
                    addLevel);

                var addCardBuffResultAction = new AddCardBuffResultAction(context.Action.Source, cardTarget, addBuffResult);
                var reactorEvents = context.Model.UpdateReactorSessionAction(addCardBuffResultAction);

                resultActions.Add(addCardBuffResultAction);
                cardEffectEvents.AddRange(reactorEvents);
                cardEffectEvents.Add(new AddCardBuffEvent(card, context.Model));
            }
        }

        return new EffectResult(cardEffectEvents, resultActions);
    }
    #endregion
    #endregion

    #region PlayBuffEffect
    public static EffectResult ApplyPlayerBuffEffect(
        TriggerContext context,
        IPlayerBuffEffect buffEffect)
    {
        var appleBuffEffectEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        
        var updateTimingAction = new UpdateTimingAction(GameTiming.TriggerBuffStart, context.Action.Source);
        appleBuffEffectEvents.AddRange(context.Model.UpdateReactorSessionAction(updateTimingAction));

        switch (buffEffect)
        {
            case AdditionalDamagePlayerBuffEffect additionalDamageBuffEffect:
            {
                var intent = new DamageIntentAction(context.Action.Source, DamageType.Additional);
                var triggerContext = context with { Action = intent };
                var targets = additionalDamageBuffEffect.Targets.Eval(triggerContext);
                foreach (var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var targetIntent = new DamageIntentTargetAction(context.Action.Source, characterTarget, DamageType.Additional);
                    var targetTriggerContext = triggerContext with { Action = targetIntent };
                    var damagePoint = additionalDamageBuffEffect.Value.Eval(targetTriggerContext);
                    var damageFormulaPoint = GameFormula.AdditionalDamagePoint(targetTriggerContext, damagePoint);

                    var damageResult = target.HealthManager.TakeDamage(damageFormulaPoint, context.Model.ContextManager.Context, DamageType.Additional);
                    var damageStyle = DamageStyle.None;

                    appleBuffEffectEvents.AddRange(
                        context.Model.UpdateReactorSessionAction(
                            new DamageResultAction(context.Action.Source, characterTarget, damageResult)));
                    appleBuffEffectEvents.Add(new DamageEvent(target.Faction(context.Model), target, damageResult, damageStyle));
                }
                break;
            }
            case EffectiveDamagePlayerBuffEffect effectiveDamageBuffEffect:
            {
                var intent = new DamageIntentAction(context.Action.Source, DamageType.Effective);
                var triggerContext = context with { Action = intent };
                var targets = effectiveDamageBuffEffect.Targets.Eval(triggerContext);
                foreach (var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var targetIntent = new DamageIntentTargetAction(context.Action.Source, characterTarget, DamageType.Effective);
                    var targetTriggerContext = triggerContext with { Action = targetIntent };
                    var damagePoint = effectiveDamageBuffEffect.Value.Eval(targetTriggerContext);
                    var damageFormulaPoint = GameFormula.EffectiveDamagePoint(targetTriggerContext, damagePoint);

                    var damageResult = target.HealthManager.TakeDamage(damageFormulaPoint, context.Model.ContextManager.Context, DamageType.Effective);
                    var damageStyle = DamageStyle.None;

                    appleBuffEffectEvents.AddRange(
                        context.Model.UpdateReactorSessionAction(
                            new DamageResultAction(context.Action.Source, characterTarget, damageResult)));
                    appleBuffEffectEvents.Add(new DamageEvent(target.Faction(context.Model), target, damageResult, damageStyle));
                }
                break;
            }
            case AddCardBuffPlayerBuffEffect addCardBuffPlayerBuffEffect:
            {
                var intent = new AddCardBuffIntentAction(context.Action.Source);
                var triggerContext = context with { Action = intent };
                var cards = addCardBuffPlayerBuffEffect.Targets.Eval(triggerContext);
                foreach (var card in cards)
                {
                    var cardTarget = new CardTarget(card);
                    var targetIntent = new AddCardBuffIntentTargetAction(context.Action.Source, cardTarget);
                    var targetTriggerContext = triggerContext with { Action = targetIntent };
                    foreach (var addCardBuffData in addCardBuffPlayerBuffEffect.AddCardBuffDatas)
                    {
                        var addLevel = addCardBuffData.Level.Eval(targetTriggerContext);
                        var addResult = card.BuffManager.AddBuff(
                            context.Model.ContextManager.CardBuffLibrary,
                            targetTriggerContext,
                            addCardBuffData.CardBuffId,
                            addLevel);

                        appleBuffEffectEvents.AddRange(
                            context.Model.UpdateReactorSessionAction(
                                new AddCardBuffResultAction(context.Action.Source, cardTarget, addResult)));
                        appleBuffEffectEvents.Add(new AddCardBuffEvent(card, context.Model));
                    }
                }
                break;
            }
            case CardPlayEffectAttributeAdditionPlayerBuffEffect cardPlayEffectAttributeBuffEffect:
            {
                if (context.Action.Source is CardPlaySource cardPlaySource)
                {
                    var intent = new CardPlayEffectAttributeIntentAction(context.Action.Source);
                    var triggerContext = context with { Action = intent };
                    var value = cardPlayEffectAttributeBuffEffect.Value.Eval(triggerContext);

                    var cardPlayAttribute = cardPlaySource.Attribute;
                    cardPlayAttribute.ApplyModify(cardPlayEffectAttributeBuffEffect.Type, value);
                }
                break;
            }
        }

        return new EffectResult(appleBuffEffectEvents, resultActions);
    }
    #endregion

    public static EffectResult DrawCards(
        IGameplayModel model,
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
                var recycleEvents = model.UpdateReactorSessionAction(recycleDeckResultAction);

                resultActions.Add(recycleDeckResultAction);
                drawCardEvents.AddRange(recycleEvents);
                drawCardEvents.Add(new RecycleGraveyardEvent(
                    Faction: player.Faction,
                    CardManagerInfo: player.CardManager.ToInfo(model)
                ));
            }

            if (player.CardManager.Deck.Cards.Count > 0)
            {
                var drawResult = _DrawCard(model, source, player);
                drawCardEvents.AddRange(drawResult.Events);
                resultActions.AddRange(drawResult.ResultActions);
            }
        }

        return new EffectResult(drawCardEvents, resultActions);
    }
    
    private static EffectResult _DrawCard(
        IGameplayModel model,
        IActionSource source,
        IPlayerEntity player)
    {
        var drawCardEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();
        
        if (player.CardManager.Deck.PopCard(out ICardEntity newCard))
        {
            player.CardManager.HandCard.AddCard(newCard);

            var drawCardResultAction = new DrawCardResultAction(source, new PlayerTarget(player), newCard);
            var reactorEvents = model.UpdateReactorSessionAction(drawCardResultAction);

            resultActions.Add(drawCardResultAction);
            drawCardEvents.AddRange(reactorEvents);

            drawCardEvents.Add(new DrawCardEvent(
                Faction: player.Faction,
                NewCardInfo: newCard.ToInfo(model),
                CardManagerInfo: player.CardManager.ToInfo(model)
            ));
        }
        
        return new EffectResult(drawCardEvents, resultActions);
    }
}
