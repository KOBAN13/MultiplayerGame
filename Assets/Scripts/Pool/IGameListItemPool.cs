using Cysharp.Threading.Tasks;
using UI.View;
using UnityEngine;

namespace Pool
{
    public interface IGameListItemPool
    {
        UniTask Initialize(GameObject parent);
        GameListItem GetListItem(int roomId);
        void ReleaseListItem(int roomId);
        GameListItem GetById(int roomId);
    }
}