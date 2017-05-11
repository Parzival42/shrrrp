using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactTracker : MonoBehaviour
{
    #region Inspector variables
    [FancyHeader("General Settings")]
    [SerializeField]
    private string artifactTag = "Artifact";

    [SerializeField]
    private GameObject artifactPrefab;

    [SerializeField]
    private Vector3 artifactSpawnOffset = new Vector3(0f, 1f, 0f);

    [FancyHeader("Spawn Time", "Timing Properties")]
    [SerializeField]
    private float minTime = 5f;

    [SerializeField]
    private float maxTime = 10f;

    [FancyHeader("Raycast setting")]
    [SerializeField]
    private float sphereCastRadius = 0.5f;

    [SerializeField]
    private int collisionLayer = 8;

    [SerializeField]
    [Tooltip("Values should be between 0f and 1f.")]
    private Vector2 randomXViewportRange = new Vector2(0f, 1f);

    [SerializeField]
    [Tooltip("Values should be between 0f and 1f.")]
    private Vector2 randomYViewportRange = new Vector2(0.3f, 1f);
    #endregion

    #region Internal Members
    private PlayerManager playerManager;
    private Camera cam;
    private Vector3 randomViewportPoint = Vector3.zero;
    #endregion

    private void Start ()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        cam = CameraUtil.GetMainCamera();
        StartCoroutine(StartRaycast());
	}

    protected IEnumerator StartRaycast()
    {
        yield return new WaitForSeconds(GetRandomSpawnTime());
        if (CanSpawnArtifact())
        {
            RaycastHit hitInfo;
            if (RandomSphereCast(out hitInfo))
            {
                DebugExtension.DebugPoint(hitInfo.point, Color.red, 2f, 2f, false);
                SpawnArtifactAt(hitInfo.point + artifactSpawnOffset);
            }
        }
        StartCoroutine(StartRaycast());
    }

    protected bool CanSpawnArtifact()
    {
        GameObject[] artifact = GameObject.FindGameObjectsWithTag(artifactTag);
        if (artifact.Length == 0)
            return true;
        else if (artifact.Length == 1)
        {
            return false;
        }
        else
        {
            Debug.LogError("More than one artifacts are not allowed!", gameObject);
            return false;
        } 
    }

    protected bool RandomSphereCast(out RaycastHit hitInfo)
    {
        Ray ray = GetRandomCameraRay();
        bool hit = Physics.SphereCast(ray, sphereCastRadius, out hitInfo, 20f, 1 << collisionLayer);
#if UNITY_EDITOR
        DebugExtension.DebugCylinder(ray.origin, ray.GetPoint(20f), Color.magenta, sphereCastRadius, 2f, true);
#endif
        return hit;
    }

    protected GameObject SpawnArtifactAt(Vector3 position)
    {
        GameObject artifact = Instantiate(artifactPrefab, position, artifactPrefab.transform.rotation);
        return artifact;
    }

    protected Ray GetRandomCameraRay()
    {
        randomViewportPoint.Set(Random.Range(randomXViewportRange.x, randomXViewportRange.y), Random.Range(randomYViewportRange.x, randomYViewportRange.y), 0f);
        Ray ray = cam.ViewportPointToRay(randomViewportPoint);
        return ray;
    }

    protected float GetRandomSpawnTime()
    {
        return Random.Range(minTime, maxTime);
    }
}