using UnityEngine;
using UnityEngine.Serialization;
using Utils.Enums;

namespace Db.Projectile
{
    public abstract class AHitScanWeaponData : AWeaponData
    {
        [field: SerializeField, FormerlySerializedAs("<LayerMask>k__BackingField")]
        public LayerMask ShotLayerMask { get; protected set; }

        [field: SerializeField, FormerlySerializedAs("<DistanceToShot>k__BackingField")]
        public float ShotDistance { get; protected set; }

        [field: SerializeField]
        public EObjectInPoolName ImpactEffectId { get; protected set; } = EObjectInPoolName.BulletImpactEffect;
    }
}
