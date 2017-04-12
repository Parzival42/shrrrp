using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyDashHandler : DashHandler
{
    private static string DASH_AXIS = "Fire2";

    #region Internal Members
    private RigidBodyInput player;

    private bool isDashPerforming = false;
    private LTDescr dashTween = null;
    #endregion

    #region Events
    public event DashCollisionHandler OnDashCollision;
    public event DashStartedHandler OnDashStarted;
    #endregion

    public RigidBodyDashHandler(RigidBodyInput player)
    {
        this.player = player;
    }

    public void HandleDash()
    {
        PerformDashProcedure();
    }

    private void PerformDashProcedure()
    {
        bool dashButton = Input.GetButtonDown(DASH_AXIS);

        // TODO: Check collision during dash.
        if (dashButton && !isDashPerforming)
        {
            ApplyDashMovement();
        }
    }

    private void ApplyDashMovement()
    {
        RaycastHit hitInfo;
        Vector3 forward = Vector3.zero;

        // Calculate forward use ground normal or player normal when in the air
        if (CheckPlayerGrounded(out hitInfo))
            forward = Vector3.ProjectOnPlane(player.transform.forward, hitInfo.normal).normalized;
        else
            forward = player.transform.forward;

        Debug.DrawLine(player.transform.position, player.transform.position + forward * player.DashForce, Color.blue, 1f);
        AddForce(forward * player.DashForce);
    }

    private void AddForce(Vector3 forceVector)
    {
        player.Rigid.velocity = Vector3.zero;

        dashTween = LeanTween.value(player.gameObject, player.transform.position, player.transform.position + forceVector, player.DashTime)
            .setOnUpdate((Vector3 value) => {
                player.Rigid.MovePosition(value);
            })
            .setOnStart(() => {
                isDashPerforming = true;
                OnPerformStart();
            })
            .setOnComplete(() => {
                isDashPerforming = false;
            })
            .setEase(LeanTweenType.easeOutCirc);
    }

    private bool CheckPlayerGrounded(out RaycastHit hitInfo)
    {
        Ray ray = new Ray(player.transform.position + player.GroundPivotOffset, Vector3.down);
        bool groundWasHit = Physics.Raycast(ray, out hitInfo, player.DashGroundCheckDistance, 1 << player.GroundedLayer, QueryTriggerInteraction.UseGlobal);

#if UNITY_EDITOR
        if (groundWasHit)
            Debug.DrawRay(ray.origin, ray.direction * player.GroundedDistance, Color.blue, 1f);
#endif
        return groundWasHit;
    }

    #region Event methods
    private void OnPerformStart()
    {
        if (OnDashStarted != null)
            OnDashStarted();
    }

    private void OnPerformDashCollision(GameObject self, GameObject other)
    {
        if (OnDashCollision != null)
            OnDashCollision(self, other);
    }
    #endregion
}