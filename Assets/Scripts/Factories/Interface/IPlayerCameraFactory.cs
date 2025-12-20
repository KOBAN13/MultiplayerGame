using Cysharp.Threading.Tasks;
using Player.Camera;
using UnityEngine;

namespace Factories.Interface
{
    public interface IPlayerCameraFactory
    {
        UniTaskVoid CreateCamera(Transform target);
    }
}
