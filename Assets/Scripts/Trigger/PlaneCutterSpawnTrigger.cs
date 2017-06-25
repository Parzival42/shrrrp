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

    [SerializeField]
    private float despawnTime = 5f;

    private bool alreadyTriggered = false;
    private float currentDespawnTime = 0f;

    private void Update()
    {
        if (!alreadyTriggered && currentDespawnTime >= despawnTime)
        {
            alreadyTriggered = true;
            DestroyTrigger();
        }
        currentDespawnTime += Time.deltaTime;
    }

    protected override void PerformOnTriggerAction(Collider other)
    {
        if (!alreadyTriggered)
        {
            if (planeCutter != null)
            {
                RigidBodyInput playerInput = other.gameObject.GetComponent<RigidBodyInput>();
                if (playerInput != null)
                {
                    GameObject prefab = Instantiate(planeCutter, playerInput.transform.position, Quaternion.identity);
                    CuttingPlaneControl cuttingPlane = prefab.GetComponent<CuttingPlaneControl>();

                    cuttingPlane.Initialize(playerInput);
                   
                    alreadyTriggered = true;
                }
                else
                    Debug.LogError("No RigidBodyInput found on collider object with the tag " + collisionTag, gameObject);
            }
            else
                Debug.LogError("No plane cutter prefab assigned!", gameObject);
        }
    }

    protected override void DestroyTrigger()
    {
        GetComponent<AudioSource>().Play();
        
        LeanTween.scale(gameObject, Vector3.zero, tweenTime).setEase(easeType)
            .setOnComplete(() => {
                base.DestroyTrigger();
            });
    }
}