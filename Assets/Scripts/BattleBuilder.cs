using System.Collections.Generic;

public class BattleBuidler
{
    public GameStatus ConstructBattle()
    { 
        var initialState = new GameStatus(
            state: GameState.None,
            player: new PlayerEntity() {
                IsNPC = false,
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
                },
                Deck = new DeckEntity() {
                    Cards = _CreateDeck()
                },
                HandCard = new HandCardEntity {
                    Cards = new List<CardEntity>(),
                },
                Graveyard = new CardGraveyardEntity {
                    Cards = new List<CardEntity>(),
                },
            },
            enemy: new PlayerEntity() {
                IsNPC = true,
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
                },
                Deck = new DeckEntity() {
                    Cards = _CreateDeck()
                },
                HandCard = new HandCardEntity {
                    Cards = new List<CardEntity>(),
                },
                Graveyard = new CardGraveyardEntity {
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
                Title = "Defend",
                Info = "Gain 5 block",
                Type = CardType.Defense,
                Cost = 1,
                Power = 5,
            },
            new CardEntity() {
                CardIndentity = 10005,
                Title = "Defend",
                Info = "Gain 5 block",
                Type = CardType.Defense,
                Cost = 1,
                Power = 5,
            },
            new CardEntity() {
                CardIndentity = 10006,
                Title = "Defend",
                Info = "Gain 5 block",
                Type = CardType.Defense,
                Cost = 1,
                Power = 5,
            },
        };
    }
}

