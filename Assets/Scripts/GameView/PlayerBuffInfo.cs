using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public struct PlayerBuffInfo
{
    public string Id;
    public Guid Identity;
    public int Level;

    public const string KEY_LEVEL = "level";

    public Dictionary<string, string> GetTemplateValues()
    {
        return new Dictionary<string, string>()
        {
            { KEY_LEVEL, Level.ToString() },
        };
    }
}