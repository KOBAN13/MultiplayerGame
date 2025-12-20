using Helpers;
using R3;
using Services.Interface;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using VContainer;

namespace Services.Connections
{
    public class RegistrationService : IRegistrationService
    {
        private SmartFox _sfs;
        
        private readonly ReactiveProperty<string> _registerError = new();
        private readonly Subject<Unit> _successRegistration = new();
        
        public ReadOnlyReactiveProperty<string> RegisterError => _registerError;
        public Observable<Unit> SuccessRegistration => _successRegistration.AsObservable();
        
        public RegistrationService(SmartFox sfs)
        {
            _sfs = sfs;
        }

        public void Register(string email, string login, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                _registerError.Value = "Email, login or password is empty";
                return;
            }
            
            _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnRegisterResult);
            
            var requestObject = SFSObject.NewInstance();
            requestObject.PutUtfString(SFSResponseHelper.USER_EMAIL, email);
            requestObject.PutUtfString(SFSResponseHelper.USER_NAME, login);
            requestObject.PutUtfString(SFSResponseHelper.PASSWORD, password);
            
            _sfs.Send(new ExtensionRequest(SFSResponseHelper.REGISTER_RESULT, requestObject));
        }
        
        private void OnRegisterResult(BaseEvent evt)
        {
            var cmd = evt.Params[SFSResponseHelper.CMD] as string;
            var data = (ISFSObject) evt.Params["params"];

            if (cmd != SFSResponseHelper.REGISTER_RESULT) 
                return;
            
            var status = data.GetBool(SFSResponseHelper.OK);
            
            if (!status)
            {
                _registerError.Value = data.GetUtfString(SFSResponseHelper.ERROR);
            }
            else
            {
                Debug.LogError("Register success! " + data.GetLong(SFSResponseHelper.USER_ID));
                _successRegistration.OnNext(Unit.Default);
            }
            
            _sfs.RemoveEventListener(SFSResponseHelper.REGISTER_RESULT, OnRegisterResult);
        }
    }
}