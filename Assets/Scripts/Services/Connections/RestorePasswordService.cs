using Helpers;
using R3;
using Services.Interface;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;

namespace Services.Connections
{
    public class RestorePasswordService : IRestorePasswordService
    {
        private SmartFox _sfs;
        private readonly ReactiveProperty<string> _restoreResult = new();
        private ISFSObject _requestObject;
        
        public ReadOnlyReactiveProperty<string> RestoreResult => _restoreResult;
        
        public RestorePasswordService(SmartFox sfs)
        {
            _sfs = sfs;
        }
        
        public void RestorePassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                _restoreResult.Value = "Email, is empty";
                return;
            }
            
            _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnRestorePassword);
            _sfs.AddEventListener(SFSEvent.LOGOUT, OnLogout);
            _sfs.AddEventListener(SFSEvent.LOGIN, OnSuccessLogin);
            
            _requestObject= SFSObject.NewInstance();
            _requestObject.PutUtfString(SFSResponseHelper.USER_EMAIL, email);
            
            _sfs.Send(new LogoutRequest());
        }

        private void OnSuccessLogin(BaseEvent evt)
        {
            _sfs.Send(new ExtensionRequest(SFSResponseHelper.RESTORE_RESULT, _requestObject));
        }

        private void OnLogout(BaseEvent evt)
        {
            _sfs.Send(new LoginRequest("", "", "GuestZone"));
        }

        private void OnRestorePassword(BaseEvent evt)
        {
            if (evt.Params["cmd"] as string != SFSResponseHelper.RESTORE_RESULT)
                return;
            
            var data = (ISFSObject) evt.Params["params"];
            
            var status = data.GetBool(SFSResponseHelper.OK);
            
            if (!status)
            {
                _restoreResult.Value = data.GetUtfString(SFSResponseHelper.ERROR);
            }
            else
            {
                _restoreResult.Value = "Send email " + data.GetUtfString(SFSResponseHelper.USER_EMAIL);
            }
            
            _sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, OnRestorePassword);
            _sfs.RemoveEventListener(SFSEvent.LOGOUT, OnLogout);
            _sfs.RemoveEventListener(SFSEvent.LOGIN, OnSuccessLogin);
        }
    }
}