using Helpers;
using R3;
using Services.Interface;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;

namespace Services.Connections
{
    public class LoginClientService : ILoginClientService
    {
        private User _user;
        private string _login;
        private string _password;

        private readonly SmartFox _sfs;
        private readonly ReactiveProperty<string> _loginErrorRequest = new();
        private readonly ReactiveProperty<string> _userName = new();
        private readonly Subject<Unit> _successLogin = new();
        private readonly LogoutRequest _logoutRequest = new();
        
        public ReadOnlyReactiveProperty<string> LoginErrorRequest => _loginErrorRequest;
        public ReadOnlyReactiveProperty<string> UserName => _userName;
        public Observable<Unit> SuccessLogin => _successLogin.AsObservable();
        
        public LoginClientService(SmartFox sfs)
        {
            _sfs = sfs;
        }

        public bool IsUserLogin()
        {
            return _sfs.MySelf is { PrivilegeId: >= 1 };
        }

        public void Login(string login, string password)
        {
            _sfs.AddEventListener(SFSEvent.LOGIN, OnSuccessLogin);
            _sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginResult);
            _sfs.AddEventListener(SFSEvent.LOGOUT, OnLogout);
            
            _sfs.Send(_logoutRequest);
            
            _login = login;
            _password = password;
        }

        private void OnLogout(BaseEvent evt)
        {
            _sfs.Send(new LoginRequest(_login, _password, "GameZone"));
        }

        private void OnLoginResult(BaseEvent evt)
        {
            var status = evt.Params["errorMessage"] as string;
            
            _loginErrorRequest.Value = status;
            
            Debug.LogError("Result: " + status);
        }
        
        private void OnSuccessLogin(BaseEvent evt)
        {
            _user = evt.Params["user"] as User;
            
            var data = (ISFSObject) evt.Params["data"];
            
            var userId = data.GetLong(SFSResponseHelper.USER_ID);
            var username = data.GetUtfString(SFSResponseHelper.USER_NAME);
            var email = data.GetUtfString(SFSResponseHelper.USER_EMAIL);
            
            var sfsObject = SFSObject.NewInstance();
            
            sfsObject.PutShort("privilege", 1);
            
            _sfs.Send(new ExtensionRequest(SFSResponseHelper.CHANGE_USER_PRIVILEGE, sfsObject));
            
            _userName.Value = username;
            
            _successLogin.OnNext(Unit.Default);
            _sfs.InitUDP();
            
            _sfs.RemoveEventListener(SFSEvent.LOGIN, OnSuccessLogin);
            _sfs.RemoveEventListener(SFSEvent.LOGIN_ERROR, OnLoginResult);
            _sfs.RemoveEventListener(SFSEvent.LOGOUT, OnLogout);
        }
    }
}