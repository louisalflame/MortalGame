using System;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class BuffView : MonoBehaviour, IRecyclable
{
    [SerializeField]
    private Image _buffIcon;

    [SerializeField]
    private TextMeshProUGUI _levelText;

    [SerializeField]
    private RectTransform _rectTransform;
    
    private CompositeDisposable _disposables = new CompositeDisposable();

    public void SetBuffInfo(BuffInfo buffInfo, SimpleTitleIInfoHintView simpleHintView)
    {
        _levelText.text = buffInfo.Level.ToString();
        
        _disposables.Dispose();
        // Disposed object can't be reused by same instance.
        _disposables = new CompositeDisposable();

        _buffIcon.OnPointerEnterAsObservable()
            .Subscribe(_ => simpleHintView.ShowBuffInfo(buffInfo, _rectTransform)) 
            .AddTo(_disposables);
        _buffIcon.OnPointerExitAsObservable()
            .Subscribe(_ => simpleHintView.Close())
            .AddTo(_disposables);
    }

    public void Reset()
    {
        _buffIcon.sprite = null;
        _levelText.text = string.Empty;

        _disposables.Dispose();
        // Disposed object can't be reused by same instance.
        _disposables = new CompositeDisposable();
    }
}

public class BuffInfo
{
    public string Id;
    public Guid Identity;
    public int Level;
}