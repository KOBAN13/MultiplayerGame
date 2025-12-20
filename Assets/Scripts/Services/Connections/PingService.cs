using System;
using System.Collections.Generic;
using Helpers;
using R3;
using Services.Interface;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities.Variables;
using Sfs2X.Requests;
using VContainer.Unity;

namespace Services.Connections
{
    public class PingService : IPingService, IInitializable, IDisposable
    {
        private readonly SmartFox _sfs;
        private const int PING_INTERVAL = 4;
        private IDisposable _subscription;
        private int _currentPing;
        private readonly List<UserVariable> _pingList = new ();
        private readonly Subject<Unit> _onPingUpdate = new();
        private SetUserVariablesRequest _setUserVariablesRequest;
        public Observable<Unit> OnPingUpdate => _onPingUpdate;

        public PingService(SmartFox sfs)
        {
            _sfs = sfs;
        }

        public void Initialize()
        {
            _setUserVariablesRequest = new SetUserVariablesRequest(_pingList);
            
            _sfs.AddEventListener(SFSEvent.PING_PONG, evt => OnUpdatePingPong(evt));
            _sfs.EnableLagMonitor(true, PING_INTERVAL);
            
            SendPingVariablesToServer();
            
            OnPingVariablesUpdate();
        }

        private void SendPingVariablesToServer()
        {
            _subscription = Observable.Timer(TimeSpan.FromSeconds(PING_INTERVAL + 0.5f), TimeSpan.FromSeconds(PING_INTERVAL + 0.5f))
                .Subscribe(_ => OnPingVariablesUpdate());
        }
        
        private void OnUpdatePingPong(BaseEvent evt)
        {
            if (evt.Params.TryGetValue("lagValue", out var param))
            {
                _currentPing = (int)param;
                _onPingUpdate.OnNext(Unit.Default);
            }
        }
        
        private void OnPingVariablesUpdate()
        {
            _pingList.Clear();
            
            _pingList.Add(new SFSUserVariable(SFSResponseHelper.USER_PING, _currentPing));
            
            _sfs.Send(_setUserVariablesRequest);
        }
        
        public void Dispose()
        {
            _sfs.RemoveEventListener(SFSEvent.PING_PONG, OnUpdatePingPong);
            _subscription.Dispose();
            _sfs.LagMonitor.Destroy();
        }
    }
}