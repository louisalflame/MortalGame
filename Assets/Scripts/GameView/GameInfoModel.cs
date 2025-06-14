using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public interface IGameInfoModel
{
    void UpdateCardInfo(CardInfo cardInfo);
    void UpdatePlayerBuffInfo(PlayerBuffInfo playerBuffInfo);
    void UpdateCharacterBuffInfo(CharacterBuffInfo characterBuffInfo);

    IReadOnlyReactiveProperty<CardInfo> ObserveCardInfo(Guid identity);
    IReadOnlyReactiveProperty<PlayerBuffInfo> ObservePlayerBuffInfo(Guid identity);
    IReadOnlyReactiveProperty<CharacterBuffInfo> ObserveCharacterBuffInfo(Guid identity);
}

public class GameInfoModel : IGameInfoModel
{
    private Dictionary<Guid, ReactiveProperty<CardInfo>> _cardInfos;
    private Dictionary<Guid, ReactiveProperty<PlayerBuffInfo>> _playerBuffInfos;
    private Dictionary<Guid, ReactiveProperty<CharacterBuffInfo>> _characterBuffInfos;

    public GameInfoModel()
    {
        _cardInfos = new Dictionary<Guid, ReactiveProperty<CardInfo>>();
        _playerBuffInfos = new Dictionary<Guid, ReactiveProperty<PlayerBuffInfo>>();
        _characterBuffInfos = new Dictionary<Guid, ReactiveProperty<CharacterBuffInfo>>();
    }

    public void UpdateCardInfo(CardInfo cardInfo)
    {
        if (!_cardInfos.ContainsKey(cardInfo.Identity))
        {
            _cardInfos[cardInfo.Identity] = new ReactiveProperty<CardInfo>(cardInfo);
        }
        else
        {
            _cardInfos[cardInfo.Identity].Value = cardInfo;
        }
    }

    public void UpdatePlayerBuffInfo(PlayerBuffInfo playerBuffInfo)
    {
        if (!_playerBuffInfos.ContainsKey(playerBuffInfo.Identity))
        {
            _playerBuffInfos[playerBuffInfo.Identity] = new ReactiveProperty<PlayerBuffInfo>(playerBuffInfo);
        }
        else
        {
            _playerBuffInfos[playerBuffInfo.Identity].Value = playerBuffInfo;
        }
    }

    public void UpdateCharacterBuffInfo(CharacterBuffInfo characterBuffInfo)
    {
        if (!_characterBuffInfos.ContainsKey(characterBuffInfo.Identity))
        {
            _characterBuffInfos[characterBuffInfo.Identity] = new ReactiveProperty<CharacterBuffInfo>(characterBuffInfo);
        }
        else
        {
            _characterBuffInfos[characterBuffInfo.Identity].Value = characterBuffInfo;
        }
    }

    public IReadOnlyReactiveProperty<CardInfo> ObserveCardInfo(Guid identity)
    {
        return _cardInfos[identity];
    }

    public IReadOnlyReactiveProperty<PlayerBuffInfo> ObservePlayerBuffInfo(Guid identity)
    {
        return _playerBuffInfos[identity];
    }

    public IReadOnlyReactiveProperty<CharacterBuffInfo> ObserveCharacterBuffInfo(Guid identity)
    {
        return _characterBuffInfos[identity];
    }
}