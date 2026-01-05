using UnityEngine;

namespace Player.Weapon
{
    public class WeaponHandIK : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _rightHandTarget;
        [SerializeField] private Transform _leftHandTarget;
        [SerializeField, Range(0f, 1f)] private float _rightHandWeight = 1f;
        [SerializeField, Range(0f, 1f)] private float _leftHandWeight = 1f;
        [SerializeField, Range(0f, 1f)] private float _rightHandRotationWeight = 1f;
        [SerializeField, Range(0f, 1f)] private float _leftHandRotationWeight = 1f;
        [SerializeField] private bool _isActive = true;

        public void SetTargets(Transform rightHandTarget, Transform leftHandTarget)
        {
            _rightHandTarget = rightHandTarget;
            _leftHandTarget = leftHandTarget;
        }

        public void SetActive(bool isActive)
        {
            _isActive = isActive;
        }

        private void Reset()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnValidate()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (_animator == null)
            {
                return;
            }

            ApplyHandIK(AvatarIKGoal.RightHand, _rightHandTarget, _rightHandWeight, _rightHandRotationWeight);
            ApplyHandIK(AvatarIKGoal.LeftHand, _leftHandTarget, _leftHandWeight, _leftHandRotationWeight);
        }

        private void ApplyHandIK(AvatarIKGoal goal, Transform target, float positionWeight, float rotationWeight)
        {
            if (!_isActive || target == null)
            {
                _animator.SetIKPositionWeight(goal, 0f);
                _animator.SetIKRotationWeight(goal, 0f);
                return;
            }

            _animator.SetIKPositionWeight(goal, positionWeight);
            _animator.SetIKRotationWeight(goal, rotationWeight);
            _animator.SetIKPosition(goal, target.position);
            _animator.SetIKRotation(goal, target.rotation);
        }
    }
}
