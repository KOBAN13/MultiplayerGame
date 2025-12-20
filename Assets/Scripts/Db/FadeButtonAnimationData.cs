using UnityEngine;

namespace Db
{
    [CreateAssetMenu(fileName = "FadeButtonAnimationData", menuName = "Db/FadeButtonAnimationData")]
    public class FadeButtonAnimationData : ScriptableObject
    {
        [field: SerializeField] public float OnClickAlpha { get; private set; }
        [field: SerializeField] public float FadeTime { get; private set; }
        [field: SerializeField] public float OnHoverAlpha { get; private set; }
    }
}