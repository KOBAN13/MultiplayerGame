using System;

namespace Services.SceneManagement
{
    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> Progressed;
        private readonly float _ratio = 1f;
        
        public void Report(float value)
        {
            Progressed?.Invoke(value / _ratio);
        }
    }
}