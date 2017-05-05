using UnityEngine;

/// <summary>
/// Destroys the ParticleSystem after its max life time. Additional
/// time can also be added in order to postpone the destruction.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class ParticleSelfDestruct : MonoBehaviour
{
    [Comment("Destroy the particle system automatically after its life time. Some extra time can be added.")]

    [FancyHeader("Settings")]
    [SerializeField]
    [Tooltip("This time is added to the particle life time.")]
    private float addDestroyTime = 0f;

    [SerializeField]
    [Tooltip("Should the duration field of the particle system be used? Otherwise the start life time is used.")]
    private bool useDuration = false;

    private ParticleSystem particles;

    private void Start()
    {
        particles = GetComponent<ParticleSystem>();

        if (!useDuration)
            Destroy(gameObject, particles.main.startLifetime.constant + addDestroyTime);
        else
            Destroy(gameObject, particles.main.duration + addDestroyTime);
    }
}
