using UnityEngine;

/// <summary>
/// Handles particle and FX spawning and Animator related
/// tasks.
/// </summary>
[RequireComponent(typeof(RigidBodyInput))]
public class PlayerAnimationAndFxHandler : MonoBehaviour
{
    #region Inspector variables
    [Header("Particles")]
    [SerializeField]
    private Vector3 particleSpawnOffset;

    [SerializeField]
    private GameObject firstJumpParticle;

    [SerializeField]
    private GameObject secondJumpParticle;

    [Space(10f)]
    [SerializeField]
    private float runParticleActivationThreshold = 0.1f;

    [SerializeField]
    private ParticleSystem runningParticle;
    #endregion

    #region Internal Members
    private RigidBodyInput inputHandler;
    private float originalRunningParticleRate = 0f;
    private float vectorOneMagnitude = Vector2.one.magnitude;
    private ParticleSystem.EmissionModule runningEmission;
    private Vector2 inputVector = Vector2.zero;
    #endregion

    private void Start ()
    {
        inputHandler = GetComponent<RigidBodyInput>();
        SetupRunningParticle();

        // Input Controller events
        inputHandler.InputController.OnJump += HandleFirstJump;
        inputHandler.InputController.OnSecondJump += HandleSecondJump;
        inputHandler.InputController.OnLandedOnGround += HandleGrounded;
    }

    private void Update()
    {
        HandleDirectionalInput();
    }

    private void SetupRunningParticle()
    {
        runningParticle.gameObject.SetActive(true);
        originalRunningParticleRate = runningParticle.emission.rateOverTime.constant;
        runningEmission = runningParticle.emission;
    }

    /// <summary>
    /// Handles directional input information for animation, particles, etc.
    /// </summary>
    private void HandleDirectionalInput()
    {
        float horizontal = inputHandler.InputController.HorizontalInputValue;
        float vertical = inputHandler.InputController.VerticalInputValue;

        HandleRunningParticles(horizontal, vertical);
    }

    private void HandleRunningParticles(float horizontal, float vertical)
    {
        var emission = runningParticle.emission;

        inputVector.Set(horizontal, vertical);

        float t = Mathf.InverseLerp(0f, vectorOneMagnitude, inputVector.magnitude);
        emission.rateOverTime = Mathf.Lerp(0f, originalRunningParticleRate, t);
    }

    private void HandleFirstJump()
    {
        runningEmission.enabled = false;
        if (firstJumpParticle != null)
            Instantiate(firstJumpParticle, transform.position + particleSpawnOffset, firstJumpParticle.transform.rotation);
    }

    private void HandleSecondJump()
    {
        if (secondJumpParticle != null)
            Instantiate(secondJumpParticle, transform.position + particleSpawnOffset, secondJumpParticle.transform.rotation);
    }

    private void HandleGrounded()
    {
        runningEmission.enabled = true;
    }
}