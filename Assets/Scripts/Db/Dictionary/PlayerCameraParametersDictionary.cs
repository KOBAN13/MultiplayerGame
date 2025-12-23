using System;
using Player.Camera;
using Utils.SerializedDictionary;

namespace Db.Dictionary
{
    [Serializable]
    public class PlayerCameraParametersDictionary
        : UnitySerializedDictionaryBase<EVirtualCameraType, PlayerCamerasParameters>
    {
    }
}