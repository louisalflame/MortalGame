using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerData
{
    [BoxGroup("Identification")]
    public string ID;

    [TitleGroup("BasicData")]
    public int MaxHealth;
    [TitleGroup("BasicData")]
    public int MaxEnergy;
    [TitleGroup("BasicData")]
    [PropertyRange(0, "MaxHealth")]
    public int InitialHealth;
    [TitleGroup("BasicData")]
    [PropertyRange(0, "MaxEnergy")]
    public int InitialEnergy;
    [TitleGroup("BasicData")]
    public int InitialPower;
    [TitleGroup("BasicData")]
    [Range(0, 10)]
    public int InitialDisposition;

    [BoxGroup("Cards")]
    public DeckScriptable Deck;
    [BoxGroup("Cards")]
    public int HandCardMaxCount;


    [TitleGroup("Localization")]
    public string NameKey;
}
