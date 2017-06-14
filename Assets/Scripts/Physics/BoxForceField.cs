using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BoxForceField : ForceField
{
    #region Inspector variables
    [FancyHeader("Box Force Field", "Specific box force field settings")]
    [SerializeField]
    [Tooltip("Force strength dependent on the distance to the middle point or not.")]
    private bool uniformForce = false;

    [SerializeField]
    private bool randomizeForce = false;

    [SerializeField]
    private float noiseTurbulence = 1f;
    #endregion

    #region Internal Members
    private static readonly int COLUMNS = 10;
    private static readonly int ROWS = 5;
    private BoxCollider boxCollider;

    private Vector2 worldPosition2D = Vector2.zero;
    private Vector2 externalBodyPosition = Vector2.zero;
    private Vector3 resultVector = Vector3.zero;
    #endregion

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    protected override Vector3 GetForceFor(Vector3 position)
    {
        Vector3 worldColliderCenter = GetWorldColliderCenter();
        worldPosition2D.Set(worldColliderCenter.x, worldColliderCenter.z);
        externalBodyPosition.Set(position.x, position.z);

        if (externalBodyPosition.x < worldPosition2D.x)
            return CalculateForceStrength(-transform.right);
        else
            return CalculateForceStrength(transform.right);
    }

    protected override void DrawDebugVisualization()
    {
        float rowStep = boxCollider.size.z / ROWS;
        float columnStep = boxCollider.size.x / COLUMNS;
        Vector3 minBound = boxCollider.bounds.min;

        DebugExtension.DebugWireSphere(minBound, Color.blue, 2f);
        
        for (float currentRowStep = minBound.z; currentRowStep <= minBound.z + boxCollider.size.z; currentRowStep += rowStep)
        {
            for (float currentColumnStep = minBound.x; currentColumnStep <= minBound.x + boxCollider.size.x; currentColumnStep += columnStep)
            {
                Vector3 drawPosition = new Vector3(currentColumnStep, transform.position.y, currentRowStep);
                Vector3 forceDirection = GetForceFor(drawPosition);

                DebugExtension.DebugArrow(drawPosition, forceDirection, GetMagnitudeColor(forceDirection));
            }
        }
    }

    private Vector3 CalculateForceStrength(Vector3 forceDirection)
    {
        if (uniformForce)
            resultVector = forceDirection.normalized;
        else
        {
            float halfSizeX = boxCollider.bounds.extents.x;
            float distanceFromMiddlePointX = Mathf.Abs(worldPosition2D.x - externalBodyPosition.x);

            float strength = distanceFromMiddlePointX / halfSizeX;
            resultVector = forceDirection * strength;
        }

        // Randomization
        if (randomizeForce)
        {
            resultVector = Quaternion.AngleAxis((Mathf.PerlinNoise(externalBodyPosition.x, externalBodyPosition.y) - 0.5f) * 2f * noiseTurbulence, Vector3.up) * forceDirection;
        }
        return resultVector;
    }

    private Color GetMagnitudeColor(Vector3 force)
    {
        float magnitude = force.magnitude;
        return Color.Lerp(Color.green, Color.red, magnitude);
    }

    private Vector3 GetWorldColliderCenter()
    {
        return boxCollider.transform.TransformPoint(boxCollider.center);
    }
}