using System.Collections;
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

    public ArrayList playerData = new ArrayList();
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
            info.playerAction.Destroy();
        playerData.Clear();
    }
    #endregion

    #region Inner class
    public class PlayerInfo
    {
        public StandardPlayerAction playerAction;
        public PlayerType playerType;

        public PlayerInfo(StandardPlayerAction playerAction, PlayerType playerType)
        {
            this.playerAction = playerAction;
            this.playerType = playerType;
        }

        public override bool Equals(object obj){
            if(obj.GetType() == typeof(PlayerInfo)){
                PlayerInfo other = (PlayerInfo)obj;
                return this.playerType == other.playerType && this.playerAction.Device.GUID.Equals(other.playerAction.Device.GUID);
            }
            return false;
        }

        public override int GetHashCode(){
            return this.playerAction.Device.GUID.GetHashCode() + 421 * (int)this.playerType;
        }

    }
    #endregion
}