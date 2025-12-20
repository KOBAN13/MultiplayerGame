using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services.SceneManagement
{
    public class SceneResources
    {
        private readonly List<GameObject> _objectsToRelease = new();
        public IReadOnlyList<GameObject> ObjectToRelease => _objectsToRelease;

        public void AddObjectToRelease(GameObject action) => _objectsToRelease.Add(action);

        public void ClearObject() => _objectsToRelease.Clear();
    }
}