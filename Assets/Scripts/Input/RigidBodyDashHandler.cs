using UnityEngine;
using System.Collections;

public class RigidBodyDashHandler : DashHandler
{
    private static string DASH_AXIS = "Fire2";
    private static string PLAYER_TAG = "Player";

    #region Internal Members
    private RigidBodyInput player;
    private Collider playerCollider;

    private bool isDashAllowed = true;
    private LTDescr dashTween = null;

    private WaitForSeconds waitForCooldown;
    #endregion

    #region Events
    /// <summary>
    /// Is called when a valid (Ground or Player Layer) collision is detected.
    /// </summary>
    public event DashCollisionHandler OnDashCollision;

    /// <summary>
    /// Is called when a dash is initiated.
    /// </summary>
    public event DashStartedHandler OnDashStarted;
    #endregion

    public RigidBodyDashHandler(RigidBodyInput player)
    {
        this.player = player;
        this.playerCollider = this.player.GetComponent<Collider>();
        this.waitForCooldown = new WaitForSeconds(player.DashCoolDownTime);
        OnDashCollision += HandlePlayerDashCollision;
    }

    /// <summary>
    /// When a collision happens between between 2 players, the other player will be pushed back.
    /// </summary>
    private void HandlePlayerDashCollision(GameObject self, GameObject other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            Rigidbody rigid = player.gameObject.GetComponent<Rigidbody>();
            if(rigid != null)
            {
                Vector3 force = other.transform.position - self.transform.position;
                rigid.AddForce(force.normalized * this.player.DashPushBackForce, ForceMode.VelocityChange);
                //Debug.Log("Force on " + other.name + " !");
            }
        }
    }

    public void HandleDash()
    {
        PerformDashProcedure();
    }

    private void PerformDashProcedure()
    {
        bool dashButton = player.PlayerAction.Dash.WasPressed;
        if (player.AllowMovement && dashButton && isDashAllowed)
        {
            isDashAllowed = false;
            ApplyDashMovement();
            player.StartCoroutine(WaitForDashCooldown());
        }
    }

    private void ApplyDashMovement()
    {
        RaycastHit hitInfo;
        Vector3 forward = Vector3.zero;

        // Calculate forward: Use ground normal or player normal when in the air
        if (CheckPlayerGrounded(out hitInfo))
            forward = Vector3.ProjectOnPlane(player.transform.forward, hitInfo.normal).normalized;
        else
            forward = player.transform.forward;

        Debug.DrawLine(player.transform.position, player.transform.position + forward * player.DashForce, Color.blue, 1f, true);
        AddForce(forward * player.DashForce);
    }

    private void AddForce(Vector3 forceVector)
    {
        player.Rigid.velocity = Vector3.zero;

        if (CheckDashCollision())
        {
            return;
        }
        
        dashTween = LeanTween.value(player.gameObject, player.transform.position, player.transform.position + forceVector, player.DashTime)
            .setOnUpdate((Vector3 value) => {
                player.Rigid.MovePosition(value);

                // Cancel dash when a collsion occurs
                if (CheckDashCollision())
                    LeanTween.cancel(dashTween.uniqueId);
            })
            .setOnStart(() => {
                OnPerformStart();
            })
            .setEase(LeanTweenType.easeOutCirc);
    }

    private bool CheckDashCollision()
    {
        int layerMaskPlayer = 1 << player.gameObject.layer;
        int layerMaskGround = 1 << player.GroundedLayer;

        Collider[] playerColliders = Physics.OverlapSphere(GetPlayerCenter(), player.DashCheckRadius, layerMaskPlayer);
        Collider[] groundCollider = Physics.OverlapSphere(GetPlayerCenter(), player.DashGroundCheckRadius, layerMaskGround);
        
        Ray ray = new Ray(playerCollider.bounds.center,Vector3.forward);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit, 1.0f, layerMaskGround))
        {
            return true;
        }
       
#if UNITY_EDITOR
        DebugExtension.DebugWireSphere(GetPlayerCenter(), Color.red, player.DashCheckRadius);
        DebugExtension.DebugWireSphere(GetPlayerCenter(), Color.green, player.DashGroundCheckRadius);

#endif
        return IsValidCollision(playerColliders) || IsValidCollision(groundCollider);
    }

    /// <summary>
    /// Checks if one of the collider is a valid ground or another player collider.
    /// </summary>
    private bool IsValidCollision(Collider[] collider)
    {
        foreach(Collider c in collider)
        {
            // Collision with other object than a player
            if (c.gameObject.GetInstanceID() != player.gameObject.GetInstanceID())
            {
                OnPerformDashCollision(player.gameObject, c.gameObject);
                return true;
            }
        }
        return false;
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

    private Vector3 GetPlayerCenter()
    {
        return playerCollider.bounds.center + player.transform.forward * player.DashCollisionForwardOffset;
    }

    private IEnumerator WaitForDashCooldown()
    {
        yield return waitForCooldown;
        isDashAllowed = true;
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