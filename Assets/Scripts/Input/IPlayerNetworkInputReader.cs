using R3;
using UnityEngine;

namespace Input
{
    public interface IPlayerNetworkInputReader
    {
        ReadOnlyReactiveProperty<Vector3> Movement { get; }
        ReadOnlyReactiveProperty<bool> Jump { get; }
        ReadOnlyReactiveProperty<bool> Run { get; }
        public ReadOnlyReactiveProperty<Vector2> Look { get; }
        public ReadOnlyReactiveProperty<bool> Aim  { get; }
        public ReadOnlyReactiveProperty<bool> Shoot { get; }
    }
}