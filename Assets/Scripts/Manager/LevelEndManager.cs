using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndManager : MonoBehaviour
{
    [Comment("Handles routines which should be called at the end of the game (scene change, etc, ...).")]

    #region Inspector variables
    [FancyHeader("Settings")]
    [SerializeField]
    private float sceneChangeDelay = 3f;
    #endregion

    #region Internal variables
    private PlayerManager playerManager;
    private bool alreadyCalled = false;
    #endregion

    private void Start ()
    {
        InitializePlayerManager();
	}

    private void InitializePlayerManager()
    {
        playerManager = FindObjectOfType<PlayerManager>();

        if (playerManager == null)
            Debug.LogError("[UIManager]: No PlayerManager found!", gameObject);
        else
        {
            playerManager.OnAllPlayersDied += HandleLevelEnd;
            playerManager.OnOnePlayerLeft += (Player player) => { HandleLevelEnd(); };
        }
    }

    private void HandleLevelEnd()
    {
        if (!alreadyCalled)
        {
            // TODO: Add level end stuff here (Change scene, etc...)
            alreadyCalled = true;
        }
    }
}