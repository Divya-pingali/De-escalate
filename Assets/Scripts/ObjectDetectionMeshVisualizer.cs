using System.Buffers;
using System.Collections.Generic;
using Meta.XR.EnvironmentDepth;
using Unity.Collections;
using UnityEngine;

namespace Meta.XR.BuildingBlocks.AIBlocks
{
#if MRUK_INSTALLED
    [RequireComponent(typeof(ObjectDetectionAgent), typeof(DepthTextureAccess), typeof(EnvironmentDepthManager))]
#endif
    public class ObjectDetectionMeshVisualizer : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private GameObject boundingBoxPrefab;

        [Header("Prefab Child Names")]
        [SerializeField] private string anchorChildName = "AnchorCube";
        [SerializeField] private string overlayChildName = "OverlayObject";

        [Header("Visibility")]
        [SerializeField] private bool showBoundingBoxes = true;
        [SerializeField] private bool showText = true;

        [Header("Sizing")]
        [SerializeField] private float anchorDepthMultiplier = 1f;
        [SerializeField] private float overlayUniformScaleMultiplier = 1f;
        [SerializeField] private float labelYOffset = 0.02f;

        private float[] _depthBuf;
        private Matrix4x4[] _vpBuf;

        private ObjectDetectionAgent _agent;

        private readonly List<GameObject> _liveBoxes = new();
        private readonly List<GameObject> _liveLabels = new();

        private readonly Queue<GameObject> _boxPool = new();
        private readonly Queue<GameObject> _labelPool = new();

        public bool ShowBoundingBoxes
        {
            get => showBoundingBoxes;
            set
            {
                if (showBoundingBoxes == value) return;
                showBoundingBoxes = value;

                foreach (var g in _liveBoxes)
                {
                    if (!g) continue;

                    var cache = g.GetComponent<VisualCache>();
                    if (cache == null) continue;

                    cache.EnsureInitialized(anchorChildName, overlayChildName);
                    foreach (var r in cache.renderers)
                    {
                        if (r) r.enabled = value;
                    }
                }
            }
        }

