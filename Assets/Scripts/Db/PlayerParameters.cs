using Db.Interface;
using UnityEngine;

namespace Db
{
    [CreateAssetMenu(fileName = "PlayerParameters", menuName = "Db/PlayerParameters")]
    public class PlayerParameters : ScriptableObject, IPlayerParameters
    {
        [field: SerializeField] public float SmoothSpeed { get; private set; }
        [field: SerializeField] public float RotationSmoothTime { get; private set; }
    }
}