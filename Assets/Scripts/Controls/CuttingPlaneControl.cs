using System.Collections;
using System.Collections.Generic;
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
    #endregion

    #region Internal variables
    private Camera cam;
    private bool isInitialized = false;
    private bool isUsed = false;
    private RigidBodyInput inputHandler;
    private Vector3 rotationWorld = Vector3.zero;
    private Vector3 rotationLocal = Vector3.zero;
    private Vector3 translation = Vector3.zero;
    #endregion

    private void Start()
    {
        cam = CameraUtil.GetMainCamera();
    }

    private void Update ()
    {
        if (isInitialized)
            HandleControls();
	}

    private void HandleControls()
    {
        HandleRotation();
        //HandleTranslation();

        if (!isUsed && (inputHandler.PlayerAction.Jump.WasPressed || inputHandler.PlayerAction.Dash.WasPressed))
        {
            isUsed = true;

            // TODO: Cut the shit out of the mesh!
            Debug.Log("Cut the shit out of the mesh!");

            CuttingManager.CuttingManagerInstance.Cut(transform, GetComponent<MeshFilter>().mesh);

            inputHandler.InputController.OnPlayerCutMove();
            DestroyPlaneControl();
        }
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

    /// <summary>
    /// This method must be called before this script can do anything.
    /// </summary>
    public void Initialize(RigidBodyInput input)
    {
        inputHandler = input;
        transform.up = inputHandler.transform.right;

        SetInputHandlerMovemet(false);
        isInitialized = true;
    }

    /// <summary>
    /// Destroy plane and give the player the controls back.
    /// </summary>
    private void DestroyPlaneControl()
    {
        inputHandler.StartCoroutine(WaitForDestruction());
    }

    IEnumerator WaitForDestruction()
    {
        yield return new WaitForSeconds(waitForDestruction);
        SetInputHandlerMovemet(true);
        Destroy(gameObject);
    }

    private void SetInputHandlerMovemet(bool movementAllowed)
    {
        inputHandler.AllowMovement = movementAllowed;
    }
}