using UnityEngine;

public delegate void DashStartedHandler();
public delegate void DashCollisionHandler(GameObject self, GameObject other);

public interface DashHandler
{
    #region Events
    event DashStartedHandler OnDashStarted;
    event DashCollisionHandler OnDashCollision;
    #endregion

    void PerformDash();
}