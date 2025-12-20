using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Db;
using Factories;
using UI.View;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

namespace Pool
{
    public class GameListItemPool : IGameListItemPool, IDisposable
    {
        [Inject] private ScreensData _screensData;
        [Inject] private ViewsFactory _viewsFactory;

        private ObjectPool<GameListItem> _pool;
        private readonly Dictionary<int, GameListItem> _activeItems = new();
        private GameListItem _prefab;
        private GameObject _parent;

        public async UniTask Initialize(GameObject parent)
        {
            _parent = parent;

            var data = _screensData.Screens.FirstOrDefault(d => d.Type == typeof(GameListItem));
            var prefabHandle = await data.Asset.LoadAssetAsync<GameObject>();
            _prefab = prefabHandle.GetComponent<GameListItem>();

            _pool = new ObjectPool<GameListItem>(
                OnCreateItem,
                OnGetItem,
                OnReleaseItem,
                OnDestroyItem,
                collectionCheck: true,
                defaultCapacity: 10,
                maxSize: 100
            );
        }

        private GameListItem OnCreateItem()
        {
            var item = _viewsFactory.Create(_prefab, _parent.transform);
            item.gameObject.SetActive(false);
            return item;
        }

        private static void OnGetItem(GameListItem item) => item.gameObject.SetActive(true);
        private static void OnReleaseItem(GameListItem item) => item.gameObject.SetActive(false);

        private static void OnDestroyItem(GameListItem item)
        {
            if (item == null) return;
            if (item.gameObject != null)
                UnityEngine.Object.Destroy(item.gameObject);
        }

        public GameListItem GetListItem(int roomId)
        {
            var item = _pool.Get();
            _activeItems[roomId] = item;
            return item;
        }

        public void ReleaseListItem(int roomId)
        {
            if (!_activeItems.Remove(roomId, out var item))
                return;

            _pool.Release(item);
        }

        public GameListItem GetById(int roomId)
        {
            _activeItems.TryGetValue(roomId, out var item);
            return item;
        }

        public void Clear()
        {
            foreach (var kv in _activeItems.Values)
                _pool.Release(kv);
            
            _activeItems.Clear();
        }

        public void Dispose()
        {
            Clear();
            _pool?.Clear();
            _prefab = null;
            _parent = null;
        }
    }
}
