using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Utils
{
    [Serializable]
    public class AddressablePrefabByType<TFilter> where TFilter : class
    {
        [Header("Type")][HideLabel][SerializeField][ValueDropdown(nameof(GetTypes))] private string type;

        [Header("Asset")][HideLabel][SerializeField] private AssetReferenceGameObject asset;
        
        public Type Type => TypeExtensions.GetType(type);
        public AssetReferenceGameObject Asset => asset;

#if UNITY_EDITOR

        [UsedImplicitly]
        private IEnumerable<string> GetTypes() => TypeExtensions.FilterTypes<TFilter>();

#endif
    }
}