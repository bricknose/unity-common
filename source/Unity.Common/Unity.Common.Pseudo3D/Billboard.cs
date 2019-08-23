using UnityEditor;
using UnityEngine;

namespace Unity.Common.Pseudo3D
{
    public class Billboard : MonoBehaviour
    {
        public delegate void BillboardAngleUpdated(float newBillboardAngle);

        public event BillboardAngleUpdated BillboardAngleUpdatedEvent;

        public Camera MainCamera;

        public void Awake()
        {
            UpdateMainCamera();
        }

        public virtual void Update()
        {
            var viewDirection = GetBillboardTargetDirection(MainCamera.transform);
            FaceBillboardTowardTarget(viewDirection);

            var billboardAngle = GetBillboardViewAngle();
            BillboardAngleUpdatedEvent?.Invoke(billboardAngle);
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

        private void UpdateMainCamera()
        {
            if (MainCamera == null)
            {
                MainCamera = Camera.main;
            }
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