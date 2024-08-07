using System.Collections.Generic;

public class BattleBuidler
{
    public GameStatus ConstructBattle()
    { 
        var initialState = new GameStatus(
            round: 0,
            state: GameState.None,
            player: new AllyEntity() {
                Name = "趙活",
                Character = new CharacterEntity() {
                    HealthManager = new HealthManager() {
                        Hp = 100,
                        MaxHp = 100,
                    },
                    PowerManager = new PowerManager() {
                        Power = 1,
                    },
                    EnergyManager = new EnergyManager() {
                        Energy = 0,
                        MaxEnergy = 100,
                    },
                    StatusManager = new StatusManager(),
                    DispositionManager = new DispositionManager(5,10),
                },
                Deck = new DeckEntity() {
                    Cards = _CreateDeck()
                },
                HandCard = new HandCardEntity {
                    MaxCount = 5,
                    Cards = new List<CardEntity>(),
                },
                Graveyard = new GraveyardEntity {
                    Cards = new List<CardEntity>(),
                },
            },
            enemy: new EnemyEntity() {
                Name = "路人俠",
                Character = new CharacterEntity() {
                    HealthManager = new HealthManager() {
                        Hp = 100,
                        MaxHp = 100,
                    },
                    PowerManager = new PowerManager() {
                        Power = 1,
                    },
                    EnergyManager = new EnergyManager() {
                        Energy = 0,
                        MaxEnergy = 100,
                    },
                    StatusManager = new StatusManager(),
                    DispositionManager = new DispositionManager(5,10),
                },
                Deck = new DeckEntity() {
                    Cards = _CreateDeck()
                },
                HandCard = new HandCardEntity {
                    MaxCount = 3,
                    Cards = new List<CardEntity>(),
                },
                Graveyard = new GraveyardEntity {
                    Cards = new List<CardEntity>(),
                },
            }
        ); 
        return initialState;
    }

    private IReadOnlyCollection<CardEntity> _CreateDeck()
    {
        return new List<CardEntity>() {
            new CardEntity() {
                CardIndentity = 10001,
                Title = "Attack",
                Info = "Deal 5 damage to enemy",
                Type = CardType.Attack,
                Cost = 1,
                Power = 5,
            },
            new CardEntity() {
                CardIndentity = 10002,
                Title = "Attack",
                Info = "Deal 5 damage to enemy",
                Type = CardType.Attack,
                Cost = 1,
                Power = 5,
            },
            new CardEntity() {
                CardIndentity = 10003,
                Title = "Attack",
                Info = "Deal 5 damage to enemy",
                Type = CardType.Attack,
                Cost = 1,
                Power = 5,
            },
            new CardEntity() {
                CardIndentity = 10004,
                Title = "Attack",
                Info = "Deal 5 damage to enemy",
                Type = CardType.Attack,
                Cost = 1,
                Power = 5,
            },
            new CardEntity() {
                CardIndentity = 10005,
                Title = "Attack",
                Info = "Deal 5 damage to enemy",
                Type = CardType.Attack,
                Cost = 1,
                Power = 5,
            },
            new CardEntity() {
                CardIndentity = 20001,
                Title = "Defend",
                Info = "Gain 5 block",
                Type = CardType.Defense,
                Cost = 1,
                Power = 5,
            },
            new CardEntity() {
                CardIndentity = 20002,
                Title = "Defend",
                Info = "Gain 5 block",
                Type = CardType.Defense,
                Cost = 1,
                Power = 5,
            },
            new CardEntity() {
                CardIndentity = 20003,
                Title = "Defend",
                Info = "Gain 5 block",
                Type = CardType.Defense,
                Cost = 1,
                Power = 5,
            },
            new CardEntity() {
                CardIndentity = 20004,
                Title = "Defend",
                Info = "Gain 5 block",
                Type = CardType.Defense,
                Cost = 1,
                Power = 5,
            },
            new CardEntity() {
                CardIndentity = 20005,
                Title = "Defend",
                Info = "Gain 5 block",
                Type = CardType.Defense,
                Cost = 1,
                Power = 5,
            },
        };
    }
}

