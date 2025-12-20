using UI.View;
using UnityEngine;

namespace Pool
{
    public interface IPlayerLobbyItemPool
    {
        void Initialize(GameObject parent);
        
        PlayerLobbyItem GetListItem(int userId);

        void ReleaseListItem(int userId);

        PlayerLobbyItem GetById(int userId);

        void Clear();
    }
}