using UnityEngine;

public class BlockStrengthAdjuster : MonoBehaviour
{
    [SerializeField] private Material blockMaterial;

    [SerializeField] private float step = 0.1f;
    [SerializeField] private float minBlockStrength = 0f;
    [SerializeField] private float maxBlockStrength = 1f;

    private static readonly int BlockStrengthId = Shader.PropertyToID("_BlockStrength");

    public void IncreaseBlockStrength()
    {
        if (blockMaterial == null)
            return;

        float blockStrength = blockMaterial.GetFloat(BlockStrengthId);
        blockStrength = Mathf.Clamp(blockStrength + step, minBlockStrength, maxBlockStrength);

        blockMaterial.SetFloat(BlockStrengthId, blockStrength);

        Debug.Log("IncreaseBlockStrength called. Block strength: " + blockStrength);
    }

    public void DecreaseBlockStrength()
    {
        if (blockMaterial == null)
            return;

        float blockStrength = blockMaterial.GetFloat(BlockStrengthId);
        blockStrength = Mathf.Clamp(blockStrength - step, minBlockStrength, maxBlockStrength);

        blockMaterial.SetFloat(BlockStrengthId, blockStrength);

        Debug.Log("DecreaseBlockStrength called. Block strength: " + blockStrength);
    }
}