using System;
using System.Collections.Concurrent;
using Cysharp.Threading.Tasks;
using R3;
using Services.Db;
using Services.Interface;

namespace Services.Connections
{
    public class PlayerJoinGameService : IPlayerJoinGameService
    {
        private readonly ConcurrentQueue<PlayerJoinRequest> _pendingPlayerJoins = new();
        private readonly IPlayerSpawnService _playerSpawnService;
        private bool _isProcessingJoinQueue;
        private IDisposable _disposable;

        public PlayerJoinGameService(IPlayerSpawnService playerSpawnService)
        {
            _playerSpawnService = playerSpawnService;
            
            Initialize();
        }

        private void Initialize()
        {
            _disposable = Observable.EveryUpdate()
                .Subscribe(_ => Tick());
        }

        private void Tick()
        {
            if (_isProcessingJoinQueue || _pendingPlayerJoins.IsEmpty)
                return;

            _isProcessingJoinQueue = true;
            ProcessPendingPlayerJoins().Forget();
        }
        
        public void AddPlayerJoinRequest(PlayerJoinRequest joinRequest)
        {
            _pendingPlayerJoins.Enqueue(joinRequest);
        }

        private async UniTaskVoid ProcessPendingPlayerJoins()
        {
            try
            {
                while (_pendingPlayerJoins.TryDequeue(out var joinRequest))
                {
                    await _playerSpawnService.SpawnPlayer(joinRequest);
                }
            }
            finally
            {
                _isProcessingJoinQueue = false;
            }
        }

        public void Dispose()
        {
            _pendingPlayerJoins.Clear();
            _disposable?.Dispose();
        }
    }
}
