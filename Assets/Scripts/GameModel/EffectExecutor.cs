using System.Collections.Generic;
using System.Linq;
using Optional;
using UnityEngine;

public static class EffectExecutor
{
    #region CardEffect
    public static IEnumerable<IGameEvent> ApplyCardEffect(
        IGameplayStatusWatcher gameplayWatcher,
        IGameplayReactor gameplayReactor,
        IActionSource actionSource,
        ITriggerSource trigger,
        ICardEffect cardEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        switch (cardEffect)
        {
            case DamageEffect damageEffect:
            {
                var intent = new DamageIntentAction(actionSource, DamageType.Normal);
                var targets = damageEffect.Targets.Eval(gameplayWatcher, trigger, intent);
                foreach (var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var targetIntent = new DamageIntentTargetAction(actionSource, characterTarget, DamageType.Normal);
                    var damagePoint = damageEffect.Value.Eval(gameplayWatcher, trigger, targetIntent);
                    damagePoint = GameFormula.NormalDamagePoint(gameplayWatcher, damagePoint, targetIntent);

                    var damageResult = target.HealthManager.TakeDamage(damagePoint, gameplayWatcher.ContextManager.Context);
                    var damageStyle = DamageStyle.None; // TODO: pass style from action source

                    gameplayReactor.UpdateReactorSessionAction(new DamageResultAction(actionSource, characterTarget, damageResult, damageStyle));
                    cardEffectEvents.Add(new DamageEvent(target.Faction(gameplayWatcher), target, damageResult, damageStyle));
                }
                break;
            }
            case PenetrateDamageEffect penetrateDamageEffect:
            {
                var intent = new DamageIntentAction(actionSource, DamageType.Penetrate);
                var targets = penetrateDamageEffect.Targets.Eval(gameplayWatcher, trigger, intent);
                foreach (var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var targetIntent = new DamageIntentTargetAction(actionSource, characterTarget, DamageType.Penetrate);
                    var damagePoint = penetrateDamageEffect.Value.Eval(gameplayWatcher, trigger, targetIntent);
                    var damageResult = target.HealthManager.TakePenetrateDamage(damagePoint, gameplayWatcher.ContextManager.Context);
                    var damageStyle = DamageStyle.None;

                    gameplayReactor.UpdateReactorSessionAction(new DamageResultAction(actionSource, characterTarget, damageResult, damageStyle));
                    cardEffectEvents.Add(new DamageEvent(target.Faction(gameplayWatcher), target, damageResult, damageStyle));
                }
                break;
            }
            case AdditionalAttackEffect additionalAttackEffect:
            {
                var intent = new DamageIntentAction(actionSource, DamageType.Additional);
                var targets = additionalAttackEffect.Targets.Eval(gameplayWatcher, trigger, intent);
                foreach (var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var targetIntent = new DamageIntentTargetAction(actionSource, characterTarget, DamageType.Additional);
                    var damagePoint = additionalAttackEffect.Value.Eval(gameplayWatcher, trigger, targetIntent);
                    var damageResult = target.HealthManager.TakeAdditionalDamage(damagePoint, gameplayWatcher.ContextManager.Context);
                    var damageStyle = DamageStyle.None;

                    gameplayReactor.UpdateReactorSessionAction(new DamageResultAction(actionSource, characterTarget, damageResult, damageStyle));
                    cardEffectEvents.Add(new DamageEvent(target.Faction(gameplayWatcher), target, damageResult, damageStyle));
                }
                break;
            }
            case EffectiveAttackEffect effectiveAttackEffect:
            {
                var intent = new DamageIntentAction(actionSource, DamageType.Effective);
                var targets = effectiveAttackEffect.Targets.Eval(gameplayWatcher, trigger, intent);
                foreach (var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var targetIntent = new DamageIntentTargetAction(actionSource, characterTarget, DamageType.Effective);
                    var damagePoint = effectiveAttackEffect.Value.Eval(gameplayWatcher, trigger, targetIntent);
                    var damageResult = target.HealthManager.TakeEffectiveDamage(damagePoint, gameplayWatcher.ContextManager.Context);
                    var damageStyle = DamageStyle.None;

                    gameplayReactor.UpdateReactorSessionAction(new DamageResultAction(actionSource, characterTarget, damageResult, damageStyle));
                    cardEffectEvents.Add(new DamageEvent(target.Faction(gameplayWatcher), target, damageResult, damageStyle));
                }
                break;
            }
            case HealEffect healEffect:
            {
                var intent = new HealIntentAction(actionSource);
                var targets = healEffect.Targets.Eval(gameplayWatcher, trigger, intent);
                foreach (var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var targetIntent = new HealIntentTargetAction(actionSource, characterTarget);
                    var healPoint = healEffect.Value.Eval(gameplayWatcher, trigger, targetIntent);
                    var healResult = target.HealthManager.GetHeal(healPoint, gameplayWatcher.ContextManager.Context);

                    gameplayReactor.UpdateReactorSessionAction(new HealResultAction(actionSource, characterTarget, healResult));
                    cardEffectEvents.Add(new GetHealEvent(target.Faction(gameplayWatcher), target, healResult));
                }
                break;
            }
            case ShieldEffect shieldEffect:
            {
                var intent = new ShieldIntentAction(actionSource);
                var targets = shieldEffect.Targets.Eval(gameplayWatcher, trigger, intent);
                foreach (var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var targetIntent = new ShieldIntentTargetAction(actionSource, characterTarget);
                    var shieldPoint = shieldEffect.Value.Eval(gameplayWatcher, trigger, targetIntent);
                    var shieldResult = target.HealthManager.GetShield(shieldPoint, gameplayWatcher.ContextManager.Context);

                    gameplayReactor.UpdateReactorSessionAction(new ShieldResultAction(actionSource, characterTarget, shieldResult));
                    cardEffectEvents.Add(new GetShieldEvent(target.Faction(gameplayWatcher), target, shieldResult));
                }
                break;
            }
            case GainEnergyEffect gainEnergyEffect:
            {
                var intent = new GainEnergyIntentAction(actionSource);
                var targets = gainEnergyEffect.Targets.Eval(gameplayWatcher, trigger, intent);
                foreach (var target in targets)
                {
                    var playerTarget = new PlayerTarget(target);
                    var targetIntent = new GainEnergyIntentTargetAction(actionSource, playerTarget);
                    var gainEnergy = gainEnergyEffect.Value.Eval(gameplayWatcher, trigger, targetIntent);
                    var gainEnergyResult = target.EnergyManager.GainEnergy(gainEnergy);

                    gameplayReactor.UpdateReactorSessionAction(new GainEnergyResultAction(actionSource, playerTarget, gainEnergyResult));
                    cardEffectEvents.Add(new GainEnergyEvent(target, gainEnergyResult));
                }
                break;
            }
            case LoseEnegyEffect loseEnegyEffect:
            {
                var intent = new LoseEnergyIntentAction(actionSource);
                var targets = loseEnegyEffect.Targets.Eval(gameplayWatcher, trigger, intent);
                foreach (var target in targets)
                {
                    var playerTarget = new PlayerTarget(target);
                    var targetIntent = new LoseEnergyIntentTargetAction(actionSource, playerTarget);
                    var loseEnergy = loseEnegyEffect.Value.Eval(gameplayWatcher, trigger, targetIntent);
                    var loseEnergyResult = target.EnergyManager.LoseEnergy(loseEnergy);

                    gameplayReactor.UpdateReactorSessionAction(new LoseEnergyResultAction(actionSource, playerTarget, loseEnergyResult));
                    cardEffectEvents.Add(new LoseEnergyEvent(target, loseEnergyResult));
                }
                break;
            }

            // === BUFF EFFECT ===
            case AddPlayerBuffEffect addBuffEffect:
            {
                var intent = new AddPlayerBuffIntentAction(actionSource);
                var targets = addBuffEffect.Targets.Eval(gameplayWatcher, trigger, intent);
                foreach (var target in targets)
                {
                    var playerTarget = new PlayerTarget(target);
                    var targetIntent = new AddPlayerBuffIntentTargetAction(actionSource, playerTarget);
                    var level = addBuffEffect.Level.Eval(gameplayWatcher, trigger, targetIntent);
                    var addResult = target.BuffManager.AddBuff(
                        gameplayWatcher.ContextManager.BuffLibrary,
                        gameplayWatcher,
                        trigger,
                        targetIntent,
                        addBuffEffect.BuffId,
                        level);

                    gameplayReactor.UpdateReactorSessionAction(new AddPlayerBuffResultAction(actionSource, playerTarget, addResult));
                    IGameEvent buffEvent = addResult.IsNewBuff ?
                        new AddPlayerBuffEvent(target, addResult.Buff.ToInfo()) :
                        new UpdatePlayerBuffEvent(target, addResult.Buff.ToInfo());
                    cardEffectEvents.Add(buffEvent);
                }
                break;
            }
            case RemovePlayerBuffEffect removeBuffEffect:
            {
                var intent = new RemovePlayerBuffIntentAction(actionSource);
                var targets = removeBuffEffect.Targets.Eval(gameplayWatcher, trigger, intent);
                foreach (var target in targets)
                {
                    var playerTarget = new PlayerTarget(target);
                    var targetIntent = new RemovePlayerBuffIntentTargetAction(actionSource, playerTarget);
                    var removeResult = target.BuffManager.RemoveBuff(
                        gameplayWatcher.ContextManager.BuffLibrary,
                        gameplayWatcher,
                        targetIntent,
                        removeBuffEffect.BuffId);

                    removeResult.Buff.MatchSome(resultBuff =>
                    {
                        gameplayReactor.UpdateReactorSessionAction(new RemovePlayerBuffResultAction(actionSource, playerTarget, removeResult));
                        cardEffectEvents.Add(new RemovePlayerBuffEvent(target, resultBuff.ToInfo()));
                    });
                }
                break;
            }

            // === CARD EFFECT ===
            case DrawCardEffect drawCardEffect:
            {
                var intent = new DrawCardIntentAction(actionSource);
                var targets = drawCardEffect.Targets.Eval(gameplayWatcher, trigger, intent);
                foreach (var target in targets)
                {
                    var playerTarget = new PlayerTarget(target);
                    var targetIntent = new DrawCardIntentTargetAction(actionSource, playerTarget);
                    var drawCount = drawCardEffect.Value.Eval(gameplayWatcher, trigger, targetIntent);
                    var drawEvents = DrawCards(gameplayWatcher, gameplayReactor, actionSource, target, drawCount);
                    cardEffectEvents.AddRange(drawEvents);
                }
                break;
            }
            case DiscardCardEffect discardCardEffect:
            {
                var intent = new DiscardCardIntentAction(actionSource);
                var cards = discardCardEffect.TargetCards.Eval(gameplayWatcher, trigger, intent).ToList();
                for (var i = 0; i < cards.Count; i++)
                {
                    var card = cards[i];
                    card.Owner(gameplayWatcher.GameStatus).MatchSome(cardOwner =>
                    {
                        if (cardOwner.CardManager.TryDiscardCard(
                            card.Identity, out var discardedCard, out var start, out var destination))
                        {
                            var cardTarget = new CardTarget(card);
                            gameplayReactor.UpdateReactorSessionAction(new DiscardCardResultAction(actionSource, cardTarget, discardedCard));
                            cardEffectEvents.Add(new DiscardCardEvent(discardedCard, gameplayWatcher, start, destination));
                        }
                    });
                }
                break;
            }
            case ConsumeCardEffect consumeCardEffect:
            {
                var intent = new ConsumeCardIntentAction(actionSource);
                var cards = consumeCardEffect.TargetCards.Eval(gameplayWatcher, trigger, intent).ToList();
                for (var i = 0; i < cards.Count; i++)
                {
                    var card = cards[i];
                    card.Owner(gameplayWatcher.GameStatus).MatchSome(cardOwner =>
                    {
                        if (cardOwner.CardManager.TryConsumeCard(
                            card.Identity, out var consumedCard, out var start, out var destination))
                        {
                            var cardTarget = new CardTarget(card);
                            gameplayReactor.UpdateReactorSessionAction(new ConsumeCardResultAction(actionSource, cardTarget, consumedCard));
                            cardEffectEvents.Add(new ConsumeCardEvent(consumedCard, gameplayWatcher, start, destination));
                        }
                    });
                }
                break;
            }
            case DisposeCardEffect disposeCardEffect:
            {
                var intent = new DisposeCardIntentAction(actionSource);
                var cards = disposeCardEffect.TargetCards.Eval(gameplayWatcher, trigger, intent).ToList();
                for (var i = 0; i < cards.Count; i++)
                {
                    var card = cards[i];
                    card.Owner(gameplayWatcher.GameStatus).MatchSome(cardOwner =>
                    {
                        if (cardOwner.CardManager.TryDisposeCard(
                            card.Identity, out var disposedCard, out var start, out var destination))
                        {
                            var cardTarget = new CardTarget(card);
                            gameplayReactor.UpdateReactorSessionAction(new DisposeCardResultAction(actionSource, cardTarget, disposedCard));
                            cardEffectEvents.Add(new DisposeCardEvent(disposedCard, gameplayWatcher, start, destination));
                        }
                    });
                }
                break;
            }
            case CreateCardEffect createCardEffect:
            {
                var intent = new CreateCardIntentAction(actionSource);
                var target = createCardEffect.Target.Eval(gameplayWatcher, trigger, intent);
                target.MatchSome(targetPlayer =>
                {
                    foreach (var cardData in createCardEffect.CardDatas)
                    {
                        var playerTarget = new PlayerTarget(targetPlayer);
                        var targetIntent = new CreateCardIntentTargetAction(actionSource, playerTarget);

                        var createResult = targetPlayer
                            .CardManager.CreateNewCard(
                                gameplayWatcher,
                                trigger,
                                targetIntent,
                                cardData.Data,
                                createCardEffect.CreateDestination,
                                gameplayWatcher.ContextManager.CardBuffLibrary,
                                createCardEffect.AddCardBuffDatas);

                        gameplayReactor.UpdateReactorSessionAction(new CreateCardResultAction(actionSource, playerTarget, createResult));
                        cardEffectEvents.Add(new CreateCardEvent(createResult.Card, gameplayWatcher, createResult.Zone));
                    }
                });
                break;
            }
            case CloneCardEffect cloneCardEffect:
            {
                var intent = new CloneCardIntentAction(actionSource);
                var target = cloneCardEffect.Target.Eval(gameplayWatcher, trigger, intent);
                target.MatchSome(targetPlayer =>
                {
                    var playerTarget = new PlayerTarget(targetPlayer);
                    var targetIntent = new CloneCardIntentTargetAction(actionSource, playerTarget);
                    var cards = cloneCardEffect.ClonedCards.Eval(gameplayWatcher, trigger, intent);
                    foreach (var card in cards)
                    {
                        var playerCardTarget = new PlayerAndCardTarget(targetPlayer, card);
                        targetIntent = new CloneCardIntentTargetAction(actionSource, playerCardTarget);
                        var cloneResult = targetPlayer
                            .CardManager.CloneNewCard(
                                gameplayWatcher,
                                trigger,
                                targetIntent,
                                card,
                                cloneCardEffect.CloneDestination,
                                gameplayWatcher.ContextManager.CardBuffLibrary,
                                cloneCardEffect.AddCardBuffDatas);

                        gameplayReactor.UpdateReactorSessionAction(new CloneCardResultAction(actionSource, playerCardTarget, cloneResult));
                        cardEffectEvents.Add(new CloneCardEvent(cloneResult.Card, gameplayWatcher, cloneResult.Zone));
                    }
                });
                break;
            }
            case AddCardBuffEffect addCardBuffEffect:
            {
                var intent = new AddCardBuffIntentAction(actionSource);
                var cards = addCardBuffEffect.TargetCards.Eval(gameplayWatcher, trigger, intent).ToList();
                for (var i = 0; i < cards.Count; i++)
                {
                    var card = cards[i];
                    foreach (var addCardBuff in addCardBuffEffect.AddCardBuffDatas)
                    {
                        var cardTarget = new CardTarget(card);
                        var targetIntent = new AddCardBuffIntentTargetAction(actionSource, cardTarget);
                        var addLevel = addCardBuff.Level.Eval(gameplayWatcher, trigger, targetIntent);
                        var addBuffResult = card.BuffManager.AddBuff(
                            gameplayWatcher.ContextManager.CardBuffLibrary,
                            gameplayWatcher,
                            trigger,
                            targetIntent,
                            addCardBuff.CardBuffId,
                            addLevel);

                        gameplayReactor.UpdateReactorSessionAction(new AddCardBuffResultAction(actionSource, cardTarget, addBuffResult));
                        cardEffectEvents.Add(new AddCardBuffEvent(card, gameplayWatcher));
                    }
                }
                break;
            }
        }

        return cardEffectEvents;
    }
    #endregion

