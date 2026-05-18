using UnityEngine;

public class LoopingMothFlight : MonoBehaviour
{
    [Header("Loop")]
    public Transform centerPoint;
    public float loopDuration = 12f;

    [Header("Flight Shape")]
    public float width = 3f;
    public float depth = 2f;
    public float height = 1.2f;

    [Header("Natural Motion")]
    public float wobbleAmount = 0.35f;
    public float verticalFlutter = 0.18f;
    public float rotationSmoothness = 6f;

    [Header("Timing")]
    public float startOffset = 0f;

    private Vector3 previousPosition;

    void Start()
    {
        if (centerPoint == null)
            centerPoint = transform.parent;

        previousPosition = transform.position;
    }

    void Update()
    {
        float t = ((Time.time + startOffset) % loopDuration) / loopDuration;
        float angle = t * Mathf.PI * 2f;

        Vector3 center = centerPoint != null ? centerPoint.position : Vector3.zero;

        Vector3 position = GetLoopPosition(center, angle);

        transform.position = position;

        Vector3 velocity = position - previousPosition;

        if (velocity.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSmoothness * Time.deltaTime
            );
        }

        previousPosition = position;
    }

    Vector3 GetLoopPosition(Vector3 center, float angle)
    {
        // Main lazy figure-eight / oval motion
        float x = Mathf.Sin(angle) * width;
        float z = Mathf.Sin(angle * 2f) * depth * 0.5f;

        // Slow natural rising and falling
        float y = Mathf.Sin(angle * 1.5f + 0.7f) * height * 0.5f;

        // Small looping irregularities, all periodic so the path loops perfectly
        x += Mathf.Sin(angle * 3f + 1.2f) * wobbleAmount;
        z += Mathf.Cos(angle * 4f + 0.4f) * wobbleAmount;
        y += Mathf.Sin(angle * 5f) * verticalFlutter;

        return center + new Vector3(x, y, z);
    }

    void OnDrawGizmosSelected()
    {
        Vector3 center = centerPoint != null ? centerPoint.position : transform.position;

        Gizmos.color = Color.yellow;

        Vector3 previous = GetLoopPosition(center, 0f);

        int steps = 120;

        for (int i = 1; i <= steps; i++)
        {
            float t = i / (float)steps;
            float angle = t * Mathf.PI * 2f;

            Vector3 next = GetLoopPosition(center, angle);

            Gizmos.DrawLine(previous, next);

            previous = next;
        }
    }
}