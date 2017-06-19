using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CuttingPlaneControl : MonoBehaviour
{
    #region Inspector variables
    [FancyHeader("Control settings", "Translation, Rotation, ...")]
    [SerializeField]
    private float rotationSpeed = 100f;

    [SerializeField]
    private float translationSpeed = 100f;

    [SerializeField]
    private float maxZRotationOffset = 50f;

    [SerializeField]
    private float waitForDestruction = 0.8f;

    [SerializeField]
    private float activationTime = 3f;

    [FancyHeader("Tween settings")]
    [SerializeField]
    private float isoLineTweenTime = 0.6f;

    [FancyHeader("Cut effect parameters")]
    [SerializeField]
    private CuttingEffectParameters cutEffectParams;

    [FancyHeader("Cut effect sound")]
    [SerializeField]
    private AudioClip cutEffectSound;

    [SerializeField]
    private float cutEffectVolume = 1;

    [SerializeField]
    private float cutEffectPitch = 1;
    #endregion

    #region Internal variables
    private Camera cam;
    private IsoLine isoLine;
    private bool isInitialized = false;
    private bool isUsed = false;
    private RigidBodyInput inputHandler;
    private Vector3 rotationWorld = Vector3.zero;
    private Vector3 rotationLocal = Vector3.zero;
    private Vector3 translation = Vector3.zero;
    private float originalLineCount;
    private float originalLineAlpha;

    private static readonly string PLAYER_CANVAS_TAG = "MenuEnvironment";
    private static readonly string PLAYER_CUT_TEXT_TAG = "TextInsert";
    private static readonly string DECIMAL_FORMAT = "F2";
    private Text playerCutText;
    private float currentCutTime = 0f;

    GameManager gameManager;
    #endregion

    private void Start()
    {
        cam = CameraUtil.GetMainCamera();
        gameManager = FindObjectOfType<GameManager>();
        gameManager.OnGameEnded += DestroyPlaneDueToGameEnd;
        currentCutTime = activationTime;

        // Plane tween
        Vector3 originalScale = transform.localScale;
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, originalScale, isoLineTweenTime).setEase(LeanTweenType.easeOutBack);

        InitializeIsoLine();
    }

    private void Update ()
    {
        if (isInitialized)
        {
            HandleControls();
            HandleIsoLineParameters();
            HandleCutTime();
        }
	}

    private void HandleCutTime()
    {
        if (!isUsed && currentCutTime <= 0f)
        {
            isUsed = true;
            currentCutTime = 0f;
            playerCutText.text = currentCutTime.ToString(DECIMAL_FORMAT);
            SetInputHandlerMovemet(true);
            DestroyPlaneControl();
        }

        if (!isUsed)
        {
            playerCutText.text = currentCutTime.ToString(DECIMAL_FORMAT);
            currentCutTime -= Time.deltaTime;
        }
    }

    private void InitializeIsoLine()
    {
        isoLine = cam.GetComponent<IsoLine>();

        isoLine.enabled = true;
        isoLine.OriginPosition = transform.position;
        originalLineAlpha = isoLine.LineColor.a;
        originalLineCount = isoLine.LineCount;

        LeanTween.value(gameObject, 0f, originalLineCount, isoLineTweenTime).setEase(LeanTweenType.easeOutBack)
            .setOnUpdate((float value) => {
                isoLine.LineCount = value;
            });
    }

    private void HandleControls()
    {
        if(!isUsed)
            HandleRotation();

        if (!isUsed && (inputHandler.PlayerAction.Jump.WasPressed || inputHandler.PlayerAction.Dash.WasPressed))
        {
            isUsed = true;

            PositionPlayerDuringCut();
            TimeUtil.TimescalePingPong(cutEffectParams.timeDestination, cutEffectParams.timeEffectDuration, cutEffectParams.timeEase);
            CameraUtil.FovPingPong(cam, cam.fieldOfView, cam.fieldOfView + cutEffectParams.fovAdd, cutEffectParams.fovTime, cutEffectParams.fovEase);
            CuttingManagerLocator.GetInstance.Cut(transform, GetComponent<MeshFilter>().mesh, 0.4f);
            SoundManager.SoundManagerInstance.Play(cutEffectSound, Vector3.zero, cutEffectVolume, cutEffectPitch, false, AudioGroup.Effects);

            inputHandler.InputController.OnPlayerCutMove();
            DestroyPlaneControl();
        }
    }

    private void PositionPlayerDuringCut()
    {
        float originalPosition = inputHandler.transform.position.y;
        LeanTween.moveY(inputHandler.gameObject, inputHandler.transform.position.y + cutEffectParams.playerHeightOffset, cutEffectParams.playerTime).setEase(cutEffectParams.playerEase)
            .setOnComplete(() => {
                LeanTween.moveY(inputHandler.gameObject, originalPosition, cutEffectParams.playerTime)
                .setEase(LeanTweenType.easeOutExpo)
                .setOnComplete(() => {
                    // Hack
                    RigidBodyInputHandler handler = (RigidBodyInputHandler)inputHandler.InputController;
                    if (handler != null)
                        handler.PerformCutJump();

                    SetInputHandlerMovemet(true);
                });
            });
    }

    private void HandleRotation()
    {
        float xRotation = inputHandler.PlayerAction.Move.X * rotationSpeed;
        float yRotation = -inputHandler.PlayerAction.Move.Y * rotationSpeed;

        rotationWorld.Set(0f, xRotation, 0f);
        rotationLocal.Set(0f, 0f, yRotation);

        transform.Rotate(rotationWorld * Time.deltaTime, Space.World);
        transform.Rotate(rotationLocal * Time.deltaTime, Space.Self);

        // Clamp angles
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, MathUtil.ClampAngle(transform.localEulerAngles.z, 270f - maxZRotationOffset, 270f + maxZRotationOffset));
        inputHandler.transform.Rotate(Vector3.up, xRotation * Time.deltaTime);
    }

    private void HandleIsoLineParameters()
    {
        isoLine.LineDirection = transform.up;
    }

    /// <summary>
    /// This method must be called before this script can do anything.
    /// </summary>
    public void Initialize(RigidBodyInput input)
    {
        inputHandler = input;
        transform.rotation = Quaternion.LookRotation(inputHandler.transform.forward, input.transform.right);
        input.Rigid.useGravity = false;
        input.Rigid.velocity = Vector3.zero;
        input.Rigid.constraints = RigidbodyConstraints.FreezeAll;

        Canvas playerCanvas = input.gameObject.FindComponentInChildWithTag<Canvas>(PLAYER_CANVAS_TAG);
        if (playerCanvas != null)
        {
            playerCutText = playerCanvas.gameObject.FindComponentInChildWithTag<Text>(PLAYER_CUT_TEXT_TAG);
            LeanTween.value(0f, 1f, 0.1f).setEase(LeanTweenType.easeInCirc)
                .setOnUpdate((float value) => {
                    SetCutTimeAlpha(value);
                });
        }

        SetInputHandlerMovemet(false);
        isInitialized = true;
    }

    /// <summary>
    /// Destroy plane and give the player the controls back.
    /// </summary>
    private void DestroyPlaneControl()
    {
        PerformDestructionTweens();

        inputHandler.Rigid.constraints = RigidbodyConstraints.None;
        inputHandler.Rigid.constraints = RigidbodyConstraints.FreezeRotation;

        inputHandler.Rigid.useGravity = true;
        inputHandler.StartCoroutine(WaitForDestruction());
    }

    private void DestroyPlaneDueToGameEnd()
    {
        isUsed = true;
        PerformDestructionTweens();
        inputHandler.StartCoroutine(WaitForDestruction());
    }

    private void PerformDestructionTweens()
    {
        float time = waitForDestruction * 0.5f;
        LeanTween.scale(gameObject, Vector3.zero, time).setEase(LeanTweenType.easeInBack);
        LeanTween.value(gameObject, isoLine.LineCount, 20f, time).setEase(LeanTweenType.easeInBack)
            .setOnUpdate((float value) => {
                isoLine.LineCount = value;
            });

        LeanTween.value(gameObject, originalLineAlpha, 0f, time).setEase(LeanTweenType.easeInBack)
             .setOnUpdate((float value) => {
                 isoLine.LineColor = new Color(isoLine.LineColor.r, isoLine.LineColor.g, isoLine.LineColor.b, value);
             });

        // Cut text
        LeanTween.value(1f, 0f, 0.1f).setEase(LeanTweenType.easeInCirc)
                .setOnUpdate((float value) => {
                    SetCutTimeAlpha(value);
                }).setDelay(0.6f);
    }

    private IEnumerator WaitForDestruction()
    {
        yield return new WaitForSeconds(waitForDestruction);

        isoLine.enabled = false;
        isoLine.LineCount = originalLineCount;
        isoLine.LineColor = new Color(isoLine.LineColor.r, isoLine.LineColor.g, isoLine.LineColor.b, originalLineAlpha);

        if(gameObject != null)
            Destroy(gameObject);
    }

    private void SetInputHandlerMovemet(bool movementAllowed)
    {
        inputHandler.AllowMovement = movementAllowed;
    }

    private void SetCutTimeAlpha(float alpha)
    {
        playerCutText.color = new Color(playerCutText.color.r, playerCutText.color.g, playerCutText.color.b, alpha);
    }

    private void OnDestroy()
    {
        gameManager.OnGameEnded -= DestroyPlaneDueToGameEnd;
    }
}