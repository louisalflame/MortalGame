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

    private AllyEntity _ParseAlly(AllyInstance allyInstance)
    {
        return new AllyEntity() 
        {
            Name = allyInstance.NameKey,
            Character = new CharacterEntity(){
                HealthManager = new HealthManager(){
                    Hp = allyInstance.CurrentHealth,
                    MaxHp = allyInstance.MaxHealth,
                },
                EnergyManager = new EnergyManager(){
                    Energy = allyInstance.CurrentEnergy,
                    MaxEnergy = allyInstance.MaxEnergy,
                },
                StatusManager = new StatusManager(),
            },
            Deck = new DeckEntity(
                allyInstance.Deck.Select(cardInstance => _ParseCard(cardInstance)).ToList() ),
            HandCard = new HandCardEntity(){
                MaxCount = allyInstance.HandCardMaxCount,
                Cards = new List<CardEntity>(),
            },
            Graveyard = new GraveyardEntity(){
                Cards = new List<CardEntity>(),
            },
            BuffManager = new BuffManager(),

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
                HealthManager = new HealthManager(){
                    Hp = enemyData.PlayerData.InitialHealth,
                    MaxHp = enemyData.PlayerData.MaxHealth,
                },
                EnergyManager = new EnergyManager(){
                    Energy = enemyData.PlayerData.InitialEnergy,
                    MaxEnergy = enemyData.PlayerData.MaxEnergy,
                },
                StatusManager = new StatusManager(),
            },
            Deck = new DeckEntity(
                enemyCardInstances.Select(cardInstance => _ParseCard(cardInstance)).ToList()
            ),
            HandCard = new HandCardEntity(){
                MaxCount = enemyData.PlayerData.HandCardMaxCount,
                Cards = new List<CardEntity>(),
            },
            Graveyard = new GraveyardEntity(){
                Cards = new List<CardEntity>(),
            },

            SelectedCards = new SelectedCardEntity(){
                MaxCount = enemyData.SelectedCardMaxCount,
                Cards = new List<CardEntity>(),
            },
            BuffManager = new BuffManager(),

            EnergyRecoverPoint = enemyData.EnergyRecoverPoint,
        };
    }

    private CardEntity _ParseCard(CardInstance cardInstance)
    {
        return new CardEntity(){
            Indentity = Guid.NewGuid().ToString(),
            Title = cardInstance.TitleKey,
            Info = cardInstance.InfoKey,
            Type = cardInstance.Type,
            Rarity = cardInstance.Rarity,
            Themes = cardInstance.Themes.ToArray(),
            Cost = cardInstance.Cost,
            Power = cardInstance.Power,
            Selectables = cardInstance.Selectables.ToArray(),
            OnUseEffects = cardInstance.OnUseEffects.ToArray(),
            OriginCardInstanceId = cardInstance.InstanceId,
        };
    }
}

