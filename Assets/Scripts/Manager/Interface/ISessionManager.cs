using R3;
using Sfs2X.Entities;
using Utils;

namespace Manager.Interface
{
    public interface ISessionManager
    {
        Observable<ERoomRole> MyRole { get; }
        T RoomVariables<T>(string keyVariable);
        T UserVariables<T>(string keyVariable);
        void SetRole(ERoomRole role);
        ERoomRole GetRole();
        int FindUserIdByName(string name);
    }
}