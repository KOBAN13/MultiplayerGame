using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils.Enums;

namespace Factories.Interface
{
    public interface IPlayerFactory
    {
        UniTask<T> CreatePlayer<T>(EPlayerType playerType, Vector3 position, Quaternion rotation) where T : Component;
    }
}