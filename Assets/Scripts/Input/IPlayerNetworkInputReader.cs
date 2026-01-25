using R3;
using UnityEngine;

namespace Input
{
    public interface IPlayerNetworkInputReader
    {
        ReadOnlyReactiveProperty<Vector3> Movement { get; }
        ReadOnlyReactiveProperty<bool> Jump { get; }
        ReadOnlyReactiveProperty<bool> Run { get; }
        ReadOnlyReactiveProperty<Vector2> Look { get; }
        ReadOnlyReactiveProperty<bool> Aim  { get; }
        ReadOnlyReactiveProperty<bool> Shoot { get; }
        
        Vector3 Origin { get; }
        Vector3 Direction { get; }
    }
}