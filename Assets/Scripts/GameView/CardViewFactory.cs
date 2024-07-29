using UnityEngine;

public class CardViewFactory : MonoBehaviour
{
    [SerializeField]
    private CardView _cardViewPrefab;

    public CardView CreateCardView()
    {
        return Instantiate(_cardViewPrefab, transform);
    }   
}
