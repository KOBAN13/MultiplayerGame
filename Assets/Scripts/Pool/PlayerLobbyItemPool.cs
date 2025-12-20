using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Db;
using Factories;
using UI.View;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using VContainer;

namespace Pool
{ 
    public class PlayerLobbyItemPool : IPlayerLobbyItemPool
    {
        [Inject] private ScreensData _screensData;
        [Inject] private ViewsFactory _viewsFactory;

        private readonly Dictionary<int, PlayerLobbyItem> _activePlayers = new();
        private ObjectPool<PlayerLobbyItem> _pool;
        private PlayerLobbyItem _prefab;
        private GameObject _parent;

        public async void Initialize(GameObject parent)
        {
            var data = _screensData.Screens
                .FirstOrDefault(d => d.Type == typeof(PlayerLobbyItem));
            var handle = await data.Asset.LoadAssetAsync<GameObject>();

            _parent = parent;
            _prefab = handle.GetComponent<PlayerLobbyItem>();

            _pool = new ObjectPool<PlayerLobbyItem>
            (
                OnCreateGameListItem,
                OnGetGameListItem,
                OnReleaseGameListItem,
                OnDestroyGameListItem,
                true,
                10,
                100
            );
        }

        private static void OnDestroyGameListItem(PlayerLobbyItem obj)
        {
            Addressables.Release(obj.gameObject);
        }

        private static void OnReleaseGameListItem(PlayerLobbyItem obj)
        {
            obj.gameObject.SetActive(false);
        }

        private static void OnGetGameListItem(PlayerLobbyItem obj)
        {
            obj.gameObject.SetActive(true);
        }

        private PlayerLobbyItem OnCreateGameListItem()
        {
            var gameListItem = _viewsFactory.Create(_prefab, _parent.transform);
            gameListItem.gameObject.SetActive(false);
            return gameListItem;
        }

        public PlayerLobbyItem GetListItem(int userId)
        {
            var item = _pool.Get();
            _activePlayers[userId] = item;
            return item;
        }

        public void ReleaseListItem(int userId)
        {
            if (!_activePlayers.Remove(userId, out var item))
                return;

            _pool.Release(item);
        }

        public PlayerLobbyItem GetById(int userId)
        {
            _activePlayers.TryGetValue(userId, out var item);
            return item;
        }

        public void Clear()
        {
            foreach (var kv in _activePlayers.Values)
                _pool.Release(kv);
            
            _activePlayers.Clear();
        }
    }
}