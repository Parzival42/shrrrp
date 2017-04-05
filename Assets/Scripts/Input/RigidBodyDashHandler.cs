using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyDashHandler : DashHandler
{
    #region Events
    public event DashCollisionHandler OnDashCollision;
    public event DashStartedHandler OnDashStarted;
    #endregion

    public void PerformDash()
    {
        // TODO: Implement shit
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