using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Utils.Enums;

namespace Db.Projectile
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Db/WeaponData")]
    public class WeaponData : SerializedScriptableObject
    {
        [OdinSerialize] public IReadOnlyDictionary<EWeaponType, AWeaponData> Data { get; private set; }

        public bool TryGet(EWeaponType weaponType, out AWeaponData weaponData)
        {
            weaponData = null;

            return Data != null && Data.TryGetValue(weaponType, out weaponData);
        }
    }
}
