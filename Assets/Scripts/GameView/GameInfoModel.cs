using System;
using System.Collections.Generic;
using Optional;
using Optional.Collections;
using UniRx;
using UnityEngine;

public interface IGameViewModel
{
    void UpdateCardCollectionInfo(Faction faction, CardCollectionInfo cardCollectionInfo);
    void EnableHandCardsAction(CardCollectionInfo handCardsInfod);
    void DisableHandCardsAction();

    IReadOnlyReactiveProperty<bool> IsHandCardsEnabled { get; }
    IReadOnlyReactiveProperty<CardCollectionInfo> ObservableCardCollectionInfo(Faction faction, CardCollectionType type);


    void UpdateCardInfo(CardInfo cardInfo);
    void UpdatePlayerBuffInfo(PlayerBuffInfo playerBuffInfo);
    void UpdateCharacterBuffInfo(CharacterBuffInfo characterBuffInfo);

    Option<IReadOnlyReactiveProperty<CardInfo>> ObservableCardInfo(Guid identity);
    Option<IReadOnlyReactiveProperty<PlayerBuffInfo>> ObservablePlayerBuffInfo(Guid identity);
    Option<IReadOnlyReactiveProperty<CharacterBuffInfo>> ObservableCharacterBuffInfo(Guid identity);
}

public class GameViewModel : IGameViewModel
{
    private Dictionary<Guid, ReactiveProperty<CardInfo>> _cardInfos;
    private Dictionary<Guid, ReactiveProperty<PlayerBuffInfo>> _playerBuffInfos;
    private Dictionary<Guid, ReactiveProperty<CharacterBuffInfo>> _characterBuffInfos;
    private Dictionary<Faction, Dictionary<CardCollectionType, ReactiveProperty<CardCollectionInfo>>> _cardCollectionInfos;

    private readonly ReactiveProperty<bool> _isHandCardsEnabled;
    public IReadOnlyReactiveProperty<bool> IsHandCardsEnabled => _isHandCardsEnabled;

    public GameViewModel()
    {
        _cardInfos = new Dictionary<Guid, ReactiveProperty<CardInfo>>();
        _playerBuffInfos = new Dictionary<Guid, ReactiveProperty<PlayerBuffInfo>>();
        _characterBuffInfos = new Dictionary<Guid, ReactiveProperty<CharacterBuffInfo>>();
        _cardCollectionInfos = new Dictionary<Faction, Dictionary<CardCollectionType, ReactiveProperty<CardCollectionInfo>>>
        {
            { Faction.Ally, new Dictionary<CardCollectionType, ReactiveProperty<CardCollectionInfo>>
                {
                    { CardCollectionType.Deck, new ReactiveProperty<CardCollectionInfo>(CardCollectionInfo.Empty) },
                    { CardCollectionType.HandCard, new ReactiveProperty<CardCollectionInfo>(CardCollectionInfo.Empty) },
                    { CardCollectionType.Graveyard, new ReactiveProperty<CardCollectionInfo>(CardCollectionInfo.Empty) },
                    { CardCollectionType.ExclusionZone, new ReactiveProperty<CardCollectionInfo>(CardCollectionInfo.Empty) },
                    { CardCollectionType.DisposeZone, new ReactiveProperty<CardCollectionInfo>(CardCollectionInfo.Empty) }
                }
            },
            { Faction.Enemy, new Dictionary<CardCollectionType, ReactiveProperty<CardCollectionInfo>>
                {
                    { CardCollectionType.Deck, new ReactiveProperty<CardCollectionInfo>(CardCollectionInfo.Empty) },
                    { CardCollectionType.HandCard, new ReactiveProperty<CardCollectionInfo>(CardCollectionInfo.Empty) },
                    { CardCollectionType.Graveyard, new ReactiveProperty<CardCollectionInfo>(CardCollectionInfo.Empty) },
                    { CardCollectionType.ExclusionZone, new ReactiveProperty<CardCollectionInfo>(CardCollectionInfo.Empty) },
                    { CardCollectionType.DisposeZone, new ReactiveProperty<CardCollectionInfo>(CardCollectionInfo.Empty) }
                }
            }
        };
        _isHandCardsEnabled = new ReactiveProperty<bool>(false);
    }

    public void EnableHandCardsAction(CardCollectionInfo handCardsInfo)
    {
        UpdateCardCollectionInfo(Faction.Ally, handCardsInfo);
        _isHandCardsEnabled.Value = true;
    }
    public void DisableHandCardsAction()
    {
        _isHandCardsEnabled.Value = false;
    }
    public void UpdateCardCollectionInfo(Faction faction, CardCollectionInfo cardCollectionInfo)
    {
        if (!_cardCollectionInfos[faction].ContainsKey(cardCollectionInfo.Type))
        {
            _cardCollectionInfos[faction][cardCollectionInfo.Type] = new ReactiveProperty<CardCollectionInfo>(cardCollectionInfo);
        }
        else
        {
            _cardCollectionInfos[faction][cardCollectionInfo.Type].Value = cardCollectionInfo;
        }

        foreach (var cardInfo in cardCollectionInfo.CardInfos)
        {
            UpdateCardInfo(cardInfo.Key);
        }
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

    public IReadOnlyReactiveProperty<CardCollectionInfo> ObservableCardCollectionInfo(Faction faction, CardCollectionType type)
    {
        return _cardCollectionInfos[faction][type];
    }


    public Option<IReadOnlyReactiveProperty<CardInfo>> ObservableCardInfo(Guid identity)
    {
        return OptionCollectionExtensions.GetValueOrNone(_cardInfos, identity)
            .Map(prop => (IReadOnlyReactiveProperty<CardInfo>)prop);
    }

    public Option<IReadOnlyReactiveProperty<PlayerBuffInfo>> ObservablePlayerBuffInfo(Guid identity)
    {
        return OptionCollectionExtensions.GetValueOrNone(_playerBuffInfos, identity)
            .Map(prop => (IReadOnlyReactiveProperty<PlayerBuffInfo>)prop);
    }

    public Option<IReadOnlyReactiveProperty<CharacterBuffInfo>> ObservableCharacterBuffInfo(Guid identity)
    {
        return OptionCollectionExtensions.GetValueOrNone(_characterBuffInfos, identity)
            .Map(prop => (IReadOnlyReactiveProperty<CharacterBuffInfo>)prop);
    }
}