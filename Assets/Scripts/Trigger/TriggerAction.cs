using UnityEngine;

public abstract class TriggerAction : MonoBehaviour
{
    [Comment("This script performs a specific task when a GameObject with a given tag entered a trigger.")]

    [FancyHeader("General settings")]
    [SerializeField]
    protected string collisionTag = "Player";

    [SerializeField]
    protected bool destroyAfterTriggering = false;

	protected virtual void Start ()
    {
        CheckIfTriggerAvailable();
	}

    private void CheckIfTriggerAvailable()
    {
        bool foundTrigger = false;
        Collider[] allCollider = GetComponents<Collider>();
        foreach(Collider c in allCollider) {
            if (c.isTrigger)
                foundTrigger = true;
        }

        if (!foundTrigger)
            Debug.LogError("No trigger attached to this GameObject!", gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == collisionTag)
        {
            PerformOnTriggerAction(other);
            if (destroyAfterTriggering)
                DestroyTrigger();
        }
    }

    protected virtual void DestroyTrigger()
    {
        Destroy(gameObject);
    }

    protected abstract void PerformOnTriggerAction(Collider other);
}