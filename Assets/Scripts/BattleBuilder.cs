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
                EnergyManager = new EnergyManager(){
                    Energy = allyData.PlayerData.InitialEnergy,
                    MaxEnergy = allyData.PlayerData.MaxEnergy,
                },
                StatusManager = new StatusManager(),
            },
            Deck = new DeckEntity(
                allyData.PlayerData.Deck.Cards.Select(c => _ParseCard(c.Data)).ToList() ),
            HandCard = new HandCardEntity(){
                MaxCount = allyData.PlayerData.HandCardMaxCount,
                Cards = new List<CardEntity>(),
            },
            Graveyard = new GraveyardEntity(){
                Cards = new List<CardEntity>(),
            },
            BuffManager = new BuffManager(),

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
                EnergyManager = new EnergyManager(){
                    Energy = enemyData.PlayerData.InitialEnergy,
                    MaxEnergy = enemyData.PlayerData.MaxEnergy,
                },
                StatusManager = new StatusManager(),
            },
            Deck = new DeckEntity(
                enemyData.PlayerData.Deck.Cards.Select(c => _ParseCard(c.Data)).ToList() ),
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

    private CardEntity _ParseCard(CardData cardData)
    {
        return new CardEntity(){
            Indentity = Guid.NewGuid().ToString(),
            Title = cardData.TitleKey,
            Info = cardData.InfoKey,
            Type = cardData.Type,
            Rarity = cardData.Rarity,
            Themes = cardData.Themes.ToArray(),
            Cost = cardData.Cost,
            Power = cardData.Power,
            Selectables = cardData.Selectables.ToArray(),
            OnUseEffects = cardData.OnUseEffects.ToArray(),
            OriginData = cardData,
        };
    }
}

