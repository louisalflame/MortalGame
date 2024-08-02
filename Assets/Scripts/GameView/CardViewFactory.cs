using UnityEngine;

public class CardViewFactory : MonoBehaviour
{
    [SerializeField]
    private CardView _cardViewPrefab;
    [SerializeField]
    private Transform _recycleRoot;

    public CardView CreateCardView()
    {
        return Instantiate(_cardViewPrefab, transform);
    }

    public void RecycleCardView(CardView cardView)
    {
        cardView.Reset();
        cardView.transform.SetParent(_recycleRoot);
    }
}
