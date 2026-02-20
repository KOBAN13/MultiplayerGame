using System.Collections.Generic;
using Services.Interface;
using VContainer;

namespace Services.Snapshot
{
    public class SnapshotsServiceRegistry : ISnapshotsServiceRegistry
    {
        private readonly Dictionary<int, ISnapshotsService> _snapshotsServices = new();
        private readonly IObjectResolver _objectResolver;
        
        public SnapshotsServiceRegistry(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        public void AddSnapshotService(int playerId)
        {
            if (_snapshotsServices.ContainsKey(playerId)) 
                return;
            
            var service = _objectResolver.Resolve<ISnapshotsService>();
            
            _snapshotsServices.Add(playerId, service);
        }

        public ISnapshotsService GetSnapshotService(int playerId)
        {
            return _snapshotsServices.GetValueOrDefault(playerId);
        }

        public bool RemoveSnapshotService(int playerId)
        {
            return _snapshotsServices.Remove(playerId);
        }

        public void Clear()
        {
            _snapshotsServices.Clear();
        }
    }
}