        public bool ShowText
        {
            get => showText;
            set
            {
                if (showText == value) return;
                showText = value;

                foreach (var g in _liveLabels)
                {
                    if (!g) continue;

                    if (g.TryGetComponent<Renderer>(out var renderer))
                    {
                        renderer.enabled = value;
                    }
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ShowBoundingBoxes = showBoundingBoxes;
            ShowText = showText;
        }
#endif

        public void SetShowBoundingBoxes(bool value) => ShowBoundingBoxes = value;
        public void SetShowText(bool value) => ShowText = value;

#if MRUK_INSTALLED
        private PassthroughCameraAccess _cam;
        private DepthTextureAccess _depth;
        private int _eyeIdx;

        private struct FrameData
        {
            public Pose Pose;
            public PassthroughCameraAccess.CameraIntrinsics CameraIntrinsics;
            public float[] Depth;
            public Matrix4x4[] ViewProjectionMatrix;
        }

        private FrameData _frame;
#endif

        private void Awake()
        {
            _agent = GetComponent<ObjectDetectionAgent>();

#if MRUK_INSTALLED
            _cam = FindAnyObjectByType<PassthroughCameraAccess>();
            _depth = GetComponent<DepthTextureAccess>();

            if (_cam != null)
            {
                _eyeIdx = _cam.CameraPosition == PassthroughCameraAccess.CameraPositionType.Left ? 0 : 1;
            }
#endif
        }

#if MRUK_INSTALLED
        private void OnEnable()
        {
            if (_agent != null)
            {
                _agent.OnBoxesUpdated += HandleBatch;
            }

            if (_depth != null)
            {
                _depth.OnDepthTextureUpdateCPU += OnDepth;
            }
        }

        private void OnDisable()
        {
            if (_agent != null)
            {
                _agent.OnBoxesUpdated -= HandleBatch;
            }

            if (_depth != null)
            {
                _depth.OnDepthTextureUpdateCPU -= OnDepth;
            }

            ReturnBuffers();
        }

        private void ReturnBuffers()
        {
            if (_depthBuf != null)
            {
                ArrayPool<float>.Shared.Return(_depthBuf, clearArray: true);
                _depthBuf = null;
            }

            if (_vpBuf != null)
            {
                ArrayPool<Matrix4x4>.Shared.Return(_vpBuf, clearArray: true);
                _vpBuf = null;
            }
        }

        private void OnDepth(DepthTextureAccess.DepthFrameData d)
        {
            if (_cam == null) return;

            _frame.Pose = d.CameraPose;
            _frame.CameraIntrinsics = _cam.Intrinsics;

            if (_depthBuf == null || _depthBuf.Length < d.DepthTexturePixels.Length)
            {
                if (_depthBuf != null)
                {
                    ArrayPool<float>.Shared.Return(_depthBuf);
                }

                _depthBuf = ArrayPool<float>.Shared.Rent(d.DepthTexturePixels.Length);
            }

            if (_vpBuf == null || _vpBuf.Length < d.ViewProjectionMatrix.Length)
            {
                if (_vpBuf != null)
                {
                    ArrayPool<Matrix4x4>.Shared.Return(_vpBuf);
                }

                _vpBuf = ArrayPool<Matrix4x4>.Shared.Rent(d.ViewProjectionMatrix.Length);
            }

            NativeArray<float>.Copy(d.DepthTexturePixels, _depthBuf, d.DepthTexturePixels.Length);
            System.Array.Copy(d.ViewProjectionMatrix, _vpBuf, d.ViewProjectionMatrix.Length);

            _frame.Depth = _depthBuf;
            _frame.ViewProjectionMatrix = _vpBuf;
        }
#endif

        private void HandleBatch(List<BoxData> batch)
        {
            foreach (var g in _liveBoxes)
            {
                if (!g) continue;
                g.SetActive(false);
                _boxPool.Enqueue(g);
            }

            foreach (var g in _liveLabels)
            {
                if (!g) continue;
                g.SetActive(false);
                _labelPool.Enqueue(g);
            }

            _liveBoxes.Clear();
            _liveLabels.Clear();

#if MRUK_INSTALLED
            if (!boundingBoxPrefab)
            {
                Debug.LogError("[ObjectDetectionMeshVisualizer] boundingBoxPrefab is null.");
                return;
            }

            if (_cam == null)
            {
                Debug.LogError("[ObjectDetectionMeshVisualizer] PassthroughCameraAccess not found.");
                return;
            }

            if (_depth == null)
            {
                Debug.LogError("[ObjectDetectionMeshVisualizer] DepthTextureAccess not found.");
                return;
            }

            foreach (var b in batch)
            {
                float xmin = b.position.x;
                float ymin = b.position.y;
                float xmax = b.scale.x;
                float ymax = b.scale.y;

                if (!TryProject(xmin, ymin, xmax, ymax, out var pos, out var rot, out var scl))
                {
                    continue;
                }

                var visual = GetBoxObject();
                ApplyVisualTransform(visual, pos, rot, scl);
                _liveBoxes.Add(visual);

                var lbl = GetLabelObject();
                var tm = lbl.GetComponent<TextMesh>();
                tm.text = b.label;

                if (lbl.TryGetComponent<Renderer>(out var textRenderer))
                {
                    textRenderer.enabled = showText;
                }

                lbl.transform.SetPositionAndRotation(pos + Vector3.up * labelYOffset, rot);
                _liveLabels.Add(lbl);

                // Uncomment for debugging:
                // Debug.Log($"Spawned {visual.name} at {visual.transform.position} with projected size {scl}");
            }
#endif
        }

        private GameObject GetBoxObject()
        {
            var go = _boxPool.Count > 0 ? _boxPool.Dequeue() : Instantiate(boundingBoxPrefab);
            go.SetActive(true);

            var cache = go.GetComponent<VisualCache>() ?? go.AddComponent<VisualCache>();
            cache.EnsureInitialized(anchorChildName, overlayChildName);

            foreach (var r in cache.renderers)
            {
                if (r) r.enabled = showBoundingBoxes;
            }

            return go;
        }

        private GameObject GetLabelObject()
        {
            var go = _labelPool.Count > 0 ? _labelPool.Dequeue() : new GameObject("Label");
            go.SetActive(true);

            var tm = go.GetComponent<TextMesh>() ?? go.AddComponent<TextMesh>();
            tm.fontSize = 24;
            tm.characterSize = 0.02f;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;

            if (go.TryGetComponent<Renderer>(out var r))
            {
                r.enabled = showText;
            }

            return go;
        }

        private void ApplyVisualTransform(GameObject go, Vector3 worldPos, Quaternion worldRot, Vector3 boxWorldSize)
        {
            var cache = go.GetComponent<VisualCache>() ?? go.AddComponent<VisualCache>();
            cache.EnsureInitialized(anchorChildName, overlayChildName);

            go.transform.SetPositionAndRotation(worldPos, worldRot);
            go.transform.localScale = Vector3.one;

            float boxDepth = Mathf.Max(0.001f, Mathf.Min(boxWorldSize.x, boxWorldSize.y) * anchorDepthMultiplier);
            Vector3 anchorScale = new Vector3(boxWorldSize.x, boxWorldSize.y, boxDepth);

            if (cache.anchorChild != null)
            {
                cache.anchorChild.localScale = Vector3.Scale(cache.anchorBaseLocalScale, anchorScale);
            }
            else
            {
                // Fallback if no AnchorCube child exists
                go.transform.localScale = anchorScale;
            }

            if (cache.overlayChild != null)
            {
                float uniformScale = Mathf.Min(boxWorldSize.x, boxWorldSize.y) * overlayUniformScaleMultiplier;
                cache.overlayChild.localScale = cache.overlayBaseLocalScale * uniformScale;
            }
        }

        private static Transform FindChildRecursive(Transform root, string childName)
        {
            if (root == null || string.IsNullOrWhiteSpace(childName)) return null;

            var all = root.GetComponentsInChildren<Transform>(true);
            foreach (var t in all)
            {
                if (t.name == childName)
                {
                    return t;
                }
            }

            return null;
        }

#if MRUK_INSTALLED
        private bool TryProject(
            float xmin,
            float ymin,
            float xmax,
            float ymax,
            out Vector3 world,
            out Quaternion rot,
            out Vector3 scale)
        {
            world = default;
            rot = default;
            scale = default;

            if (_cam == null || _depth == null)
            {
                return false;
            }

            var cameraTexture = _cam.GetTexture();
            if (cameraTexture == null)
            {
                return false;
            }

            if (_frame.Depth == null || _frame.ViewProjectionMatrix == null)
            {
                return false;
            }

            float px = (xmin + xmax) * 0.5f;
            float py = (ymin + ymax) * 0.5f;

            float normalizedCenterX = px / cameraTexture.width;
            float normalizedCenterY = py / cameraTexture.height;

            var ray = _cam.ViewportPointToRay(
                new Vector2(normalizedCenterX, 1.0f - normalizedCenterY),
                _frame.Pose);

            var world1M = ray.origin + ray.direction;
            var clip = _frame.ViewProjectionMatrix[_eyeIdx] * new Vector4(world1M.x, world1M.y, world1M.z, 1f);
            if (clip.w <= 0)
            {
                return false;
            }

            var uv = (new Vector2(clip.x, clip.y) / clip.w) * 0.5f + Vector2.one * 0.5f;

            if (!_depth.IsInitialized)
            {
                return false;
            }

            int texSize = _depth.TextureSize;
            int sx = Mathf.Clamp((int)(uv.x * texSize), 0, texSize - 1);
            int sy = Mathf.Clamp((int)(uv.y * texSize), 0, texSize - 1);
            int idx = _eyeIdx * texSize * texSize + sy * texSize + sx;

            if (idx < 0 || idx >= _frame.Depth.Length)
            {
                return false;
            }

            float d = _frame.Depth[idx];
            if (d <= 0 || d > 20 || float.IsInfinity(d) || float.IsNaN(d))
            {
                return false;
            }

            world = ray.origin + ray.direction * d;
            rot = Quaternion.LookRotation(world - _frame.Pose.position);

            float normalizedWidth = (xmax - xmin) / cameraTexture.width;
            float normalizedHeight = (ymax - ymin) / cameraTexture.height;

            var rayLeft = _cam.ViewportPointToRay(
                new Vector2(normalizedCenterX - normalizedWidth * 0.5f, 1.0f - normalizedCenterY),
                _frame.Pose);

            var rayRight = _cam.ViewportPointToRay(
                new Vector2(normalizedCenterX + normalizedWidth * 0.5f, 1.0f - normalizedCenterY),
                _frame.Pose);

            var rayTop = _cam.ViewportPointToRay(
                new Vector2(normalizedCenterX, 1.0f - (normalizedCenterY - normalizedHeight * 0.5f)),
                _frame.Pose);

            var rayBottom = _cam.ViewportPointToRay(
                new Vector2(normalizedCenterX, 1.0f - (normalizedCenterY + normalizedHeight * 0.5f)),
                _frame.Pose);

            var worldLeft = rayLeft.origin + rayLeft.direction * d;
            var worldRight = rayRight.origin + rayRight.direction * d;
            var worldTop = rayTop.origin + rayTop.direction * d;
            var worldBottom = rayBottom.origin + rayBottom.direction * d;

            float w = Vector3.Distance(worldLeft, worldRight);
            float h = Vector3.Distance(worldTop, worldBottom);

            scale = new Vector3(w, h, 1f);
            return true;
        }
#endif

        private sealed class VisualCache : MonoBehaviour
        {
            public Renderer[] renderers;

            public Transform anchorChild;
            public Transform overlayChild;

            public Vector3 anchorBaseLocalScale = Vector3.one;
            public Vector3 overlayBaseLocalScale = Vector3.one;

            private bool _initialized;
            private string _lastAnchorName;
            private string _lastOverlayName;

            public void EnsureInitialized(string anchorName, string overlayName)
            {
                if (_initialized && _lastAnchorName == anchorName && _lastOverlayName == overlayName)
                {
                    return;
                }

                renderers = GetComponentsInChildren<Renderer>(true);

                anchorChild = FindChildRecursive(transform, anchorName);
                overlayChild = FindChildRecursive(transform, overlayName);

                anchorBaseLocalScale = anchorChild != null ? anchorChild.localScale : Vector3.one;
                overlayBaseLocalScale = overlayChild != null ? overlayChild.localScale : Vector3.one;

                _lastAnchorName = anchorName;
                _lastOverlayName = overlayName;
                _initialized = true;
            }
        }
    }
}
