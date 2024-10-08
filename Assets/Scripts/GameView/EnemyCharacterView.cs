using UnityEngine;

public class EnemyCharacterView : MonoBehaviour
{
    [SerializeField]
    private DamageEventViewFactory _damageEventViewFactory;
    [SerializeField]
    private HealEventViewFactory _healEventViewFactory;
    [SerializeField]
    private ShieldEventViewFactory _shieldEventViewFactory;
}
