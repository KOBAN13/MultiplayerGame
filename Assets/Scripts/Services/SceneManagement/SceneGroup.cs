using System;
using System.Collections.Generic;
using System.Linq;
using Eflatun.SceneReference;
using Services.SceneManagement.Enums;
using UnityEngine;

namespace Services.SceneManagement
{
    [Serializable]
    public class SceneGroup
    {
        [field: SerializeField] public List<SceneData> Scenes { get; private set; }

        public string FindSceneByName(TypeScene sceneType) =>
            Scenes.FirstOrDefault(scene => scene.typeScene == sceneType).Name;
        
        public SceneReference FindSceneByReference(TypeScene sceneType) =>
            Scenes.FirstOrDefault(scene => scene.typeScene == sceneType).scene;
    }
}