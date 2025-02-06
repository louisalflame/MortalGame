using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class AllyInstance
{
    public Guid Identity;
    public string NameKey;
    public int CurrentDisposition;

    public int CurrentHealth;
    public int MaxHealth;
    public int CurrentEnergy;
    public int MaxEnergy;

    public List<CardInstance> Deck;
    public int HandCardMaxCount;


}

