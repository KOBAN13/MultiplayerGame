using R3;
using UnityEngine;

namespace Input
{
    public interface IInputSource
    {
        ReactiveCommand<bool> AimCommand { get; }
        ReactiveCommand<bool> ShotCommand { get; }
        InputFrame ReadInputFrame(float rotationCameraY, Vector3 aimDirection, float aimPitch);
        WeaponInputFrame ReadWeaponInput();
    }
}
