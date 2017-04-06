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
        if (dashButton)
        {
            ApplyDashMovement();
        }
    }

    private void ApplyDashMovement()
    {
        Vector3 forward = player.transform.forward;
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