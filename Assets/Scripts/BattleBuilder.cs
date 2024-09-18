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
            state: GameState.None,
            player: _ParseAlly(_context.Ally),
            enemy: _ParseEnemy(_context.AllEnemies[0])
        ); 
        return initialState;
    }
    public GameContextManager ConstructBattleManager()
    {
        var cardLibrary = new CardLibrary(_context.CardTable);
        var buffLibrary = new BuffLibrary(_context.BuffTable);

        return new GameContextManager(cardLibrary, buffLibrary);
    }

    private AllyEntity _ParseAlly(AllyInstance allyInstance)
    {
        return new AllyEntity() 
        {
            Name = allyInstance.NameKey,
            Character = new CharacterEntity(){
                HealthManager = new HealthManager(allyInstance.CurrentHealth, allyInstance.MaxHealth),
                EnergyManager = new EnergyManager(allyInstance.CurrentEnergy, allyInstance.MaxEnergy),
                BuffManager = new BuffManager(),
            },
            Deck = new DeckEntity(
                allyInstance.Deck.Select(cardInstance => _ParseCard(cardInstance)).ToList() ),
            HandCard = new HandCardEntity(allyInstance.HandCardMaxCount),
            Graveyard = new GraveyardEntity(),

            DispositionManager = new DispositionManager(allyInstance.CurrentDisposition),
        };
    }

    private EnemyEntity _ParseEnemy(EnemyData enemyData)
    {
        var enemyCardInstances = enemyData.PlayerData.Deck.Cards.Select(c => CardInstance.Create(c.Data)).ToList(); 

        return new EnemyEntity()
        {
            Name = enemyData.PlayerData.NameKey,
            Character = new CharacterEntity(){
                HealthManager = new HealthManager(enemyData.PlayerData.InitialHealth, enemyData.PlayerData.MaxHealth),
                EnergyManager = new EnergyManager(enemyData.PlayerData.InitialEnergy, enemyData.PlayerData.MaxEnergy),
                BuffManager = new BuffManager(),
            },
            Deck = new DeckEntity(
                enemyCardInstances.Select(cardInstance => _ParseCard(cardInstance)).ToList()
            ),
            HandCard = new HandCardEntity(enemyData.PlayerData.HandCardMaxCount),
            Graveyard = new GraveyardEntity(),

            SelectedCards = new SelectedCardEntity(){
                MaxCount = enemyData.SelectedCardMaxCount,
                Cards = new List<CardEntity>(),
            },

            EnergyRecoverPoint = enemyData.EnergyRecoverPoint,
        };
    }

    private CardEntity _ParseCard(CardInstance cardInstance)
    {
        return new CardEntity(){
            Indentity = Guid.NewGuid(),
            Title = cardInstance.TitleKey,
            Info = cardInstance.InfoKey,
            Type = cardInstance.Type,
            Rarity = cardInstance.Rarity,
            Themes = cardInstance.Themes.ToArray(),
            Cost = cardInstance.Cost,
            Power = cardInstance.Power,
            Selectables = cardInstance.Selectables.ToArray(),
            Effects = cardInstance.Effects.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.ToList()
            ),
            Properties = cardInstance.PropertyDatas.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.Select(data => data.CreateEntity()).ToList()
            ),
            OriginCardInstanceGuid = cardInstance.InstanceGuid,
        };
    }
}

