using UnityEngine;

/// <summary>
/// Handles particle and FX spawning and Animator related
/// tasks.
/// </summary>
[RequireComponent(typeof(RigidBodyInput))]
public class PlayerAnimationAndFxHandler : MonoBehaviour
{
    #region Inspector variables
    [FancyHeader("Jump Particles", "Various jump particles")]
    [SerializeField]
    private Vector3 particleSpawnOffset;

    [SerializeField]
    private GameObject firstJumpParticle;

    [SerializeField]
    private GameObject secondJumpParticle;

    [FancyHeader("Run Particles")]
    [SerializeField]
    private float runParticleActivationThreshold = 0.1f;

    [SerializeField]
    private ParticleSystem runningParticle;

    [FancyHeader("Other particles")]
    [SerializeField]
    private ParticleSystem landedParticles;

    [SerializeField]
    private ParticleSystem swooshParticle;

    [SerializeField]
    private ParticleSystem dashPlayerCollisionParticle;

    [SerializeField]
    private ParticleSystem dashWallCollisionParticle;

    [FancyHeader("Player Sounds")]
    [SerializeField]
    private AudioClip dashClip;

    [SerializeField]
    [Range(-3,3)]
    private float dashPitch;

    [SerializeField]
    [Range(0,1)]
    private float dashVolume;

    [SerializeField]
    private AudioClip collideClip;

    [SerializeField]
    [Range(-3,3)]
    private float collidePitch;

    [SerializeField]
    [Range(0,1)]
    private float collideVolume;

    [SerializeField]
    private MultipleAudioclips stepClips;

    [SerializeField]
    private AudioClip jumpClip;

    [SerializeField]
    [Range(-3,3)]
    private float firstJumpPitch;

     [SerializeField]
    [Range(-3,3)]
    private float secondJumpPitch;

    [SerializeField]
    [Range(0,1)]
    private float jumpVolume;

    [SerializeField]
    private AudioClip landedClip;

    [SerializeField]
    [Range(-3,3)]
    private float landedPitch;

    [SerializeField]
    [Range(0,1)]
    private float landedVolume;

    [SerializeField]
    private AudioClip swordClip;

    [SerializeField]
    [Range(-3,3)]
    private float swordPitch;

    [SerializeField]
    [Range(0,1)]
    private float swordVolume;

    [FancyHeader("Dash reaction")]
    [SerializeField]
    private float dashShakePlayer = 3f;

    [SerializeField]
    private float dashTimePlayer = 0.1f;

    [SerializeField]
    private float dashShakeWall = 1f;

    [SerializeField]
    private float dashTimeWall = 0.1f;
    #endregion

    #region Internal Members
    private static readonly string ANIMATOR_RUN = "Run";
    private static readonly string ANIMATOR_JUMPED = "Jumped";
    private static readonly string ANIMATOR_GROUNDED = "Grounded";
    private static readonly string ANIMATOR_DASH = "Dash";
    private static readonly string ANIMATOR_CUT = "Cut";

    private static readonly string PLAYER_TAG = "Player";

    private RigidBodyInput inputHandler;
    private float originalRunningParticleRate = 0f;
    private float vectorOneMagnitude = Vector2.one.magnitude;
    private ParticleSystem.EmissionModule runningEmission;
    private Vector2 inputVector = Vector2.zero;
    private Animator playerAnimator;
    private Camera cam;
    #endregion

    private void Start ()
    {
        inputHandler = GetComponent<RigidBodyInput>();
        playerAnimator = GetComponent<Animator>();
        cam = CameraUtil.GetMainCamera();
        SetupRunningParticle();

        // Input Controller events
        inputHandler.InputController.OnJump += HandleFirstJump;
        inputHandler.InputController.OnSecondJump += HandleSecondJump;
        inputHandler.InputController.OnLandedOnGround += HandleGrounded;
        inputHandler.InputController.OnPlayerCut += HandleCutMove;

        // Dash Controller events
        inputHandler.DashController.OnDashStarted += HandleDash;
        inputHandler.DashController.OnDashCollision += HandleDashCollision;
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

        FillAnimationData(horizontal, vertical);
        HandleRunningParticles(horizontal, vertical);
    }

