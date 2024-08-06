using UnityEngine;

public class AiCardViewFactory : MonoBehaviour
{
    [SerializeField]
    private AiCardView _cardViewPrefab;
    [SerializeField]
    private Transform _recycleRoot;

    public AiCardView CreateCardView()
    {
        return Instantiate(_cardViewPrefab, transform);
    }

    public void RecycleCardView(AiCardView cardView)
    {
        cardView.Reset();
        cardView.transform.SetParent(_recycleRoot);
    }
}
