using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffView : MonoBehaviour, IRecyclable
{
    [SerializeField]
    private Image _buffIcon;

    [SerializeField]
    private TextMeshProUGUI _levelText;

    public void SetBuffInfo(BuffInfo buffInfo)
    {
        _levelText.text = buffInfo.Level.ToString();
    }

    public void Reset()
    {
        _buffIcon.sprite = null;
        _levelText.text = string.Empty;
    }
}

public class BuffInfo
{
    public string Id;
    public Guid Identity;
    public int Level;
}