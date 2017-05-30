using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using InControl;

public class PlayerLobby : MonoBehaviour {

    [FancyHeader("Settings", "Assign PlayerPrefab used in game and PlayerTypes spawned")]
	[SerializeField]
	private GameObject playerPrefab;

    [SerializeField]
    private PlayerType[] playerTypes;

    #region internal members
    private bool[] isPlayerTypeUsed;
    private List<GameObject> currentPlayers;
	private MenuSelectionContainer menuSelectionContainer;
    #endregion

	void Start () {
		// destroy the clouds and menu lights
		Destroy(GameObject.FindGameObjectWithTag("MenuEnvironment"));

		// get the menu selection container to add player info and assign the player prefab to the info
		menuSelectionContainer = (MenuSelectionContainer)FindObjectOfType(typeof(MenuSelectionContainer));
		menuSelectionContainer.playerPrefab = playerPrefab;

        isPlayerTypeUsed = new bool[playerTypes.Length];
        currentPlayers = new List<GameObject>();

        InputManager.OnDeviceDetached += inputDevice => OnDeviceDetach(inputDevice);
	}
	
	void Update () {
		UpdatePlayerDevices();
	}

	private void UpdatePlayerDevices()
    {
        if(menuSelectionContainer.playerData.Count < 4){

            InputDevice activeDevice = InputManager.ActiveDevice;

            // Determine if device is in use already
            // Iterate in reverse order to avoid concurrency issues
            bool deviceRegistered = false;
            foreach(MenuSelectionContainer.PlayerInfo data in menuSelectionContainer.playerData.AsEnumerable().Reverse()){
                if(data.playerInputDevice.Equals(activeDevice)){
                    deviceRegistered = true;
                    break;
                }
            }

            if(!deviceRegistered){
                CreateAndSpawnPlayer(activeDevice);
            }
        }
    }

    /// <summary>
    /// Returns the first found unused PlayerType in the playerType array, returns 
    /// PlayerType of NullPlayer if none is found (this should not happen anyways).
    /// </summary>
    private PlayerType GetFirstUnusedPlayerType(){
        for(int i = 0; i < playerTypes.Length; i++){
            if(!isPlayerTypeUsed[i]){
                isPlayerTypeUsed[i] = true;
                return playerTypes[i];
            }
        }
        return PlayerType.NullPlayer;
    }

    private void CreateAndSpawnPlayer(InputDevice device){
        // Get unused PlayerType and add player to MenuSelectionContainer
        PlayerType type = GetFirstUnusedPlayerType();
        menuSelectionContainer.playerData.Add(new MenuSelectionContainer.PlayerInfo(device, type));

        // Spawn player at specific spawn position
        GameObject spawnedPlayer = SpawnPointHelper.SpawnPlayer(menuSelectionContainer.playerPrefab, type);
        RigidBodyInput input = spawnedPlayer.GetComponent<RigidBodyInput>();

        // Add standard input binding
        input.PlayerAction = StandardPlayerAction.CreateStandardBinding(InputManager.ActiveDevice);

        // Add new player to currentPlayers list
        currentPlayers.Add(spawnedPlayer);
    }

    /// <summary>
    /// When a device is detached, remove PlayerInfo from MenuSelectionContainer and
    /// despawn/destroy the player connected to it.
    /// </summary>
    private void OnDeviceDetach(InputDevice device){
        for(int i = menuSelectionContainer.playerData.Count - 1; i > -1; i--){
            MenuSelectionContainer.PlayerInfo data = menuSelectionContainer.playerData[i];
            if(data.playerInputDevice.Equals(device)){
                // Reset PlayerType array and MenuSelectionContainer
                isPlayerTypeUsed[i] = false;
                menuSelectionContainer.playerData.Remove(data);

                // Destroy the PlayerAction and the player
                GameObject player = currentPlayers[i];
                player.GetComponent<RigidBodyInput>().PlayerAction.Destroy();
                Destroy(player);
            }
        }
    }

    void OnDisable(){
        InputManager.OnDeviceDetached -= inputDevice => OnDeviceDetach(inputDevice);
    }
}
