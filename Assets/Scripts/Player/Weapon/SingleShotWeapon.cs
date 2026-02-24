using Db.Projectile;
using Helpers;
using Player.Weapon.Data;
using Player.Weapon.Services;
using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Utils.Enums;
using VContainer;

namespace Player.Weapon
{
    public class SingleShotWeapon : AWeapon
    {
        public override EWeaponType WeaponType => EWeaponType.SingleShot;
        
        private SmartFox _sfs;

        private AHitScanWeaponData HitScanWeaponData => (AHitScanWeaponData)Data;

        [Inject]
        private void Construct(SmartFox sfs)
        {
            _sfs = sfs;
        }
        
        protected override void PerformedAttack(ref ServerFireCommand command)
        {
            command.shotData.layerMask = HitScanWeaponData.ShotLayerMask;
            command.shotData.distanceToShot = HitScanWeaponData.ShotDistance;
            
            SendShotToServer(command);
            ShotFxSimulator.SimulateShotClient(HitScanWeaponData, command.shotData);
        }

        private void SendShotToServer(ServerFireCommand command)
        {
            var shotData = command.shotData;
            
            var data = SFSObject.NewInstance();
            
            var originArray = new SFSArray();
            originArray.AddFloat(shotData.origin.x);
            originArray.AddFloat(shotData.origin.y);
            originArray.AddFloat(shotData.origin.z);
                
            var directionArray = new SFSArray();
            directionArray.AddFloat(shotData.direction.x);
            directionArray.AddFloat(shotData.direction.y);
            directionArray.AddFloat(shotData.direction.z);
            
            data.PutSFSArray("originVector", originArray);
            data.PutSFSArray("directionVector", directionArray);
            data.PutByte("shotAlpha", command.alpha);
            
            data.PutLong("snapshotId", command.snapshotId);
            data.PutInt("layerMask", shotData.layerMask);
            data.PutFloat("distance", shotData.distanceToShot);
                    
            _sfs.Send(new ExtensionRequest(SFSResponseHelper.RAYCAST, data, _sfs.LastJoinedRoom));
        }
    }
}
