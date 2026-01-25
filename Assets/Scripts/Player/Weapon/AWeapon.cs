using Cysharp.Threading.Tasks;
using Db.Projectile;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.Enums;
using VContainer;

namespace Player.Weapon
{
    public abstract class AWeapon : MonoBehaviour, IWeapon
    {
        [Header("References")]
        [SerializeField] protected Transform ShotPoint;
        [SerializeField] protected int WeaponId;
        
        public abstract EWeaponType WeaponType { get; }
        
        protected AWeaponData Data;
        protected GameObject Owner;

        private float _currentProjectileCount;
        private bool _isReloading;
        private float _nextAttackTime;
        
        [Inject]
        private void Construct(WeaponData weaponData)
        {
            if (weaponData == null)
            {
                Debug.LogError($"{name}: WeaponData is not registered in the container.", this);
                enabled = false;
                return;
            }

            if (!weaponData.TryGet(WeaponType, out var data) || data == null)
            {
                Debug.LogError($"{name}: Missing weapon data for {WeaponType}.", this);
                enabled = false;
                return;
            }

            Data = data;
            Owner = gameObject;
            _currentProjectileCount = Data.MagazineSize;
        }

        public virtual void Attack(ref FireCommand command)
        {
            if (Time.time < _nextAttackTime || _isReloading) 
                return;

            if (_currentProjectileCount <= 0)
            {
                Reload();
                return;
            }
            
            PerformedAttack(ref command);
            
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

        public int GetWeaponId() => WeaponId;

        public AWeaponData GetWeaponData() => Data;

        public void SetOwner(GameObject owner) => Owner = owner;
        
        protected abstract void PerformedAttack(ref FireCommand command);

        private async UniTaskVoid ReloadAsync()
        {
            _isReloading = true;
            await UniTask.WaitForSeconds(Data.ReloadTime);
            _isReloading = false;
            _currentProjectileCount = Data.MagazineSize;
        }
    }
}
