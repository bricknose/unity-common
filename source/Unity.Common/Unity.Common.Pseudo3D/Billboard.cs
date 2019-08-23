using UnityEditor;
using UnityEngine;

namespace Unity.Common.Pseudo3D
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Billboard : MonoBehaviour
    {
        private Animator _animator;
        private float _maxMirrorAngle;
        private float _minMirrorAngle;
        private SpriteRenderer _spriteRenderer;

        public int Directions = 8;
        public bool MirrorLeft = true;
        public Camera MainCamera;
        public string AnimatorAngleVariable = "ViewAngle";

        public void Start()
        {
            GetDependencies();
            InitializeBillboard();
        }

        public void Awake()
        {
            UpdateMainCamera();
        }

        public void Update()
        {
            var viewDirection = GetBillboardTargetDirection(MainCamera.transform);
            FaceBillboardTowardTarget(viewDirection);

            var billboardAngle = GetBillboardViewAngle();
            UpdateAnimatorViewAngle(billboardAngle);

            if (MirrorLeft)
            {
                UpdateSpriteMirroring(billboardAngle);
            }
        }

        public void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                // Editor camera stands in for player camera in edit mode
                FaceBillboardTowardSceneCamera();
            }

            DrawBillboardDirectionRay();
        }

        private void GetDependencies()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
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

        private void UpdateMainCamera()
        {
            if (MainCamera == null)
            {
                MainCamera = Camera.main;
            }
        }

        private void UpdateSpriteMirroring(float billboardAngle)
        {
            _spriteRenderer.flipX = !(billboardAngle >= _minMirrorAngle &&
                                      billboardAngle <= _maxMirrorAngle);
        }

        private void UpdateAnimatorViewAngle(float billboardAngle)
        {
            _animator.SetFloat(AnimatorAngleVariable, billboardAngle);
        }

        private float GetBillboardViewAngle()
        {
            return transform.localEulerAngles.y;
        }

        private static Vector3 GetBillboardTargetDirection(Transform cameraTransform)
        {
            return -new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z);
        }

        private void FaceBillboardTowardTarget(Vector3 viewDirection)
        {
            transform.LookAt(transform.position + viewDirection);
        }

        private void DrawBillboardDirectionRay()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.parent.position, transform.parent.forward * 2);
        }

        private void FaceBillboardTowardSceneCamera()
        {
            var sceneView = GetActiveSceneView();
            if (sceneView)
            {
                var viewDirection = GetBillboardTargetDirection(sceneView.camera.transform);
                FaceBillboardTowardTarget(viewDirection);
            }
        }

        private static SceneView GetActiveSceneView()
        {
            return EditorWindow.focusedWindow as SceneView;
        }
    }
}