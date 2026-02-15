using Player;
using Player.Db;
using UnityEngine;

namespace Services.Interface
{
    public interface IReconciliationService
    {
        void Reconciliation(APlayer player, Vector3 position, in SnapshotData snapshotData);
    }
}
