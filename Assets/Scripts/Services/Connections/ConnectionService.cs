using System;
using Cysharp.Threading.Tasks;
using Db;
using Helpers;
using Newtonsoft.Json;
using R3;
using Services.Interface;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Util;
using UI.View;
using UnityEngine;
using UnityEngine.Networking;
using VContainer.Unity;

namespace Services.Connections
{
    public class ConnectionService : IConnectionService, IInitializable, ITickable, IDisposable
    {
        private SmartFox _sfs;
        private readonly IScreenService _screenService;
        private readonly IEncryptionService _encryptionService;
        private readonly ReactiveProperty<string> _connectionErrorDescription = new();

        private IDisposable _disposable;
        
        public ReadOnlyReactiveProperty<string> ConnectionErrorDescription => _connectionErrorDescription;
        
        public ConnectionService(IScreenService screenService, IEncryptionService encryptionService, SmartFox sfs)
        {
            _sfs = sfs;
            _encryptionService = encryptionService;
            _screenService = screenService;
        }
        
        public void Tick()
        {
            _sfs?.ProcessEvents();
        }
        
        public void Initialize()
        {
            _disposable = _connectionErrorDescription
                .Skip(1)
                .Subscribe(async _ => await _screenService.OpenAsync<ConnectionErrorScreen>());
            
            Connection().Forget();

            Application.quitting += ApplicationQuit;
            Application.runInBackground = true;
        }
        
        public void Dispose()
        {
            Application.quitting -= ApplicationQuit;
            _disposable.Dispose();
        }

        private void ApplicationQuit()
        {
            if(_sfs is { IsConnected: true }) 
                _sfs.Disconnect();
        }

        private async UniTaskVoid Connection()
        {
            _sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
            _sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
            
            var gameServerData = await ConnectionToWebApi(SFSResponseHelper.SERVER_URL);

            var configData = new ConfigData
            {
                Host = gameServerData.Host,
                Port = gameServerData.Port,
                Zone = gameServerData.Zone,
                UdpHost = gameServerData.Host,
                UdpPort = gameServerData.Port
            };
            
            _sfs.Connect(configData);
        }

        private async UniTask<GameServerData> ConnectionToWebApi(string url)
        {
            using var client = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            client.downloadHandler = new DownloadHandlerBuffer();

            try
            {
                var operation = client.SendWebRequest();
                
                while (!operation.isDone)
                {
                    await UniTask.Yield();
                }
                
                switch (client.result)
                {
                    case UnityWebRequest.Result.Success:
                        var decrypted = _encryptionService.Decrypt(client.downloadHandler.text);
                        return JsonConvert.DeserializeObject<GameServerData>(decrypted);

                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                    case UnityWebRequest.Result.ProtocolError:
                        _connectionErrorDescription.Value = $"Error while connecting to {url}: {client.error}";
                        break;
                }
            }
            catch (Exception ex)
            {
                _connectionErrorDescription.Value = $"Exception while connecting to {url}: {ex.Message}";
            }

            return null;
        }
        
        private void OnConnectionLost(BaseEvent evt)
        {
            _connectionErrorDescription.Value = "Connection Lost; is reason is: " + (string)evt.Params["reason"];
            ResetConnect();
        }

        private void OnConnection(BaseEvent evt)
        {
            if ((bool)evt.Params["success"])
            {
                Debug.Log("Connection established successfully");
                Debug.Log("SFS2X API version: " + _sfs.Version);
                
                _sfs.Send(new LoginRequest("", "", "GuestZone"));
            }
            else
            {
                _connectionErrorDescription.Value = "Connection failed; is the server running at all?";
                ResetConnect();
            }
        }

        private void ResetConnect()
        {
            _sfs.RemoveEventListener(SFSEvent.CONNECTION, OnConnection);
            _sfs.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);

            _sfs = null;
        }
    }
}
