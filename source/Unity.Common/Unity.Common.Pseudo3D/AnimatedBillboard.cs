using UnityEngine;

namespace Unity.Common.Pseudo3D
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Billboard))]
    public class AnimatedBillboard : MonoBehaviour
    {
        private Animator _animator;
        private Billboard _billboard;
        private float _maxMirrorAngle;
        private float _minMirrorAngle;
        private SpriteRenderer _spriteRenderer;

        public int Directions = 8;
        public bool MirrorLeft = true;
        public string AnimatorAngleVariable = "ViewAngle";

        public void Start()
        {
            GetDependencies();
            InitializeBillboard();
        }

        public void OnDestroy()
        {
            CleanUpDependencies();
        }

        private void GetDependencies()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            _billboard = GetComponent<Billboard>();

            _billboard.BillboardAngleUpdatedEvent += AngleUpdatedSpriteMirroring;
            _billboard.BillboardAngleUpdatedEvent += AngleUpdatedAnimatorViewAngle;
        }

        private void InitializeBillboard()
        {
            if (Directions <= 0)
            {
                Directions = 1;
            }

            _minMirrorAngle = 360f / 2 / Directions;
            _maxMirrorAngle = 180f - _minMirrorAngle;
        }

        private void CleanUpDependencies()
        {
            _billboard.BillboardAngleUpdatedEvent -= AngleUpdatedSpriteMirroring;
            _billboard.BillboardAngleUpdatedEvent -= AngleUpdatedAnimatorViewAngle;
        }

        private void AngleUpdatedSpriteMirroring(float billboardAngle)
        {
            if (MirrorLeft)
            {
                _spriteRenderer.flipX = !(billboardAngle >= _minMirrorAngle &&
                                          billboardAngle <= _maxMirrorAngle);
            }
        }

        private void AngleUpdatedAnimatorViewAngle(float billboardAngle)
        {
            _animator.SetFloat(AnimatorAngleVariable, billboardAngle);
        }
    }
}