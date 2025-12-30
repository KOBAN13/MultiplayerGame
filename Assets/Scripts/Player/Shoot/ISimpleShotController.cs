using UnityEngine;

namespace Player.Shoot
{
    public interface ISimpleShotController
    {
        void Shot(Transform shotPoint);
    }
}