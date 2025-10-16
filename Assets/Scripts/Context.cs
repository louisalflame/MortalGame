using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Context
{
    public Dictionary<string, CardData> CardTable { get; private set; }
    public Dictionary<string, CardBuffData> CardBuffTable { get; private set; }
    public Dictionary<string, PlayerBuffData> BuffTable { get; private set; }
    public DispositionData[] DispositionSettings { get; private set; }
    public EnemyData[] AllEnemies { get; private set; }
    public AllyInstance Ally{ get; private set; }
    public Dictionary<LocalizeSimpleType, Dictionary<string, string>> LocalizeSimpleSetting { get; private set; }
    public Dictionary<LocalizeTitleInfoType, Dictionary<string, LocalizeTitleInfoData>> LocalizeTitleInfoSetting { get; private set; }

    public Context(
        ScriptableDataLoader scriptableDataLoader)
    {
        CardTable = scriptableDataLoader.AllCards.ToDictionary(c => c.ID, c => c);
        CardBuffTable = scriptableDataLoader.AllCardBuffs.ToDictionary(c => c.ID, c => c);
        BuffTable = scriptableDataLoader.AllBuffs.ToDictionary(b => b.ID, b => b);
        DispositionSettings = scriptableDataLoader.DispositionSettings;
        AllEnemies = scriptableDataLoader.AllEnemies;

        // Create player instance
        Ally = new AllyInstance{
            Identity = Guid.NewGuid(),
            NameKey = scriptableDataLoader.Ally.PlayerData.NameKey,
            CurrentDisposition = scriptableDataLoader.Ally.InitialDisposition,
            CurrentHealth = scriptableDataLoader.Ally.PlayerData.InitialHealth,
            MaxHealth = scriptableDataLoader.Ally.PlayerData.MaxHealth,
            CurrentEnergy = scriptableDataLoader.Ally.PlayerData.MaxEnergy,
            MaxEnergy = scriptableDataLoader.Ally.PlayerData.MaxEnergy,
            Deck = scriptableDataLoader.Ally.PlayerData.Deck.Cards.Select(card => CardInstance.Create(card.Data)).ToList(),
            HandCardMaxCount = scriptableDataLoader.Ally.PlayerData.HandCardMaxCount,
        };

        LocalizeSimpleSetting = scriptableDataLoader.LocalizeSimpleSetting;
        LocalizeTitleInfoSetting = scriptableDataLoader.LocalizeTitleInfoSetting;
    }
}
