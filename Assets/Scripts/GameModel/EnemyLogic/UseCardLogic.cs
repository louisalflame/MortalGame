using System.Linq;
using Optional;
using Optional.Collections;
using UnityEngine;

public static class UseCardLogic
{
    public static bool TryGetRecommandSelectCard(
        IGameplayStatusWatcher gameplayWatcher,
        EnemyEntity enemy,
        out ICardEntity cardEntity
    )
    {
        var totalSelectedCost = enemy.SelectedCards.Cards
            .Sum(card => GameFormula.CardCost(
                gameplayWatcher,
                card,
                new CardLookIntentAction(card),
                new CardTrigger(card)
            ));
        var remainCost = enemy.CurrentEnergy - totalSelectedCost;

        var candidateCards = enemy.CardManager.HandCard.Cards
            .Where(c => !enemy.SelectedCards.Cards.Contains(c))
            .Select(c => (
                Card: c,
                Cost: GameFormula.CardCost(gameplayWatcher, c, new CardLookIntentAction(c), new CardTrigger(c))
            ))
            .Where(c => c.Cost <= remainCost);

        var highestCard = candidateCards.OrderByDescending(c => c.Cost).FirstOrDefault();
        if (highestCard != default)
        {
            cardEntity = highestCard.Card;
            return true;
        }

        cardEntity = null;
        return false;
    }

    public static bool TryGetNextUseCardAction(
        IGameplayStatusWatcher gameplayWatcher,
        EnemyEntity enemy,
        out UseCardAction useCardAction)
    {
        foreach (var selectedCard in enemy.SelectedCards.Cards)
        {
            var selectResult = SelectTargetLogic.SelectTarget(gameplayWatcher, selectedCard);
            if (!selectResult.IsValid) continue;

            var cardRuntimeCost = GameFormula.CardCost(gameplayWatcher, selectedCard, new CardLookIntentAction(selectedCard), new CardTrigger(selectedCard));
            if (cardRuntimeCost <= enemy.CurrentEnergy)
            {
                useCardAction = new UseCardAction(selectedCard.Identity, selectResult.TargetType, selectResult.TargetId);
                return true;
            }
        }

        useCardAction = null;
        return false;
    }
}
