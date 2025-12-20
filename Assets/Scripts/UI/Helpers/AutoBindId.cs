using UnityEngine;
using System;

namespace UI.Helpers
{
    [Serializable]
    public class AutoBindId
    {
        [SerializeField] 
        private string _generatedGuid;

        public string GeneratedGuid => _generatedGuid;
        
        public void ClearGuid() => _generatedGuid = string.Empty;
        
        public void SetGuid(string guid) => _generatedGuid = guid;

        public void EnsureGuid()
        {
            if (string.IsNullOrEmpty(_generatedGuid))
                _generatedGuid = Guid.NewGuid().ToString("N");
        }
    }
}