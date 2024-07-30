using System.Collections.Generic;
using UnityEngine;

public class GameplayView : MonoBehaviour
{
    [SerializeField]
    private PlayerHandCardView _playerHandCardView;

    public void Render(IReadOnlyCollection<IGameEvent> events, IGameplayActionReciever reciever) 
    {
        foreach (var gameEvent in events)
        {
            switch (gameEvent)
            {
                case DrawCardEvent drawCardEvent:
                    _CreateCardView(drawCardEvent, reciever);
                    break;
            }
        }
    }

    private void _CreateCardView(DrawCardEvent drawCardEvent, IGameplayActionReciever reciever)
    {
        _playerHandCardView.CreateCardView(drawCardEvent, reciever);
    }
}
