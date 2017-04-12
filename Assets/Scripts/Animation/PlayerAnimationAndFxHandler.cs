using System.Collections;
using System.Collections.Generic;
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
    #endregion

    #region Internal Members
    private RigidBodyInput inputHandler;
    #endregion

    private void Start ()
    {
        inputHandler = GetComponent<RigidBodyInput>();
        
        // Input Controller events
        inputHandler.InputController.OnJump += HandleFirstJump;
        inputHandler.InputController.OnSecondJump += HandleSecondJump;
    }

    private void HandleFirstJump()
    {
        if (firstJumpParticle != null)
            Instantiate(firstJumpParticle, transform.position + particleSpawnOffset, firstJumpParticle.transform.rotation);
    }

    private void HandleSecondJump()
    {
        if (secondJumpParticle != null)
            Instantiate(secondJumpParticle, transform.position + particleSpawnOffset, secondJumpParticle.transform.rotation);
    }
}
