#if UNITY_EDITOR
using ParrelSync;
using System;
using UnityEngine;

namespace Utils
{
    public class ParrelSyncRuntime
    {
        private const string AutoLeftArgument = "autoMovement";

        private static bool? _isClone;
        private static string _argument;

        public bool IsAutoLeftClone()
        {
            if (!ClonesManager.IsClone())
            {
                return false;
            }

            var argument = ClonesManager.GetArgument();
            
            Debug.Log(argument);
            
            return string.Equals(argument, AutoLeftArgument, StringComparison.OrdinalIgnoreCase);
        }

        public string GetLogin()
        {
            return ClonesManager.IsClone() ? "Daniil" : "Koban";
        }
        
        public string GetPassword()
        {
            return ClonesManager.IsClone() ? "giftrola05" : "giftrola05";
        }
    }
}
#endif
