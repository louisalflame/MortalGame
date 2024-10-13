using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class ShieldEventView: MonoBehaviour, IRecyclable, IHealthEventView
{
    [SerializeField]
    private TextMeshProUGUI _text;
    [SerializeField]
    private PlayableDirector _playableDirector;

    public void SetEventInfo(GetShieldEvent getShieldEvent, Transform parent)
    {
        transform.SetParent(parent, false);
        _text.text = getShieldEvent.DeltaShield.ToString();
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
