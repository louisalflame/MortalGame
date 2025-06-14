using System;
using System.Collections.Generic;
using UnityEngine;

public class CardBuffInfo
{    
    public readonly Guid Identity;
    public readonly string CardBuffDataId;
    public readonly int Level;
    public const string KEY_LEVEL = "level";

    public CardBuffInfo(ICardBuffEntity statusEntity)
    {
        Identity = statusEntity.Identity;
        CardBuffDataId = statusEntity.CardBuffDataID;
        Level = statusEntity.Level;
    }

    public Dictionary<string, string> GetTemplateValues()
    {
        return new Dictionary<string, string>()
        {
            { KEY_LEVEL, Level.ToString() },
        };
    }
}