    private void FillAnimationData(float horizontal, float vertical)
    {
        // Running
        if (inputHandler.AllowMovement)
        {
            float runSpeed = Mathf.Max(Mathf.Abs(horizontal), Mathf.Abs(vertical));
            playerAnimator.SetFloat(ANIMATOR_RUN, runSpeed);
        }
        else
            playerAnimator.SetFloat(ANIMATOR_RUN, 0f);

        // Grounded
        playerAnimator.SetBool(ANIMATOR_GROUNDED, inputHandler.InputController.IsGrounded);
    }

    private void HandleRunningParticles(float horizontal, float vertical)
    {
        var emission = runningParticle.emission;
        if (inputHandler.AllowMovement)
        {

            inputVector.Set(horizontal, vertical);

            float t = Mathf.InverseLerp(0f, vectorOneMagnitude, inputVector.magnitude);
            emission.rateOverTime = Mathf.Lerp(0f, originalRunningParticleRate, t);
        }
        else
            emission.rateOverTime = 0f;
    }

    private void HandleCutMove()
    {
        // Animation
        playerAnimator.SetTrigger(ANIMATOR_CUT);

        // Particle
        if (swooshParticle != null)
            swooshParticle.Play(true);

        // Sound
        if(swordClip != null)
            SoundManager.SoundManagerInstance.Play(swordClip, transform, swordVolume, swordPitch, false, AudioGroup.Character);
    }

    private void HandleDash()
    {
        // Animation
        playerAnimator.SetTrigger(ANIMATOR_DASH);

        // Sound
        if(dashClip != null)
            SoundManager.SoundManagerInstance.Play(dashClip, transform, dashVolume, dashPitch, false, AudioGroup.Character);
    }

     private void HandleDashCollision(GameObject player, GameObject other)
    {

        // Dash Camera Shake
        if (other.tag.Equals(PLAYER_TAG))
        {
            Vector3 particlePosition = other.transform.position + Vector3.up * 0.6f;
            CameraUtil.DirectionalShake(cam, player.transform, dashShakePlayer, dashTimePlayer);
            if (dashPlayerCollisionParticle != null)
            {
                ParticleSystem g = Instantiate(dashPlayerCollisionParticle, particlePosition, dashPlayerCollisionParticle.transform.rotation) as ParticleSystem;
                g.transform.forward = player.transform.forward;
            }
        }
        else
        {
            CameraUtil.DirectionalShake(cam, player.transform, dashShakeWall, dashTimeWall);
            if (dashWallCollisionParticle != null)
            {
                //ParticleSystem g = Instantiate(dashWallCollisionParticle, transform.position, dashPlayerCollisionParticle.transform.rotation) as ParticleSystem;
                //g.transform.forward = player.transform.forward;
            }
        }

        // Sound
        if (collideClip != null)
            SoundManager.SoundManagerInstance.Play(collideClip, transform, collideVolume, collidePitch, false, AudioGroup.Character);
     }

    private void HandleFirstJump()
    {
        // Particles
        runningEmission.enabled = false;
        if (firstJumpParticle != null)
            Instantiate(firstJumpParticle, transform.position + particleSpawnOffset, firstJumpParticle.transform.rotation);

        // Animation
        playerAnimator.SetTrigger(ANIMATOR_JUMPED);

        // Sound
        if(jumpClip != null)
            SoundManager.SoundManagerInstance.Play(jumpClip, transform, jumpVolume, firstJumpPitch, false, AudioGroup.Character);
    }

    private void HandleSecondJump()
    {
        // Particles
        if (secondJumpParticle != null)
            Instantiate(secondJumpParticle, transform.position + particleSpawnOffset, secondJumpParticle.transform.rotation);

        // Animation
        playerAnimator.SetTrigger(ANIMATOR_JUMPED);

        // Sound
        if(jumpClip != null)
            SoundManager.SoundManagerInstance.Play(jumpClip, transform, jumpVolume, secondJumpPitch, false, AudioGroup.Character);
    }

    private void HandleGrounded()
    {
        // Running particles
        runningEmission.enabled = true;

        // Landed particles
        if (landedParticles != null)
            Instantiate(landedParticles, transform.position, landedParticles.transform.rotation);

        // Sound
        if(landedClip != null)
            SoundManager.SoundManagerInstance.Play(landedClip, transform, landedVolume, landedPitch, false, AudioGroup.Character);
    }

    // Step sounds
    public void Step(){
        if(stepClips != null)
            stepClips.PlayRandomClip();
    }
}