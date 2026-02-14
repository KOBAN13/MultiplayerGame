using Input;
using Player.Db;
using UnityEngine;

namespace Player.Interface.Local
{
    public interface IPredictionStateProvider
    {
        PredictionStateFrame Read(Vector3 position);
    }
}