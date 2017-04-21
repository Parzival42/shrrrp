using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SphereForceField : ForceField
{
    #region Inspector variables
    [FancyHeader("Sphere Force Field", "Specific sphere force field settings")]
    [SerializeField]
    [Tooltip("Force strength dependent on the distance to the middle point or not.")]
    private bool uniformForce = false;

    [SerializeField]
    private bool randomizeForce = false;

    [SerializeField]
    private float noiseTurbulence = 1f;
    #endregion

    #region Internal Members
    private static readonly int DEBUG_ITERATIONS = 8;
    private SphereCollider sphereCollider;

    Vector2 middlePosition = Vector2.zero;
    Vector2 bodyPosition = Vector2.zero;
    #endregion

    private void Start ()
    {
        sphereCollider = GetComponent<SphereCollider>();
	}

    protected override Vector3 GetForceFor(Vector3 position)
    {

        Vector3 worldColliderCenter = GetWorldColliderCenter();
        // Only XZ plane starting from the circle collider middle point
        middlePosition.Set(worldColliderCenter.x, worldColliderCenter.z);
        bodyPosition.Set(position.x, position.z);

        // Calculate direction and normalize it based on the collider radius
        Vector2 forceDirection = bodyPosition - middlePosition;
        forceDirection *= 1f / sphereCollider.radius;

        // Randomization
        if (randomizeForce)
            forceDirection = forceDirection.Rotate((Mathf.PerlinNoise(bodyPosition.x, bodyPosition.y) - 0.5f) * 2f * noiseTurbulence);

        // Save into 3D result vector
        Vector3 resultVector = new Vector3(forceDirection.x, 0f, forceDirection.y);
        if (uniformForce)
            resultVector.Normalize();

        return resultVector;
    }

    protected override void DrawDebugVisualization()
    {
        float distanceStep = sphereCollider.radius / DEBUG_ITERATIONS;
        Vector3 worldColliderCenter = GetWorldColliderCenter();

        // Red shape circles
        DebugExtension.DebugCircle(worldColliderCenter, Vector3.up, Color.red, 0.5f);
        DebugExtension.DebugCircle(worldColliderCenter + Vector3.up * 0.3f, Vector3.up, Color.red, sphereCollider.radius);
        DebugExtension.DebugCircle(worldColliderCenter + Vector3.down * 0.3f, Vector3.up, Color.red, sphereCollider.radius);

        // Arrows
        for (int i = 0; i < DEBUG_ITERATIONS; i++)
        {
            Vector3 forward = sphereCollider.transform.forward;
            float distanceFromCenter = distanceStep * (i + 1);

            for (float angle = 0; angle <= 360f; angle += 20f)
            {
                Vector3 position = worldColliderCenter + forward * distanceFromCenter;
                Vector3 force = GetForceFor(position);
                DebugExtension.DebugArrow(position, force, GetMagnitudeColor(force));

                forward = Quaternion.Euler(0f, angle, 0f) * sphereCollider.transform.forward;
                forward.Normalize();
            }
        }
    }

    private Color GetMagnitudeColor(Vector3 force)
    {
        float magnitude = force.magnitude;
        return Color.Lerp(Color.green, Color.red, magnitude);
    }

    private Vector3 GetWorldColliderCenter()
    {
        return sphereCollider.transform.TransformPoint(sphereCollider.center);
    }
}