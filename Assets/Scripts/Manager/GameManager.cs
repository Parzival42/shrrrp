using UnityEngine;

public delegate void GameEnd();

/// <summary>
/// Processes events like player death, win situations etc.
/// Manages the general flow of the current game session.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Inspector variables
    #endregion

    #region Internal variables
    private PlayerManager playerManager;
    public event GameEnd OnGameEnded;
    #endregion

    private void Start ()
    {
        Initialize();
	}

    private void Initialize()
    {
        // Find PlayerManager
        playerManager = FindObjectOfType<PlayerManager>();
        if (playerManager == null)
            Debug.LogError("What the hell, <b>no PlayerManager</b> in the scene?", gameObject);
        else
        {
            playerManager.OnAllPlayersDied += HandleAllPlayersDied;
            playerManager.OnPlayerDied += HandlePlayerDeath;
            playerManager.OnOnePlayerLeft += HandleOnePlayerLeft;
        }
    }

    /// <summary>
    /// Shouldn't really happen in real game, only in editor if you spawn with 1 player.
    /// </summary>
    private void HandleAllPlayersDied()
    {
        FireOnGameEnded();
        Debug.Log("Well, this shouldn't really happen (in game), but all players died :O. If you are in the editor everything is ok ;).");
    }

    /// <summary>
    /// This player wins!
    /// </summary>
    private void HandleOnePlayerLeft(Player player)
    {
        FireOnGameEnded();

        // Deactivate controls and disable physics!
        RigidBodyInput input = player.GetComponent<RigidBodyInput>();
        input.Rigid.constraints = RigidbodyConstraints.FreezeAll;
        input.AllowMovement = false;

        // TODO: After some time and fancy UI text, change to the menu scene or so.
    }

    private void HandlePlayerDeath(Player player)
    {
        // TODO: Maybe show some fancy text or play sound etc...
    }

    protected void FireOnGameEnded()
    {
        if (OnGameEnded != null)
            OnGameEnded();
    }
}