using System.Collections.Generic;
using Sirenix.OdinInspector;
using UI.Base;
using UnityEngine;
using Utils;

namespace Db
{
    [CreateAssetMenu(fileName = "SceneConfig", menuName = "Db/SceneConfig")]
    public class ScreensData : SerializedScriptableObject
    {
        [field: SerializeField] public Transform Root { get; private set; }
        
        [SerializeField] private List<AddressablePrefabByType<View>> _screens;

        public IReadOnlyList<AddressablePrefabByType<View>> Screens => _screens;
    }
}