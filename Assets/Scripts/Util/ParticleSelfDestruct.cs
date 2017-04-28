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

    private ParticleSystem particles;

    private void Start()
    {
        particles = GetComponent<ParticleSystem>();
        Destroy(gameObject, particles.main.startLifetime.constant + addDestroyTime);
    }
}
