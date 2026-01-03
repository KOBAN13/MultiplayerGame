using UnityEngine;

namespace Db.Projectile
{
    [CreateAssetMenu(fileName = "SingleShotWeaponData", menuName = "Db/SingleShotWeaponData")]
    public class SingleShotWeaponData : AWeaponData
    {
        [field: SerializeField] public LayerMask LayerMask { get; private set; }
        [field: SerializeField] public float DistanceToShot { get; private set; }
    }
}
