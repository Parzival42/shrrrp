using System.Collections.Generic;
using UnityEngine;
using InControl;

/// <summary>
/// Contains the player selection information and
/// level information which was filled during the menu life cycle.
///
/// The player information for example should be used to spawn the players.
/// If there is nu MenuSelectionContainer in the scene, it can be assumed that
/// the scene is started in the editor.
/// </summary>
public class MenuSelectionContainer : MonoBehaviour
{
    #region Inspector
    [Header("Level information")]
    public string levelName;

    [Header("Beware of the order of the prefabs! Should be the same like in character menu.")]
    [Tooltip("The order of the character prefabs should be the same order like in the character menu."
        + " If wrong characters are spawned, check this order first!")]
    public GameObject[] playerPrefabs;
    #endregion

    #region Data (Should be filled in menu)
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
        public int playerPrefabIndex;

        public PlayerInfo(InputDevice inputDevice, int prefabIndex)
        {
            this.playerInputDevice = inputDevice;
            this.playerPrefabIndex = prefabIndex;
        }
    }
    #endregion
}
