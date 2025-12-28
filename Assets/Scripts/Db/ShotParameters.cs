using Db.Interface;
using UnityEngine;

namespace Db
{
    [CreateAssetMenu(fileName = "ShotParameters", menuName = "Db/ShotParameters")]
    public class ShotParameters : ScriptableObject, IShotParameters
    {
        [field: SerializeField] public float DistanceToShot { get; private set; }
        [field: SerializeField] public LayerMask LayerMask { get; private set; }
    }
}