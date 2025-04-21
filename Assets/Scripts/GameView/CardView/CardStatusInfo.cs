using UnityEngine;

public class CardBuffInfo
{
    public string CardBuffDataId { get; }

    public CardBuffInfo(ICardBuffEntity statusEntity)     
    {
        CardBuffDataId = statusEntity.CardBuffDataID;
    }
}
