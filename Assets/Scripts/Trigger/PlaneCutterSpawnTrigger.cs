using UnityEngine;

public class PlaneCutterSpawnTrigger : TriggerAction
{
    [FancyHeader("Plane cutter spawn settings", "Specific settings for plane cutting")]
    [SerializeField]
    private GameObject planeCutter;

    [SerializeField]
    private float tweenTime = 0.4f;

    [SerializeField]
    private LeanTweenType easeType = LeanTweenType.easeInBack;

    protected override void PerformOnTriggerAction(Collider other)
    {
        if (planeCutter != null)
        {
            RigidBodyInput playerInput = other.gameObject.GetComponent<RigidBodyInput>();
            if (playerInput != null)
            {
                // TODO: Find better initial rotation?
                GameObject prefab = Instantiate(planeCutter, playerInput.transform.position, Quaternion.identity);
                CuttingPlaneControl cuttingPlane = prefab.GetComponent<CuttingPlaneControl>();
                
                cuttingPlane.Initialize(playerInput);
            }
            else
                Debug.LogError("No RigidBodyInput found on collider object with the tag " + collisionTag, gameObject);
        }
        else
            Debug.LogError("No plane cutter prefab assigned!", gameObject);
    }

    protected override void DestroyTrigger()
    {
        LeanTween.scale(gameObject, Vector3.zero, tweenTime).setEase(easeType)
            .setOnComplete(() => {
                base.DestroyTrigger();
            });
    }
}