using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshInstancer : MonoBehaviour
{
    [Comment("Can instance a high number of object (same mesh data) with different transformation values." 
        + "Those can be animated in order to achieve effects like floating clouds, etc...", 35)]

    [FancyHeader("Mesh Settings", "Mesh related settings")]
    [SerializeField]
    private Mesh mesh;

    [SerializeField]
    private Material material;

    [SerializeField]
    private Vector3 size = Vector3.one;

    [SerializeField]
    private Vector3 sizeOffset = Vector3.zero;

    [SerializeField]
    [Tooltip("Picks the largest value of 'sizeOffset' and uses it for scaling.")]
    private bool uniformScale = true;

    [SerializeField]
    [Tooltip("Min. size for uniform scaling")]
    private float minSize = 0f;

    [Space(10)]
    [SerializeField]
    private Vector3 positionOffset = Vector3.zero;

    [Space(10)]
    [SerializeField]
    private Vector3 eulerFrom = Vector3.zero;

    [SerializeField]
    private Vector3 eulerTo = Vector3.zero;

    [FancyHeader("Render settigs", "General render specific settings")]
    [SerializeField]
    [Tooltip("Max number: 1023")]
    private int meshCount = 100;

    [SerializeField]
    private Vector2 spawnPlane = Vector2.one * 5f;

    [SerializeField]
    private Vector2 translation = Vector2.zero;

    [SerializeField]
    private bool repeatPosition = false;

    [FancyHeader("Update settings", "Animation update settings")]
    [SerializeField]
    [Tooltip("Specifies how many objects are updated per frame")]
    private int objectsPerFrame = 10;

    #region Internal Members
    private Matrix4x4[] matrices;
    private Vector3[] positions;
    private float meshCountSqrt;
    private int currentObject = 0;
    private Vector2 halfSize = Vector2.zero;
    #endregion

    private void Start ()
    {
        meshCount = (int) Mathf.Clamp(meshCount, 0f, 1023f);
        meshCountSqrt = Mathf.Floor(Mathf.Sqrt(meshCount));
        matrices = new Matrix4x4[meshCount];
        positions = new Vector3[meshCount];
        halfSize.Set(spawnPlane.x * 0.5f, spawnPlane.y * 0.5f);

        CalculatePositions();
	}
	
	private void Update ()
    {
        Graphics.DrawMeshInstanced(mesh, 0, material, matrices);
        DebugExtension.DebugLocalCube(transform, new Vector3(spawnPlane.x, 0.1f, spawnPlane.y));
        UpdateTransformations();
	}

    private void UpdateTransformations()
    {
        do
        {
            if (currentObject == meshCount)
                currentObject = 0;

            if (!repeatPosition)
                positions[currentObject].Set(positions[currentObject].x + translation.x, positions[currentObject].y, positions[currentObject].z + translation.y);
            else
            {
                float positionX = RepeatBetween(transform.position.x - halfSize.x, transform.position.x + halfSize.x, positions[currentObject].x + translation.x);
                float positionZ = RepeatBetween(transform.position.z - halfSize.y, transform.position.z + halfSize.y, positions[currentObject].z + translation.y);
                positions[currentObject].Set(positionX, positions[currentObject].y, positionZ);
            }

            matrices[currentObject] = SetTranslationFor(matrices[currentObject], positions[currentObject]);
        } while (currentObject++ % objectsPerFrame != 0);
    }

    private float RepeatBetween(float min, float max, float value)
    {
        if (value > max)
            return min;
        else if (value < min)
            return max;

        return value;
    }

    private void CalculatePositions()
    {
        Vector3 startPosition = transform.position - new Vector3(spawnPlane.x, 0f, spawnPlane.y) * 0.5f;
        Vector3 originalStartPosition = new Vector3(startPosition.x, startPosition.y, startPosition.z);
        Vector2 gaps = new Vector2(spawnPlane.x / meshCountSqrt, spawnPlane.y / meshCountSqrt);

        for (int x = 0, i = 0; x < meshCountSqrt; x++)
        {
            for (int z = 0; z < meshCountSqrt; z++, i++)
            {
                positions[i] = RandomOffsetOf(startPosition, positionOffset);
                matrices[i] = Matrix4x4.TRS(
                    positions[i],
                    Quaternion.Euler(RandomFromTo(eulerFrom, eulerTo)),
                    uniformScale ? size + GetUniformRandomOffset(size) : RandomOffsetOf(size, sizeOffset));

                startPosition.Set(startPosition.x, startPosition.y, startPosition.z + gaps.y);
            }
            startPosition.Set(startPosition.x + gaps.x, startPosition.y, originalStartPosition.z);
        }
    }

    private Vector3 GetUniformRandomOffset(Vector3 vector)
    { 
        float largest = Mathf.Max(vector.x, Mathf.Max(vector.y, vector.z));
        return Vector3.one * Random.Range(minSize, largest);
    }

    private Vector3 RandomOffsetOf(Vector3 original, Vector3 offset)
    {
        return original + new Vector3(Random.Range(-offset.x, offset.x), Random.Range(-offset.y, offset.y), Random.Range(-offset.z, offset.z));
    }

    private Vector3 RandomFromTo(Vector3 from, Vector3 to)
    {
        return new Vector3(Random.Range(from.x, to.x), Random.Range(from.y, to.y), Random.Range(from.z, to.z));
    }

    private Matrix4x4 SetTranslationFor(Matrix4x4 matrix, Vector3 position)
    {
        matrix.m03 = position.x;
        matrix.m13 = position.y;
        matrix.m23 = position.z;
        return matrix;
    }

    private Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
    }
}