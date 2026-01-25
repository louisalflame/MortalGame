using System;
using System.Collections.Generic;
using System.Linq;
using Optional;

public record EffectCommandSet(
    IReadOnlyCollection<IEffectCommand> Commands);

public static class EffectExecutor
{
    #region CardEffect
    public static EffectCommandSet ResolveCardEffect(
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

            _ => new EffectCommandSet(Array.Empty<IEffectCommand>())
        };
    }

    #region Damage Effect Handlers
    private static EffectCommandSet ApplyDamageEffect(
        TriggerContext context,
        ITargetCharacterCollectionValue targets,
        IIntegerValue value,
        DamageType damageType,
        Func<TriggerContext, int, int> formulaFunc)
    {
        var effectCommands = new List<IEffectCommand>();
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

            effectCommands.Add(new DamageEffectCommand(target, damageFormulaPoint, damageType));
        }
        return new EffectCommandSet(effectCommands);
    }
    #endregion

    #region Character Health Effect Handlers
    private static EffectCommandSet ApplyHealEffect(TriggerContext context, HealEffect healEffect)
    {
        var effectCommands = new List<IEffectCommand>();
        var intent = new HealIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var targets = healEffect.Targets.Eval(triggerContext);

        foreach (var target in targets)
        {
            var characterTarget = new CharacterTarget(target);
            var targetIntent = new HealIntentTargetAction(context.Action.Source, characterTarget);
            var targetTriggerContext = triggerContext with { Action = targetIntent };

            var healPoint = healEffect.Value.Eval(targetTriggerContext);

            effectCommands.Add(new HealEffectCommand(target, healPoint));
        }
        return new EffectCommandSet(effectCommands);
    }

    private static EffectCommandSet ApplyShieldEffect(TriggerContext context, ShieldEffect shieldEffect)
    {
        var effectCommands = new List<IEffectCommand>();
        var intent = new ShieldIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var targets = shieldEffect.Targets.Eval(triggerContext);

        foreach (var target in targets)
        {
            var characterTarget = new CharacterTarget(target);
            var targetIntent = new ShieldIntentTargetAction(context.Action.Source, characterTarget);
            var targetTriggerContext = triggerContext with { Action = targetIntent };
            
            var shieldPoint = shieldEffect.Value.Eval(targetTriggerContext);

            effectCommands.Add(new ShieldEffectCommand(target, shieldPoint));
        }
        return new EffectCommandSet(effectCommands);
    }
    #endregion

    #region Energy Effect Handlers
    private static EffectCommandSet ApplyGainEnergyEffect(TriggerContext context, GainEnergyEffect gainEnergyEffect)
    {
        var effectCommands = new List<IEffectCommand>();
        var intent = new GainEnergyIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var targets = gainEnergyEffect.Targets.Eval(triggerContext);

        foreach (var target in targets)
        {
            var playerTarget = new PlayerTarget(target);
            var targetIntent = new GainEnergyIntentTargetAction(context.Action.Source, playerTarget);
            var targetTriggerContext = triggerContext with { Action = targetIntent };

            var gainEnergyPoint = gainEnergyEffect.Value.Eval(targetTriggerContext);

            effectCommands.Add(new GainEnergyEffectCommand(target, gainEnergyPoint));
        }
        return new EffectCommandSet(effectCommands);
    }

    private static EffectCommandSet ApplyLoseEnergyEffect(TriggerContext context, LoseEnegyEffect loseEnergyEffect)
    {
        var effectCommands = new List<IEffectCommand>();
        var intent = new LoseEnergyIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var targets = loseEnergyEffect.Targets.Eval(triggerContext);

        foreach (var target in targets)
        {
            var playerTarget = new PlayerTarget(target);
            var targetIntent = new LoseEnergyIntentTargetAction(context.Action.Source, playerTarget);
            var targetTriggerContext = triggerContext with { Action = targetIntent };

            var loseEnergyPoint = loseEnergyEffect.Value.Eval(targetTriggerContext);

            effectCommands.Add(new LoseEnergyEffectCommand(target, loseEnergyPoint));
        }
        return new EffectCommandSet(effectCommands);
    }
    #endregion

    #region Disposition Effect Handlers
    private static EffectCommandSet ApplyIncreaseDispositionEffect(TriggerContext context, IncreaseDispositionEffect increaseDispositionEffect)
    {
        var effectCommands = new List<IEffectCommand>();
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

                var increasePoint = increaseDispositionEffect.Value.Eval(targetTriggerContext);

                effectCommands.Add(new IncreaseDispositionEffectCommand(ally, increasePoint));
            }
        }
        return new EffectCommandSet(effectCommands);
    }
    private static EffectCommandSet ApplyDecreaseDispositionEffect(TriggerContext context, DecreaseDispositionEffect decreaseDispositionEffect)
    {
        var effectCommands = new List<IEffectCommand>();
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

                var decreasePoint = decreaseDispositionEffect.Value.Eval(targetTriggerContext);

                effectCommands.Add(new DecreaseDispositionEffectCommand(ally, decreasePoint));
            }
        }
        return new EffectCommandSet(effectCommands);
    }
    #endregion

    #region Player Buff Effect Handlers
    private static EffectCommandSet ApplyAddPlayerBuffEffect(TriggerContext context, AddPlayerBuffEffect addBuffEffect)
    {
        var effectCommands = new List<IEffectCommand>();
        var intent = new AddPlayerBuffIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var targets = addBuffEffect.Targets.Eval(triggerContext);

        foreach (var target in targets)
        {
            var playerTarget = new PlayerTarget(target);
            var targetIntent = new AddPlayerBuffIntentTargetAction(context.Action.Source, playerTarget);
            var targetTriggerContext = triggerContext with { Action = targetIntent };
            var level = addBuffEffect.Level.Eval(targetTriggerContext);

            effectCommands.Add(new AddPlayerBuffEffectCommand(target, addBuffEffect.BuffId, level));
        }
        return new EffectCommandSet(effectCommands);
    }

    private static EffectCommandSet ApplyRemovePlayerBuffEffect(TriggerContext context, RemovePlayerBuffEffect removeBuffEffect)
    {
        var effectCommands = new List<IEffectCommand>();
        var intent = new RemovePlayerBuffIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var targets = removeBuffEffect.Targets.Eval(triggerContext);

        foreach (var target in targets)
        {
            effectCommands.Add(new RemovePlayerBuffEffectCommand(target, removeBuffEffect.BuffId));
        }
        return new EffectCommandSet(effectCommands);
    }
    #endregion

    #region Card Effect Handlers
    private static EffectCommandSet ApplyDrawCardEffect(TriggerContext context, DrawCardEffect drawCardEffect)
    {
        var effectCommands = new List<IEffectCommand>();
        var intent = new DrawCardIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var targets = drawCardEffect.Targets.Eval(triggerContext);

        foreach (var target in targets)
        {
            var playerTarget = new PlayerTarget(target);
            var targetIntent = new DrawCardIntentTargetAction(context.Action.Source, playerTarget);
            var targetTriggerContext = triggerContext with { Action = targetIntent };
            var drawCount = drawCardEffect.Value.Eval(targetTriggerContext);
            
            effectCommands.Add(new DrawCardEffectCommand(target, drawCount));
        }

        return new EffectCommandSet(effectCommands);
    }

    private static EffectCommandSet ApplyDiscardCardEffect(TriggerContext context, DiscardCardEffect discardCardEffect)
    {
        var effectCommands = new List<IEffectCommand>();
        var intent = new DiscardCardIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var cards = discardCardEffect.TargetCards.Eval(triggerContext).ToList();

        foreach (var card in cards)
        {
            var destinationZone = card.IsConsumable() ?
                CardCollectionType.ExclusionZone :
                card.IsDisposable() ?
                    CardCollectionType.DisposeZone :
                    CardCollectionType.Graveyard;

            card.Owner(context.Model).MatchSome(cardOwner =>
            {
                cardOwner.CardManager.HandCard.GetCardOrNone(c => c.Identity == card.Identity)
                    .Map(handCard => CardCollectionType.HandCard)
                    .Else(cardOwner.CardManager.Deck.GetCardOrNone(card => card.Identity == card.Identity)
                        .Map(deckCard => CardCollectionType.Deck))
                    .MatchSome(cardStartZone =>
                    {
                        effectCommands.Add(new MoveCardEffectCommand(
                            cardOwner,
                            card,
                            cardStartZone,
                            destinationZone,
                            MoveCardType.Discard));
                    });
            });
        }
        return new EffectCommandSet(effectCommands);
    }

    private static EffectCommandSet ApplyConsumeCardEffect(TriggerContext context, ConsumeCardEffect consumeCardEffect)
    {
        var effectCommands = new List<IEffectCommand>();
        var intent = new ConsumeCardIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var cards = consumeCardEffect.TargetCards.Eval(triggerContext).ToList();

        foreach (var card in cards)
        {
            var destinationZone = card.IsDisposable() ?
                CardCollectionType.DisposeZone :
                CardCollectionType.ExclusionZone;

            card.Owner(context.Model).MatchSome(cardOwner =>
            {
                cardOwner.CardManager.HandCard.GetCardOrNone(c => c.Identity == card.Identity)
                    .Map(handCard => CardCollectionType.HandCard)
                    .Else(cardOwner.CardManager.Deck.GetCardOrNone(card => card.Identity == card.Identity)
                        .Map(deckCard => CardCollectionType.Deck))
                    .Else(cardOwner.CardManager.Graveyard.GetCardOrNone(card => card.Identity == card.Identity)
                        .Map(graveCard => CardCollectionType.Graveyard))
                    .MatchSome(cardStartZone =>
                    {
                        effectCommands.Add(new MoveCardEffectCommand(
                            cardOwner,
                            card,
                            cardStartZone,
                            destinationZone,
                            MoveCardType.Consume));
                    });
            });
        }
        return new EffectCommandSet(effectCommands);
    }

    private static EffectCommandSet ApplyDisposeCardEffect(TriggerContext context, DisposeCardEffect disposeCardEffect)
    {
        var effectCommands = new List<IEffectCommand>();
        var intent = new DisposeCardIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var cards = disposeCardEffect.TargetCards.Eval(triggerContext).ToList();

        foreach (var card in cards)
        {
            card.Owner(context.Model).MatchSome(cardOwner =>
            {
                cardOwner.CardManager.HandCard.GetCardOrNone(c => c.Identity == card.Identity)
                    .Map(handCard => CardCollectionType.HandCard)
                    .Else(cardOwner.CardManager.Deck.GetCardOrNone(card => card.Identity == card.Identity)
                        .Map(deckCard => CardCollectionType.Deck))
                    .Else(cardOwner.CardManager.Graveyard.GetCardOrNone(card => card.Identity == card.Identity)
                        .Map(graveCard => CardCollectionType.Graveyard)
                    .Else(cardOwner.CardManager.ExclusionZone.GetCardOrNone(card => card.Identity == card.Identity)
                        .Map(exclusionCard => CardCollectionType.ExclusionZone)))
                    .MatchSome(cardStartZone =>
                    {
                        effectCommands.Add(new MoveCardEffectCommand(
                            cardOwner,
                            card,
                            cardStartZone,
                            CardCollectionType.DisposeZone,
                            MoveCardType.Dispose));
                    });
            });
        }
        return new EffectCommandSet(effectCommands);
    }

    private static EffectCommandSet ApplyCreateCardEffect(TriggerContext context, CreateCardEffect createCardEffect)
    {
        var effectCommands = new List<IEffectCommand>();
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

                var newCard = CardEntity.RuntimeCreateFromId(cardDataId, context.Model.ContextManager.CardLibrary);
                createCardEffect.AddCardBuffDatas
                    .Select(addCardBuffData => newCard.BuffManager
                        .AddBuff(
                            context.Model.ContextManager.CardBuffLibrary,
                            targetTriggerContext,
                            addCardBuffData.CardBuffId,
                            addCardBuffData.Level.Eval(targetTriggerContext)));
                
                effectCommands.Add(new CreateCardEffectCommand(
                    targetPlayer,
                    newCard,
                    createCardEffect.CreateDestination));
            }
        });

        return new EffectCommandSet(effectCommands);
    }

    private static EffectCommandSet ApplyCloneCardEffect(TriggerContext context, CloneCardEffect cloneCardEffect)
    {
        var effectCommands = new List<IEffectCommand>();
        var intent = new CloneCardIntentAction(context.Action.Source);
        var triggerContext = context with { Action = intent };
        var target = cloneCardEffect.Target.Eval(triggerContext);
        
        target.MatchSome(targetPlayer =>
        {
            var playerTarget = new PlayerTarget(targetPlayer);
            var targetIntent = new CloneCardIntentTargetAction(context.Action.Source, playerTarget);
            var targetTriggerContext = triggerContext with { Action = targetIntent };
            var cards = cloneCardEffect.ClonedCards.Eval(targetTriggerContext);
            
            foreach (var originCard in cards)
            {
                var playerCardTarget = new PlayerAndCardTarget(targetPlayer, originCard);
                targetIntent = new CloneCardIntentTargetAction(context.Action.Source, playerCardTarget);
                targetTriggerContext = targetTriggerContext with { Action = targetIntent };
                
                var cloneCard = originCard.Clone(includeCardBuffs: false, includeCardProperties: false);
                cloneCardEffect.AddCardBuffDatas
                    .Select(addCardBuffData => cloneCard.BuffManager
                        .AddBuff(
                            context.Model.ContextManager.CardBuffLibrary,
                            targetTriggerContext,
                            addCardBuffData.CardBuffId,
                            addCardBuffData.Level.Eval(targetTriggerContext)));
                
                effectCommands.Add(new CloneCardEffectCommand(
                    targetPlayer,
                    originCard,
                    cloneCard,
                    cloneCardEffect.CloneDestination));
            }
        });
        return new EffectCommandSet(effectCommands);
    }

    private static EffectCommandSet ApplyAddCardBuffEffect(TriggerContext context, AddCardBuffEffect addCardBuffEffect)
    {
        var effectCommands = new List<IEffectCommand>();
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

                effectCommands.Add(new AddCardBuffEffectCommand(
                    card,
                    addCardBuff.CardBuffId,
                    addLevel));
            }
        }
        return new EffectCommandSet(effectCommands);
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
                    appleBuffEffectEvents.AddRange(
                        context.Model.UpdateReactorSessionAction(
                            new DamageResultAction(context.Action.Source, characterTarget, damageResult)));
                    appleBuffEffectEvents.Add(new DamageEvent(target.Faction(context.Model), target, damageResult));
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

                    appleBuffEffectEvents.AddRange(
                        context.Model.UpdateReactorSessionAction(
                            new DamageResultAction(context.Action.Source, characterTarget, damageResult)));
                    appleBuffEffectEvents.Add(new DamageEvent(target.Faction(context.Model), target, damageResult));
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
                        appleBuffEffectEvents.Add(new AddCardBuffEvent(card.Faction(context.Model), card.ToInfo(context.Model)));
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

        return new EffectResult(resultActions, appleBuffEffectEvents);
    }
    #endregion
    
    public static EffectResult RecycleCardOnPlayEnd(
        IGameplayModel model,
        IPlayerEntity player,
        ICardEntity card)
    {
        var drawCardEvents = new List<IGameEvent>();
        var resultActions = new List<BaseResultAction>();

        var recycleEvents = player.CardManager.RecycleCardOnPlayEnd(model, card);
        drawCardEvents.AddRange(recycleEvents);

        return new EffectResult(resultActions, drawCardEvents);
    }
}
