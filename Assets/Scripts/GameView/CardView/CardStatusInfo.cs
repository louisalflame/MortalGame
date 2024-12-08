using UnityEngine;

public class CardStatusInfo
{
    public string CardStatusDataId { get; }

    public CardStatusInfo(ICardStatusEntity statusEntity)     
    {
        CardStatusDataId = statusEntity.CardStatusDataID;
    }
}
