using UnityEngine;

public class PlaneCutterSpawnTrigger : TriggerAction
{
    [FancyHeader("Plane cutter spawn settings", "Specific settings for plane cutting")]
    [SerializeField]
    private GameObject planeCutter;

    protected override void PerformOnTriggerAction(Collider other)
    {
        // TODO: Spawn plane cutter
        // TODO: Handle control stuff
    }
}