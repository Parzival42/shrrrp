using UnityEngine;

public class DestroyTrigger : MonoBehaviour
{
    [FancyHeader("Tween time")]
    [SerializeField]
    private float tweenTime = 0.4f;

    [SerializeField]
    private float selfDestructionTime = 1.5f;

    private bool destroyed = false;

    private void Start()
    {
        LeanTween.delayedCall(selfDestructionTime, () => {
            if (!destroyed)
            {
                destroyed = true;
                PerformTween();
            }
        });
    }

    void OnTriggerExit(Collider other)
    {
        if (!destroyed && other.tag == "Player")
        {
            destroyed = true;
            PerformTween();
        }
    }

    private void PerformTween()
    {
        LeanTween.scale(gameObject, Vector3.zero, tweenTime).setEase(LeanTweenType.easeInBack)
            .setOnComplete(() => {
                Destroy(gameObject);
            });
    }
}
