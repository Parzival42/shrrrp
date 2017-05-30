using System.Collections.Generic;
using UnityEngine;
using InControl;

/// <summary>
/// Contains the player selection information and
/// level information which was filled during the menu life cycle.
///
/// The player information for example should be used to spawn the players.
/// If there is no MenuSelectionContainer in the scene, it can be assumed that
/// the scene is started in the editor.
/// </summary>
public class MenuSelectionContainer : MonoBehaviour
{
    #region Data (Should be filled in menu)
    [FancyHeader("Level information")]
    public string levelName;

    public GameObject playerPrefab;

    public List<PlayerInfo> playerData = new List<PlayerInfo>();
    #endregion

    #region Methods
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Resets all data which should not persist after a new menu cycle.
    /// </summary>
    public void ResetContainer()
    {
        levelName = "";

        foreach (PlayerInfo info in playerData)
            info.playerInputDevice = null;
        playerData.Clear();
    }
    #endregion

    #region Inner class
    public class PlayerInfo
    {
        public InputDevice playerInputDevice;
        public PlayerType playerType;

        public PlayerInfo(InputDevice inputDevice, PlayerType playerType)
        {
            this.playerInputDevice = inputDevice;
            this.playerType = playerType;
        }
    }
    #endregion
}