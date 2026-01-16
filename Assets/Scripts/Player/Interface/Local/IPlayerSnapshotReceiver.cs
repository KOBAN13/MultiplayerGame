using Player.Db;
using UnityEngine;

namespace Player.Interface.Local
{
    public interface IPlayerSnapshotReceiver
    {
        void SetSnapshot(in SnapshotData snapshotData);
    }
}