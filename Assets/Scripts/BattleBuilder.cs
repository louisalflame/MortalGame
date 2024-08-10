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

    private AllyEntity _ParseAlly(AllyData allyData)
    {
        return new AllyEntity() 
        {
            Name = allyData.PlayerData.NameKey,
            Character = new CharacterEntity(){
                HealthManager = new HealthManager(){
                    Hp = allyData.PlayerData.InitialHealth,
                    MaxHp = allyData.PlayerData.MaxHealth,
                },
                PowerManager = new PowerManager(){
                    Power = allyData.PlayerData.InitialPower,
                },
                EnergyManager = new EnergyManager(){
                    Energy = allyData.PlayerData.InitialEnergy,
                    MaxEnergy = allyData.PlayerData.MaxEnergy,
                },
                StatusManager = new StatusManager(),
            },
            Deck = new DeckEntity(){
                Cards = allyData.PlayerData.Deck.Cards.Select(c => _ParseCard(c.Data)).ToList(),
            },
            HandCard = new HandCardEntity(){
                MaxCount = allyData.PlayerData.HandCardMaxCount,
                Cards = new List<CardEntity>(),
            },
            Graveyard = new GraveyardEntity(){
                Cards = new List<CardEntity>(),
            },

            DispositionManager = new DispositionManager(allyData.InitialDisposition),
        };
    }

    private EnemyEntity _ParseEnemy(EnemyData enemyData)
    {
        return new EnemyEntity()
        {
            Name = enemyData.PlayerData.NameKey,
            Character = new CharacterEntity(){
                HealthManager = new HealthManager(){
                    Hp = enemyData.PlayerData.InitialHealth,
                    MaxHp = enemyData.PlayerData.MaxHealth,
                },
                PowerManager = new PowerManager(){
                    Power = enemyData.PlayerData.InitialPower,
                },
                EnergyManager = new EnergyManager(){
                    Energy = enemyData.PlayerData.InitialEnergy,
                    MaxEnergy = enemyData.PlayerData.MaxEnergy,
                },
                StatusManager = new StatusManager(),
            },
            Deck = new DeckEntity(){
                Cards = enemyData.PlayerData.Deck.Cards.Select(c => _ParseCard(c.Data)).ToList(),
            },
            HandCard = new HandCardEntity(){
                MaxCount = enemyData.PlayerData.HandCardMaxCount,
                Cards = new List<CardEntity>(),
            },
            Graveyard = new GraveyardEntity(){
                Cards = new List<CardEntity>(),
            },
            EnergyRecoverPoint = enemyData.EnergyRecoverPoint,
        };
    }

    private CardEntity _ParseCard(CardData cardData)
    {
        return new CardEntity(){
            Indentity = Guid.NewGuid().ToString(),
            Title = cardData.TitleKey,
            Info = cardData.InfoKey,
            Type = cardData.Type,
            Cost = cardData.Cost,
            Power = cardData.Power,
            OnUseEffects = cardData.OnUseEffects,
            OriginData = cardData,
        };
    }
}

