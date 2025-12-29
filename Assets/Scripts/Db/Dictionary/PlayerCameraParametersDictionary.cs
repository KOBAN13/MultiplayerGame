using System;
using Utils.Enums;
using Utils.SerializedDictionary;

namespace Db.Dictionary
{
    [Serializable]
    public class PlayerCameraParametersDictionary
        : UnitySerializedDictionaryBase<EVirtualCameraType, PlayerCamerasParameters>
    {
    }
}