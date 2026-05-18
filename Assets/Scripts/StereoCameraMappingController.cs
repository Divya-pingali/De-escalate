using System.Collections;
using Meta.XR;
using UnityEngine;

namespace QuestCameraKit.CameraMapping
{
    public class StereoCameraMappingController : MonoBehaviour
    {
        public enum ShaderMode
        {
            Blur,
            Pixelate,
            Block
        }

        [SerializeField] private PassthroughCameraAccess leftCameraAccess;
        [SerializeField] private PassthroughCameraAccess rightCameraAccess;

        [Header("Shader Materials")]
        [SerializeField] private Material blurMaterial;
        [SerializeField] private Material pixelateMaterial;
        [SerializeField] private Material blockMaterial;

        [Header("Per-Eye UV Offset")]
        [SerializeField, Range(-0.2f, 0f)] private float leftUvOffsetX;
        [SerializeField, Range(-0.2f, 0f)] private float leftUvOffsetY;
        [SerializeField, Range(-0.2f, 0f)] private float rightUvOffsetX;
        [SerializeField, Range(-0.2f, 0f)] private float rightUvOffsetY;

        private Material _activeMaterial;

        private static readonly int LeftTexId = Shader.PropertyToID("_LeftTex");
        private static readonly int RightTexId = Shader.PropertyToID("_RightTex");

        private static readonly int LeftCameraPosId = Shader.PropertyToID("_LeftCameraPos");
        private static readonly int RightCameraPosId = Shader.PropertyToID("_RightCameraPos");
        private static readonly int LeftCameraRotationMatrixId = Shader.PropertyToID("_LeftCameraRotationMatrix");
        private static readonly int RightCameraRotationMatrixId = Shader.PropertyToID("_RightCameraRotationMatrix");

        private static readonly int LeftFocalLengthId = Shader.PropertyToID("_LeftFocalLength");
        private static readonly int RightFocalLengthId = Shader.PropertyToID("_RightFocalLength");
        private static readonly int LeftPrincipalPointId = Shader.PropertyToID("_LeftPrincipalPoint");
        private static readonly int RightPrincipalPointId = Shader.PropertyToID("_RightPrincipalPoint");

        private static readonly int LeftSensorResolutionId = Shader.PropertyToID("_LeftSensorResolution");
        private static readonly int RightSensorResolutionId = Shader.PropertyToID("_RightSensorResolution");
        private static readonly int LeftCurrentResolutionId = Shader.PropertyToID("_LeftCurrentResolution");
        private static readonly int RightCurrentResolutionId = Shader.PropertyToID("_RightCurrentResolution");

        private static readonly int LeftUvOffsetId = Shader.PropertyToID("_LeftUvOffset");
        private static readonly int RightUvOffsetId = Shader.PropertyToID("_RightUvOffset");

        private IEnumerator Start()
        {
            leftCameraAccess = ResolveCamera(leftCameraAccess, PassthroughCameraAccess.CameraPositionType.Left);
            rightCameraAccess = ResolveCamera(rightCameraAccess, PassthroughCameraAccess.CameraPositionType.Right);

            if (!leftCameraAccess || !rightCameraAccess)
            {
                Debug.LogError("[StereoCameraMappingController] Left/Right PassthroughCameraAccess components are required.");
                yield break;
            }

            _activeMaterial = pixelateMaterial;

            yield return new WaitUntil(() => leftCameraAccess.IsPlaying && rightCameraAccess.IsPlaying);

            ApplyCurrentMaterialData();
        }

        private void Update()
        {
            if (!_activeMaterial || !leftCameraAccess || !rightCameraAccess)
                return;

            if (!leftCameraAccess.IsPlaying || !rightCameraAccess.IsPlaying)
                return;

            ApplyCurrentMaterialData();
        }

        public void SetShaderMode(ShaderMode mode)
        {
            Material selected = mode switch
            {
                ShaderMode.Blur => blurMaterial,
                ShaderMode.Pixelate => pixelateMaterial,
                ShaderMode.Block => blockMaterial,
                _ => pixelateMaterial
            };

            SetTargetMaterial(selected);
        }

        public void SetTargetMaterial(Material material)
        {
            if (!material)
            {
                Debug.LogWarning("[StereoCameraMappingController] Ignored null material assignment.");
                return;
            }

            _activeMaterial = material;
            ApplyCurrentMaterialData();
        }

        private void ApplyCurrentMaterialData()
        {
            if (!_activeMaterial || !leftCameraAccess || !rightCameraAccess)
                return;

            Texture leftTexture = leftCameraAccess.GetTexture();
            if (leftTexture)
                _activeMaterial.SetTexture(LeftTexId, leftTexture);

            Texture rightTexture = rightCameraAccess.GetTexture();
            if (rightTexture)
                _activeMaterial.SetTexture(RightTexId, rightTexture);

            ApplyCalibrationToMaterial();
            UpdateEyeData(leftCameraAccess, true);
            UpdateEyeData(rightCameraAccess, false);
        }

        private void UpdateEyeData(PassthroughCameraAccess cameraAccess, bool leftEye)
        {
            var pose = cameraAccess.GetCameraPose();
            var intrinsics = cameraAccess.Intrinsics;
            var currentResolution = cameraAccess.CurrentResolution;

            _activeMaterial.SetVector(
                leftEye ? LeftCameraPosId : RightCameraPosId,
                pose.position
            );

            _activeMaterial.SetMatrix(
                leftEye ? LeftCameraRotationMatrixId : RightCameraRotationMatrixId,
                Matrix4x4.Rotate(Quaternion.Inverse(pose.rotation))
            );

            _activeMaterial.SetVector(
                leftEye ? LeftFocalLengthId : RightFocalLengthId,
                intrinsics.FocalLength
            );

            _activeMaterial.SetVector(
                leftEye ? LeftPrincipalPointId : RightPrincipalPointId,
                intrinsics.PrincipalPoint
            );

            _activeMaterial.SetVector(
                leftEye ? LeftSensorResolutionId : RightSensorResolutionId,
                new Vector4(intrinsics.SensorResolution.x, intrinsics.SensorResolution.y, 0f, 0f)
            );

            _activeMaterial.SetVector(
                leftEye ? LeftCurrentResolutionId : RightCurrentResolutionId,
                new Vector4(currentResolution.x, currentResolution.y, 0f, 0f)
            );
        }

        private void ApplyCalibrationToMaterial()
        {
            _activeMaterial.SetVector(LeftUvOffsetId, new Vector2(leftUvOffsetX, leftUvOffsetY));
            _activeMaterial.SetVector(RightUvOffsetId, new Vector2(rightUvOffsetX, rightUvOffsetY));
        }

        private static PassthroughCameraAccess ResolveCamera(
            PassthroughCameraAccess configuredAccess,
            PassthroughCameraAccess.CameraPositionType cameraPosition)
        {
            if (configuredAccess && configuredAccess.CameraPosition == cameraPosition)
                return configuredAccess;

            var allCameras = FindObjectsByType<PassthroughCameraAccess>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

            foreach (var cameraAccess in allCameras)
            {
                if (cameraAccess && cameraAccess.CameraPosition == cameraPosition)
                    return cameraAccess;
            }

            return null;
        }
    }
}