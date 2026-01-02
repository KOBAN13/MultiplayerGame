using Cysharp.Threading.Tasks;
using Db.Projectile;
using UnityEngine;
using Utils.Enums;

namespace Player.Weapon
{
    public abstract class AWeapon : MonoBehaviour, IWeapon
    {
        public abstract EWeaponType WeaponType { get; protected set; }
        
        protected AWeaponData Data;
        protected GameObject Owner;

        private float _currentProjectileCount;
        private bool _isReloading;
        private float _nextAttackTime;

        public virtual void Attack()
        {
            if (Time.time < _nextAttackTime || _isReloading) 
                return;

            if (_currentProjectileCount <= 0)
            {
                Reload();
                return;
            }
            
            PerformedAttack();
            
            _nextAttackTime = Time.time + 1f / Data.AttackRate;
            _currentProjectileCount--;
        }

        public virtual void Reload()
        {
            if (!_isReloading)
            {
                ReloadAsync().Forget();
            }
        }

        public AWeaponData GetWeaponData() => Data;

        public void SetOwner(GameObject owner) => Owner = owner;
        
        protected abstract void PerformedAttack();
        
        protected virtual void InitializeWeapon(AWeaponData data, GameObject owner)
        {
            Data = data;
            Owner = owner;
            
            _currentProjectileCount = Data.MagazineSize;
        }

        private async UniTaskVoid ReloadAsync()
        {
            _isReloading = true;
            await UniTask.WaitForSeconds(Data.ReloadTime);
            _isReloading = false;
            _currentProjectileCount = Data.MagazineSize;
        }
    }
}