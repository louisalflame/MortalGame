using System.Collections.Generic;
using UnityEngine;

public class CardBuffInfo
{
    public string CardBuffDataId { get; }
    public int Level { get; }
    public const string KEY_LEVEL = "level";

    public CardBuffInfo(ICardBuffEntity statusEntity)
    {
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
