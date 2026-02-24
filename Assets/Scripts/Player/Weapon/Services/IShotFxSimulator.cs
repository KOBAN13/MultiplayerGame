using Db.Projectile;
using Player.Weapon.Data;

namespace Player.Weapon.Services
{
    public interface IShotFxSimulator
    {
        void SimulateShotClient(AHitScanWeaponData weaponData, in ShotData shotData);
        void SimulateShotServer(AHitScanWeaponData weaponData, in ClientFireCommand shotData);
    }
}
