using Db.Interface;
using Services.Interface;
using UnityEngine;

namespace Player.Shared
{
    public struct SnapshotCharacterMotorArgs
    {
        public readonly ISnapshotsService _snapshotsService;
        public readonly Transform _physicalRoot;
        public readonly Transform _visualRoot;
        public readonly Transform _yawTarget;
        public readonly IRemotePlayerParameters _remotePlayerParameters;
        
        public SnapshotCharacterMotorArgs(
            ISnapshotsService snapshotsService,
            Transform physicalRoot,
            Transform visualRoot,
            Transform yawTarget,
            IRemotePlayerParameters remotePlayerParameters)
        {
            _snapshotsService = snapshotsService;
            _physicalRoot = physicalRoot;
            _visualRoot = visualRoot;
            _yawTarget = yawTarget;
            _remotePlayerParameters = remotePlayerParameters;
        }
    }
}