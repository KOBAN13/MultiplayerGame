using Sirenix.OdinInspector;
using UnityEngine;
using Utils.Enums;

namespace Db.Projectile
{
    public abstract class AWeaponData : SerializedScriptableObject
    {
        [field: SerializeField] public EWeaponType WeaponType { get; protected set; }
        [field: SerializeField] public float AttackRate { get; protected set; }
        [field: SerializeField] public float ReloadTime { get; protected set; }
        [field: SerializeField] public int MagazineSize { get; protected set; }
        [field: SerializeField] public AProjectileData ProjectileData { get; protected set; }
    }
}