using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class HealEventView: MonoBehaviour, IRecyclable, IHealthEventView
{
    [SerializeField]
    private TextMeshProUGUI _text;
    [SerializeField]
    private PlayableDirector _playableDirector;
    
    public void SetEventInfo(GetHealEvent getHealEvent, Transform parent)
    {
        transform.SetParent(parent);
        _text.text = getHealEvent.DeltaHp.ToString();
    }

    public void Reset()
    {
    }

    public async UniTask PlayAnimation()
    {
        gameObject.SetActive(true);
        await _playableDirector.PlayAsync();
        gameObject.SetActive(false);
    }
}
