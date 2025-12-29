using UnityEngine;

namespace Db.Projectile
{
    public abstract class AProjectileData : ScriptableObject
    {
        [field: SerializeField] public float Damage { get; protected set; }
        [field: SerializeField] public float Speed { get; protected set; }
        [field: SerializeField] public float LifeTime { get; protected set; }
    }
}