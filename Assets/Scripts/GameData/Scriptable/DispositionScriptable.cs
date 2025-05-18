using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "DispositionScriptable", menuName = "Scriptable Objects/DispositionScriptable")]
public class DispositionScriptable : SerializedScriptableObject
{
    public DispositionData[] Datas = new DispositionData[0];
}