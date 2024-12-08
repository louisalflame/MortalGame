using System;
using System.Collections.Generic;
using System.Linq;

public class BattleBuidler
{
    private Context _context;

    public BattleBuidler(Context context)
    {
        _context = context;
    }

    public GameStatus ConstructBattle()
    { 
        var initialState = new GameStatus(
            round: 0,
            state: GameState.GameStart,
            player: _ParseAlly(_context.Ally),
            enemy: _ParseEnemy(_context.AllEnemies[0])
        ); 
        return initialState;
    }
    public GameContextManager ConstructGameContextManager()
    {
        var cardLibrary = new CardLibrary(_context.CardTable);
        var CardStatusLibrary = new CardStatusLibrary(_context.CardStatusTable);
        var buffLibrary = new BuffLibrary(_context.BuffTable);
        var localizeLibrary = new LocalizeLibrary(_context.LocalizeSimpleSetting, _context.LocalizeTitleInfoSetting);

        return new GameContextManager(
            cardLibrary, 
            CardStatusLibrary,
            buffLibrary, 
            localizeLibrary);
    }

    private AllyEntity _ParseAlly(AllyInstance allyInstance)
    {
        return new AllyEntity(
            nameKey             : allyInstance.NameKey,
            currentHealth       : allyInstance.CurrentHealth,
            maxHealth           : allyInstance.MaxHealth,
            currentEnergy       : allyInstance.CurrentEnergy,
            maxEnergy           : allyInstance.MaxEnergy,
            handCardMaxCount    : allyInstance.HandCardMaxCount,
            currentDisposition  : allyInstance.CurrentDisposition,
            deckInstance        : allyInstance.Deck
        );
    }

    private EnemyEntity _ParseEnemy(EnemyData enemyData)
    {
        var enemyCardInstances = enemyData.PlayerData.Deck.Cards.Select(c => CardInstance.Create(c.Data)).ToList(); 

        return new EnemyEntity(
            nameKey                 : enemyData.PlayerData.NameKey,
            initialHealth           : enemyData.PlayerData.InitialHealth,
            maxHealth               : enemyData.PlayerData.MaxHealth,
            initialEnergy           : enemyData.PlayerData.InitialEnergy,
            maxEnergy               : enemyData.PlayerData.MaxEnergy,
            handCardMaxCount        : enemyData.PlayerData.HandCardMaxCount,
            enemyCardInstances      : enemyCardInstances,
            selectedCardMaxCount    : enemyData.SelectedCardMaxCount,
            turnStartDrawCardCount  : enemyData.TurnStartDrawCardCount,
            energyRecoverPoint      : enemyData.EnergyRecoverPoint
        );
    }
}

