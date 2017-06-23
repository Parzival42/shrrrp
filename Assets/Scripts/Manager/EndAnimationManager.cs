using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.PostProcessing.Utilities;
using UnityEngine.SceneManagement;

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

    [FancyHeader("Dissolve", "Dissolve specific settings")]
    [SerializeField]
    private float maxDissolveTime = 4f;

    [SerializeField]
    private float minDissolveTime = 1.5f;

    [SerializeField]
    private float maxDissolveScale = 4f;

    [SerializeField]
    private float minDissolveScale = 1.5f;
    #endregion

    #region Camera specific variables
    private Camera gameCamera;
    private FocusPuller focusPuller;
    private PostProcessingController postProcessing;
    #endregion

    #region Player Animation variables
    private static readonly string PLAYER_ANIMATOR_CUT = "EndCut";
    private Animator playerAnimator;
    private RigidBodyInput playerInput;
    private ParticleSystem playerSwordParticle;
    private PlayerAnimationAndFxHandler playerAnimationHandler;
    #endregion

    private void Start ()
    {
        gameCamera = CameraUtil.GetMainCamera();
        postProcessing = gameCamera.GetComponent<PostProcessingController>();

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
        playerAnimationHandler = winner.GetComponent<PlayerAnimationAndFxHandler>();
        playerAnimationHandler.enabled = false;

        playerSwordParticle.Emit(1);

        playerInput.AllowMovement = false;
        playerInput.Rigid.constraints = RigidbodyConstraints.FreezeAll;

        //focusPuller = gameCamera.gameObject.AddComponent<FocusPuller>();
        //focusPuller.target = winner.transform;
    }
    
    private void StartAnimation(Player winner)
    {
        // Dissolve
        DestroyTheWholeUniverseTwice();

        // Player position
        PositionPlayer(winner);
        
        // Play player animation
        LeanTween.delayedCall(playerCutAnimationDelay, () => {
            playerAnimator.SetTrigger(PLAYER_ANIMATOR_CUT);
        });

        TweenDepthOfField();

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

    private void TweenDepthOfField()
    {
        // Depth of Field (Basically disable it)
        postProcessing.controlDepthOfField = true;
        postProcessing.enableDepthOfField = true;
        LeanTween.value(postProcessing.depthOfField.aperture, 32f, cameraTweenTime)
            .setOnUpdate((float value) => {
                postProcessing.depthOfField.aperture = value;
            });
    }

    private Player GetWinner()
    {
        return FindObjectOfType<Player>();
    }

    private void DestroyTheWholeUniverseTwice()
    {
        List<GameObject> rootObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjects);

        for (int i = 0; i < rootObjects.Count; ++i)
        {
            GameObject gameObject = rootObjects[i];
            if (gameObject.hideFlags == HideFlags.None && gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                AddDissolve(gameObject);
            }
            else if (gameObject.hideFlags == HideFlags.None && gameObject.layer == LayerMask.NameToLayer("TerrainPhysics"))
            {
                GameObject child = gameObject.transform.GetChild(0).gameObject;
                if (child != null)
                    AddDissolve(child);
            }
        }
    }

    private void AddDissolve(GameObject g)
    {
        DissolveManual disolvedObject = g.AddComponent<DissolveManual>();
        disolvedObject.DissolveObject(Random.Range(minDissolveTime, maxDissolveTime),
            Random.Range(minDissolveScale, maxDissolveScale));
    }
}