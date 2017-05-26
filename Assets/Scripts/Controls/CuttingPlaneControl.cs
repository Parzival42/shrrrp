using System.Collections;
using UnityEngine;

public class CuttingPlaneControl : MonoBehaviour
{
    #region Inspector variables
    [FancyHeader("Control settings", "Translation, Rotation, ...")]
    [SerializeField]
    private float rotationSpeed = 100f;

    [SerializeField]
    private float translationSpeed = 100f;

    [SerializeField]
    private float waitForDestruction = 0.8f;

    [FancyHeader("Tween settings")]
    [SerializeField]
    private float isoLineTweenTime = 0.6f;

    [FancyHeader("Cut effect parameters")]
    [SerializeField]
    private CuttingEffectParameters cutEffectParams;
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
    #endregion

    private void Start()
    {
        cam = CameraUtil.GetMainCamera();

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
            CuttingManagerLocator.GetInstance.Cut(transform, GetComponent<MeshFilter>().mesh);

            inputHandler.InputController.OnPlayerCutMove();
            DestroyPlaneControl();
        }
    }

    private void PositionPlayerDuringCut()
    {
        LeanTween.moveY(inputHandler.gameObject, inputHandler.transform.position.y + cutEffectParams.playerHeightOffset, cutEffectParams.playerTime).setEase(cutEffectParams.playerEase);
    }

    private void HandleRotation()
    {
        float xRotation = inputHandler.PlayerAction.Move.X * rotationSpeed;
        float yRotation = -inputHandler.PlayerAction.Move.Y * rotationSpeed;
        rotationWorld.Set(0f, xRotation, 0f);

        rotationLocal.Set(0f, 0f, yRotation);

        transform.Rotate(rotationWorld * Time.deltaTime, Space.World);
        transform.Rotate(rotationLocal * Time.deltaTime, Space.Self);

        inputHandler.transform.Rotate(Vector3.up, xRotation * Time.deltaTime);
    }

    private void HandleTranslation()
    {
        // TODO: Translate to local x and z direction, but with a projected direction on the world x-z plane
        translation.Set(inputHandler.PlayerAction.RightStick.Y * translationSpeed, 0f, -inputHandler.PlayerAction.RightStick.X * translationSpeed);

        translation = Vector3.ProjectOnPlane(translation, Vector3.up);
        transform.Translate(translation * Time.deltaTime, Space.World);
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
        transform.up = inputHandler.transform.right;
        input.Rigid.useGravity = false;
        input.Rigid.velocity = Vector3.zero;
        input.Rigid.constraints = RigidbodyConstraints.FreezeAll;

        SetInputHandlerMovemet(false);
        isInitialized = true;
    }

    /// <summary>
    /// Destroy plane and give the player the controls back.
    /// </summary>
    private void DestroyPlaneControl()
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

        inputHandler.Rigid.constraints = RigidbodyConstraints.None;
        inputHandler.Rigid.constraints = RigidbodyConstraints.FreezeRotation;

        inputHandler.Rigid.useGravity = true;
        inputHandler.StartCoroutine(WaitForDestruction());
    }

    IEnumerator WaitForDestruction()
    {
        yield return new WaitForSeconds(waitForDestruction);
        SetInputHandlerMovemet(true);

        isoLine.enabled = false;
        isoLine.LineCount = originalLineCount;
        isoLine.LineColor = new Color(isoLine.LineColor.r, isoLine.LineColor.g, isoLine.LineColor.b, originalLineAlpha);

        Destroy(gameObject);
    }

    private void SetInputHandlerMovemet(bool movementAllowed)
    {
        inputHandler.AllowMovement = movementAllowed;
    }
}