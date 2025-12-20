using System;
using Eflatun.SceneReference;
using Services.SceneManagement.Enums;

namespace Services.SceneManagement
{
    [Serializable]
    public struct SceneData
    {
        public SceneReference scene;
        public TypeScene typeScene;
        public string Name => scene.Name;
    }
}