using Db.Interface;
using UnityEngine;

namespace Db.Players
{
    [CreateAssetMenu(fileName = "LocalPlayerParameters", menuName = "Db/LocalPlayerParameters")]
    public class LocalPlayerParameters : ScriptableObject, ILocalPlayerParameters
    {
        [field: SerializeField] public float RotationSmoothTime { get; private set; }
    }
}