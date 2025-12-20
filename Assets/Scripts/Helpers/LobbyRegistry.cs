using System;
using Db;
using ObservableCollections;
using R3;
using Services.Interface;
using Sfs2X.Entities;
using Utils.Extension;
using VContainer.Unity;

namespace Helpers
{
    public class LobbyRegistry : IInitializable, IDisposable
    {
        private readonly IPingService _pingService;
        private IDisposable _subscription;

        public readonly ObservableDictionary<int, User> Users = new();

        public LobbyRegistry(IPingService pingService)
        {
            _pingService = pingService;
        }

        public void Initialize()
        {
            _subscription = _pingService.OnPingUpdate.Subscribe(_ => Users.NotifyChanged());
        }

        public void UpdateUser(User user) => Users[user.Id] = user;

        public void RemoveUser(int id) => Users.Remove(id);

        public void Clear() => Users.Clear();

        public void Dispose()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }

}