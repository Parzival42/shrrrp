using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    #endregion

    #region Internal Members
    private LineRenderer playerLine;

    // An Input component is required in the parent GameObject with an InputHandler!
    private InputHandler inputHandler;

    private float originalPlayerLineWidth;

    private LTDescr jumpTween;
    #endregion

    private void Start ()
    {
        playerLine = GetComponent<LineRenderer>();
        inputHandler = transform.parent.GetComponent<RigidBodyInput>().InputController;
        inputHandler.OnJump += JumpLineTween;
        inputHandler.OnSecondJump += SecondJumpTween;

        if (inputHandler == null)
            Debug.LogError("No input handler in parent object found! Check hierarchy!", gameObject);

        originalPlayerLineWidth = playerLine.widthMultiplier;
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

        CancelJumpTween();
        jumpTween = LeanTween.value(gameObject, 0f, originalPlayerLineWidth, lineTweenTime)
            .setOnUpdate((float value) => {
                playerLine.widthMultiplier = value;
            }).setEase(lineTweenEase);
    }

    private void SecondJumpTween()
    {
        CancelJumpTween();
        jumpTween = LeanTween.value(gameObject, originalPlayerLineWidth, originalPlayerLineWidth * secondJumpLineWidthMultiplier, secondJumpTweenTime * 0.5f)
            .setOnUpdate((float value) => {
                playerLine.widthMultiplier = value;
            })
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() => {
                CancelJumpTween();
                jumpTween = LeanTween.value(playerLine.widthMultiplier, originalPlayerLineWidth, secondJumpTweenTime * 0.5f)
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
            endPosition = hitInfo.point;
        else
            endPosition = startPosition + Vector3.down * maxCollisionDistance;
        
        playerLine.SetPosition(0, startPosition);
        playerLine.SetPosition(1, endPosition);
    }

    private void CancelJumpTween()
    {
        if (jumpTween != null)
            LeanTween.cancel(jumpTween.id);
    }
}