using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Meta.XR;

public class QuestPassthroughSnapshot : MonoBehaviour
{
    [Header("Shared Passthrough Camera Access")]
    [SerializeField] private PassthroughCameraAccess cameraAccess;

    [Header("Snapshot")]
    [SerializeField] private int snapshotWidth = 512;
    [SerializeField] private int snapshotHeight = 512;

    [Range(1, 100)]
    [SerializeField] private int jpegQuality = 65;

    [Header("Debug")]
    [SerializeField] private RawImage debugPreview;
    [SerializeField] private bool saveLastSnapshotToFile = true;
    [SerializeField] private string savedSnapshotFileName = "last_passthrough_snapshot.jpg";

    private Texture2D debugTexture;

    public bool IsReady
    {
        get
        {
            if (cameraAccess == null)
                return false;

            if (!PassthroughCameraAccess.IsSupported)
                return false;

            if (!cameraAccess.IsPlaying)
                return false;

            Texture texture = cameraAccess.GetTexture();

            if (texture == null)
                return false;

            if (texture.width <= 16 || texture.height <= 16)
                return false;

            return true;
        }
    }

    public string CaptureJpegBase64()
    {
        if (!IsReady)
        {
            Debug.LogError("Cannot capture snapshot because shared PassthroughCameraAccess is not ready.");
            return null;
        }

        Texture sourceTexture = cameraAccess.GetTexture();

        if (sourceTexture == null)
        {
            Debug.LogError("PassthroughCameraAccess.GetTexture returned null.");
            return null;
        }

        RenderTexture previous = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(
            snapshotWidth,
            snapshotHeight,
            0,
            RenderTextureFormat.ARGB32
        );

        Texture2D snapshotTexture = new Texture2D(
            snapshotWidth,
            snapshotHeight,
            TextureFormat.RGB24,
            false
        );

        Graphics.Blit(sourceTexture, renderTexture);

        RenderTexture.active = renderTexture;

        snapshotTexture.ReadPixels(
            new Rect(0, 0, snapshotWidth, snapshotHeight),
            0,
            0
        );

        snapshotTexture.Apply();

        byte[] jpegBytes = snapshotTexture.EncodeToJPG(jpegQuality);
        string base64 = Convert.ToBase64String(jpegBytes);

        if (debugPreview != null)
        {
            if (debugTexture != null)
                Destroy(debugTexture);

            debugTexture = new Texture2D(snapshotWidth, snapshotHeight, TextureFormat.RGB24, false);
            debugTexture.LoadImage(jpegBytes);
            debugPreview.texture = debugTexture;
        }

        if (saveLastSnapshotToFile)
        {
            string path = Path.Combine(Application.persistentDataPath, savedSnapshotFileName);
            File.WriteAllBytes(path, jpegBytes);
            Debug.Log("Saved passthrough snapshot: " + path);
        }

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTexture);
        Destroy(snapshotTexture);

        if (base64.Length < 1000)
        {
            Debug.LogError("Captured snapshot is too small. Base64 length: " + base64.Length);
            return null;
        }

        Debug.Log("Captured passthrough snapshot. Base64 length: " + base64.Length);

        return base64;
    }

    private void OnDestroy()
    {
        if (debugTexture != null)
        {
            Destroy(debugTexture);
            debugTexture = null;
        }
    }
}