    #region PlayBuffEffect
    public static IEnumerable<IGameEvent> ApplyPlayerBuffEffect(
        IGameplayStatusWatcher gameplayWatcher,
        IGameplayReactor gameplayReactor,
        IActionSource actionSource,
        ITriggerSource triggerSource,
        IPlayerBuffEffect buffEffect)
    {
        var appleBuffEffectEvents = new List<IGameEvent>();
        gameplayReactor.UpdateReactorSessionTiming(UpdateTiming.TriggerBuffStart);

        switch (buffEffect)
        {
            case EffectiveDamagePlayerBuffEffect effectiveDamageBuffEffect:
            {
                var intent = new DamageIntentAction(actionSource, DamageType.Effective);
                var targets = effectiveDamageBuffEffect.Targets.Eval(gameplayWatcher, triggerSource, intent);
                foreach (var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var targetIntent = new DamageIntentTargetAction(actionSource, characterTarget, DamageType.Effective);
                    var damagePoint = effectiveDamageBuffEffect.Value.Eval(gameplayWatcher, triggerSource, targetIntent);
                    var damageResult = target.HealthManager.TakeEffectiveDamage(damagePoint, gameplayWatcher.ContextManager.Context);
                    var damageStyle = DamageStyle.None;

                    gameplayReactor.UpdateReactorSessionAction(new DamageResultAction(actionSource, characterTarget, damageResult, damageStyle));
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

                        gameplayReactor.UpdateReactorSessionAction(new AddCardBuffResultAction(actionSource, cardTarget, addResult));
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

        return appleBuffEffectEvents;
    }
    #endregion

    public static IEnumerable<IGameEvent> DrawCards(
        IGameplayStatusWatcher gameplayWatcher,
        IGameplayReactor gameplayReactor,
        IActionSource source,
        IPlayerEntity player,
        int drawCount)
    {
        var drawCardEvents = new List<IGameEvent>();
        for (int i = 0; i < drawCount; i++)
        {
            if (player.CardManager.Deck.Cards.Count == 0 &&
                player.CardManager.Graveyard.Cards.Count > 0)
            {
                var graveyardCards = player.CardManager.Graveyard.PopAllCards();
                player.CardManager.Deck.EnqueueCardsThenShuffle(graveyardCards);

                gameplayReactor.UpdateReactorSessionAction(new RecycleDeckResultAction(new PlayerTarget(player)));
                drawCardEvents.Add(new RecycleGraveyardEvent()
                {
                    Faction = player.Faction,
                    DeckInfo = player.CardManager.Deck.ToCardCollectionInfo(gameplayWatcher),
                    GraveyardInfo = player.CardManager.Graveyard.ToCardCollectionInfo(gameplayWatcher)
                });
            }

            if (player.CardManager.Deck.Cards.Count > 0)
            {
                _DrawCard(gameplayWatcher, gameplayReactor, source, player)
                    .MatchSome(drawCardEvent =>
                    {
                        drawCardEvents.Add(drawCardEvent);
                    });
            }
        }

        return drawCardEvents;
    }
    
    private static Option<IGameEvent> _DrawCard(
        IGameplayStatusWatcher gameplayWatcher,
        IGameplayReactor gameplayReactor,
        IActionSource source,
        IPlayerEntity player)
    {
        if (player.CardManager.Deck.PopCard(out ICardEntity newCard))
        {
            player.CardManager.HandCard.AddCard(newCard);
            gameplayReactor.UpdateReactorSessionAction(new DrawCardResultAction(source, new PlayerTarget(player), newCard));

            var newCardInfo = new CardInfo(newCard, gameplayWatcher);
            IGameEvent drawCardEvent = new DrawCardEvent()
            {
                Faction = player.Faction,
                NewCardInfo = newCardInfo,
                HandCardInfo = player.CardManager.HandCard.ToCardCollectionInfo(gameplayWatcher),
                DeckInfo = player.CardManager.Deck.ToCardCollectionInfo(gameplayWatcher)
            };
            return drawCardEvent.Some();
        }
        return Option.None<IGameEvent>();
    }
}
