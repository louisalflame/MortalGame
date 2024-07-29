using System.Collections.Generic;
using UnityEngine;

public class GameplayView : MonoBehaviour
{
    [SerializeField]
    private PlayerHandCardView _playerHandCardView;

    public void Render(IReadOnlyCollection<IGameEvent> events) 
    {
        foreach (var gameEvent in events)
        {
            switch (gameEvent)
            {
                case DrawCardEvent drawCardEvent:
                    _CreateCardView(drawCardEvent);
                    break;
            }
        }
    }

    private void _CreateCardView(DrawCardEvent drawCardEvent)
    {
        _playerHandCardView.CreateCardView(drawCardEvent);
    }
}
