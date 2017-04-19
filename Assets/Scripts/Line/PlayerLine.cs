using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class PlayerLine : MonoBehaviour
{
    #region Inspector variables
    [Header("Settings")]
    [SerializeField]
    private int collisionLayer = 8;

    [SerializeField]
    private float maxCollisionDistance = 50f;


    [Header("Tween parameters")]
    [SerializeField]
    private float lineTweenTime = 0.1f;

    [SerializeField]
    private LeanTweenType lineTweenEase = LeanTweenType.easeInCirc;

    [SerializeField]
    private float secondJumpLineWidthMultiplier = 1.5f;

    [SerializeField]
    private float secondJumpTweenTime = 0.2f;

    [Header("Player Circle")]
    [SerializeField]
    private Image playerCircle;

    [SerializeField]
    private float circleYOffset = 0.1f;

    [SerializeField]
    private float sizeTarget = 1.4f;

    [SerializeField]
    private float circleTweenTime = 0.3f;
    #endregion

    #region Internal Members
    private LineRenderer playerLine;

    // An Input component is required in the parent GameObject with an InputHandler!
    private InputHandler inputHandler;

    private float originalPlayerLineWidth;
    #endregion

    private void Start ()
    {
        Initialize();
        CheckForErrors();
	}

    private void Initialize()
    {
        playerLine = GetComponent<LineRenderer>();
        inputHandler = transform.parent.GetComponent<RigidBodyInput>().InputController;
        inputHandler.OnJump += HandleJumpCircleTween;
        inputHandler.OnLandedOnGround += HandleLandedCircleTween;
        inputHandler.OnJump += JumpLineTween;
        inputHandler.OnSecondJump += SecondJumpTween;

        originalPlayerLineWidth = playerLine.widthMultiplier;
        SetZeroAlphaPlayerCircle();
    }

    private void CheckForErrors()
    {
        if (inputHandler == null)
            Debug.LogError("No input handler in parent object found! Check hierarchy!", gameObject);

        if (playerCircle == null)
            Debug.LogError("No player circle specified!", gameObject);
    }

	private void Update ()
    {
        SetLinePositions();
        HandleLineVisibility();
    }

    private void HandleLineVisibility()
    {
        if (inputHandler.IsGrounded)
            playerLine.enabled = false;
        else
            playerLine.enabled = true;
    }

    private void JumpLineTween()
    {
        playerLine.widthMultiplier = 0f;

        LeanTween.value(gameObject, 0f, originalPlayerLineWidth, lineTweenTime)
            .setOnUpdate((float value) => {
                playerLine.widthMultiplier = value;
            }).setEase(lineTweenEase);
    }

    private void SecondJumpTween()
    {
        LeanTween.value(gameObject, originalPlayerLineWidth, originalPlayerLineWidth * secondJumpLineWidthMultiplier, secondJumpTweenTime * 0.5f)
            .setOnUpdate((float value) => {
                playerLine.widthMultiplier = value;
            })
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() => {
                LeanTween.value(playerLine.widthMultiplier, originalPlayerLineWidth, secondJumpTweenTime * 0.5f)
                    .setOnUpdate((float value) => {
                        playerLine.widthMultiplier = value;
                    })
                    .setEase(LeanTweenType.easeOutQuad);
            });
    }

    private void SetLinePositions()
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = Vector3.zero;

        RaycastHit hitInfo;
        bool hit = Physics.Raycast(startPosition, Vector3.down, out hitInfo, maxCollisionDistance, 1 << collisionLayer);

        if (hit)
        {
            endPosition = hitInfo.point;
            SetCircleTransformation(endPosition, hitInfo.normal);
        }
        else
        {
            endPosition = startPosition + Vector3.down * maxCollisionDistance;
            SetCircleTransformation(endPosition, Vector3.up);
        }

        playerLine.SetPosition(0, startPosition);
        playerLine.SetPosition(1, endPosition);
    }

    private void SetCircleTransformation(Vector3 groundHit, Vector3 normal)
    {
        playerCircle.transform.position = groundHit + Vector3.up * circleYOffset;
        playerCircle.transform.forward = normal;
    }

    private void HandleLandedCircleTween()
    {
        playerCircle.rectTransform.localScale = Vector3.one;
        LeanTween.alpha(playerCircle.rectTransform, 0f, circleTweenTime).setEase(LeanTweenType.easeOutExpo);
        LeanTween.scale(playerCircle.rectTransform, Vector3.one * sizeTarget, circleTweenTime).setEase(LeanTweenType.easeOutExpo);
    }

    private void HandleJumpCircleTween()
    {
        SetZeroAlphaPlayerCircle();
        playerCircle.rectTransform.localScale = Vector3.one * sizeTarget;

        LeanTween.alpha(playerCircle.rectTransform, 1f, circleTweenTime).setEase(LeanTweenType.easeOutExpo);
        LeanTween.scale(playerCircle.rectTransform, Vector3.one, circleTweenTime).setEase(LeanTweenType.easeOutExpo);
    }

    private void SetZeroAlphaPlayerCircle()
    {
        playerCircle.color = new Color(playerCircle.color.r, playerCircle.color.g, playerCircle.color.b, 0f);
    }
}