using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAnimationManager : MonoBehaviour
{
    #region Inspector variables
    [FancyHeader("Camera", "Camera specific animation settings")]
    [SerializeField]
    private float cameraTweenTime = 0.5f;

    [SerializeField]
    private LeanTweenType cameraEase = LeanTweenType.easeInBack;

    [SerializeField]
    private Vector3 cameraTargetRotation = Vector3.zero;

    [SerializeField]
    private Vector3 cameraPositionOffset = Vector3.zero;

    [FancyHeader("Player", "Player specific animation settings")]
    [SerializeField]
    private Vector3 playerLookAtTarget = Vector3.zero;

    [SerializeField]
    private float playerCutAnimationDelay = 2f;
    #endregion

    #region Camera specific variables
    private Camera gameCamera;
    #endregion

    #region Player Animation variables
    private static readonly string PLAYER_ANIMATOR_CUT = "EndCut";
    private Animator playerAnimator;
    private RigidBodyInput playerInput;
    private ParticleSystem playerSwordParticle;
    #endregion

    private void Start ()
    {
        gameCamera = CameraUtil.GetMainCamera();

        // Start animation based on game end event
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.OnGameEnded += PerformGameEndAnimation;
	}

    [ContextMenu("Start Animation")]
    private void PerformGameEndAnimation()
    {
        Player winnerPlayer = GetWinner();
        InitializeAnimationParameters(winnerPlayer);
        StartAnimation(winnerPlayer);
    }

    private void InitializeAnimationParameters(Player winner)
    {
        playerAnimator = winner.GetComponent<Animator>();
        playerInput = winner.GetComponent<RigidBodyInput>();
        playerSwordParticle = winner.GetComponentInChildren<ParticleSystem>();

        playerSwordParticle.Emit(1);

        playerInput.AllowMovement = false;
        playerInput.Rigid.constraints = RigidbodyConstraints.FreezeAll;
    }
    
    private void StartAnimation(Player winner)
    {
        PositionPlayer(winner);

        LeanTween.delayedCall(playerCutAnimationDelay, () => {
            playerAnimator.SetTrigger(PLAYER_ANIMATOR_CUT);
        });

        // Position camera
        LeanTween.move(gameCamera.gameObject, winner.transform.position + cameraPositionOffset, cameraTweenTime)
            .setEase(cameraEase);
        LeanTween.rotate(gameCamera.gameObject, cameraTargetRotation, cameraTweenTime).setEase(cameraEase);
    }

    private void PositionPlayer(Player winner)
    {
        // Tween this maybe
        Vector3 cameraEndPosition = winner.transform.position + playerLookAtTarget;
        winner.transform.LookAt(new Vector3(cameraEndPosition.x, winner.transform.position.y, cameraEndPosition.z));
    }

    private Player GetWinner()
    {
        return FindObjectOfType<Player>();
    }
}