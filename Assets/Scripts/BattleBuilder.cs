using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.TextCore.Text;

public class BattleBuidler
{
    private Context _context;

    public BattleBuidler(Context context)
    {
        _context = context;
    }

    public GameContextManager ConstructGameContextManager()
    {
        var cardLibrary = new CardLibrary(_context.CardTable);
        var CardStatusLibrary = new CardStatusLibrary(_context.CardStatusTable);
        var buffLibrary = new BuffLibrary(_context.BuffTable);
        var dispositionLibrary = new DispositionLibrary(_context.DispositionSettings);
        var localizeLibrary = new LocalizeLibrary(_context.LocalizeSimpleSetting, _context.LocalizeTitleInfoSetting);

        return new GameContextManager(
            cardLibrary, 
            CardStatusLibrary,
            buffLibrary, 
            dispositionLibrary,
            localizeLibrary);
    }

    public GameStatus ConstructBattle(GameContextManager gameContextManager)
    { 
        var initialState = new GameStatus(
            round: 0,
            state: GameState.GameStart,
            player: _ParseAlly(_context.Ally, gameContextManager),
            enemy: _ParseEnemy(_context.AllEnemies[0])
        ); 
        return initialState;
    }

    private AllyEntity _ParseAlly(AllyInstance allyInstance, GameContextManager gameContextManager)
    {
        var character = new CharacterEntity(
            nameKey         : allyInstance.NameKey,
            currentHealth   : allyInstance.CurrentHealth,
            maxHealth       : allyInstance.MaxHealth          
        );

        return new AllyEntity(
            originPlayerInstanceGuid    : allyInstance.Identity,            
            characters                  : new CharacterEntity[] { character },
            currentEnergy               : allyInstance.CurrentEnergy,
            maxEnergy                   : allyInstance.MaxEnergy,
            handCardMaxCount            : allyInstance.HandCardMaxCount,
            currentDisposition          : allyInstance.CurrentDisposition,
            maxDisposition              : gameContextManager.DispositionLibrary.MaxDisposition,
            deckInstance                : allyInstance.Deck
        );
    }

    private EnemyEntity _ParseEnemy(EnemyData enemyData)
    {
        var enemyCardInstances = enemyData.PlayerData.Deck.Cards.Select(c => CardInstance.Create(c.Data)).ToList(); 

        var character = new CharacterEntity(
            nameKey         : enemyData.PlayerData.NameKey,
            currentHealth   : enemyData.PlayerData.InitialHealth,
            maxHealth       : enemyData.PlayerData.MaxHealth            
        );

        return new EnemyEntity(
            characters              : new CharacterEntity[] { character },
            currentEnergy           : enemyData.PlayerData.InitialEnergy,
            maxEnergy               : enemyData.PlayerData.MaxEnergy,
            handCardMaxCount        : enemyData.PlayerData.HandCardMaxCount,
            enemyCardInstances      : enemyCardInstances,
            selectedCardMaxCount    : enemyData.SelectedCardMaxCount,
            turnStartDrawCardCount  : enemyData.TurnStartDrawCardCount,
            energyRecoverPoint      : enemyData.EnergyRecoverPoint
        );
    }
}

