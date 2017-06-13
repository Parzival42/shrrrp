﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using InControl;
using Prime31.TransitionKit;

public class PlayerLobby : MonoBehaviour {

    [FancyHeader("Settings", "Assign PlayerPrefab used in game and PlayerTypes spawned")]
	[SerializeField]
	private GameObject playerPrefab;

    [SerializeField]
    private PlayerType[] playerTypes;

    [SerializeField]
    private PlayerButton[] playerButtons;

    [SerializeField]
    private PlayerButton[] backButtons;

    [SerializeField]
    private Shader transitionShader;

    [SerializeField]
    private float transitionTimeout = 0.2f;

    #region internal members
    private bool[] isPlayerTypeUsed;
    private List<GameObject> currentPlayers;
	private MenuSelectionContainer menuSelectionContainer;
    private StandardPlayerAction defaultGamepadAction;
    private StandardPlayerAction defaultKeyboardAction;
    private bool keyboardRegistered = false;
    private bool isSwitchingScene = false;
    #endregion

	void Start () {
		// destroy the clouds and menu lights
		Destroy(GameObject.FindGameObjectWithTag("MenuEnvironment"));

		// get the menu selection container to add player info and assign the player prefab to the info
		menuSelectionContainer = (MenuSelectionContainer)FindObjectOfType(typeof(MenuSelectionContainer));
		menuSelectionContainer.playerPrefab = playerPrefab;

        isPlayerTypeUsed = new bool[playerTypes.Length];
        currentPlayers = new List<GameObject>();
        defaultGamepadAction = StandardPlayerAction.CreateStandardGamePadBinding();
        defaultKeyboardAction = StandardPlayerAction.CreateStandardKeyboardBinding();

        InputManager.OnDeviceDetached += inputDevice => OnDeviceDetach(inputDevice);
	}
	
	void Update () {
        if(!isSwitchingScene){
            if(IsTimeToStart())
                StartCoroutine(SwitchScene(menuSelectionContainer.levelName));
            if(IsTimeToGoBack())
                StartCoroutine(SwitchScene("LevelSelection"));
        }
        IsTimeToGoBack();

		UpdatePlayerDevices();
	}

	private void UpdatePlayerDevices(){
        if(menuSelectionContainer.playerData.Count < 4){
            
            if(defaultGamepadAction.Jump.WasPressed){
                if(!IsDeviceRegistered(defaultGamepadAction)){
                    CreateAndSpawnPlayer(defaultGamepadAction.ActiveDevice, false);
                }
            }
            if (defaultKeyboardAction.Jump.WasPressed){
                if(!keyboardRegistered){
                    CreateAndSpawnPlayer(defaultKeyboardAction.ActiveDevice, true);
                    keyboardRegistered = true;
                }
            }
        }
    }

    /// <summary>
    /// Determines if device is in use already.
    /// </summary>
    private bool IsDeviceRegistered(StandardPlayerAction action){
        bool deviceRegistered = false;
        foreach(MenuSelectionContainer.PlayerInfo data in menuSelectionContainer.playerData.AsEnumerable().Reverse()){
            if(data.playerAction.Device.GUID.Equals(action.ActiveDevice.GUID)){
                deviceRegistered = true;
                break;
            }
        }
        return deviceRegistered;
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

    private void CreateAndSpawnPlayer(InputDevice device, bool isKeyboard){
        // Get unused PlayerType
        PlayerType type = GetFirstUnusedPlayerType();

        // Spawn player at specific spawn position
        GameObject spawnedPlayer = SpawnPointHelper.SpawnPlayer(menuSelectionContainer.playerPrefab, type);
        RigidBodyInput input = spawnedPlayer.GetComponent<RigidBodyInput>();

        // Add standard input binding
        if(isKeyboard)
            input.PlayerAction = StandardPlayerAction.CreateStandardKeyboardBinding(device);
        else
            input.PlayerAction = StandardPlayerAction.CreateStandardGamePadBinding(device);

        // Add new player to currentPlayers list
        currentPlayers.Add(spawnedPlayer);
        
        // Add player to MenuSelectionContainer
        menuSelectionContainer.playerData.Add(new MenuSelectionContainer.PlayerInfo(input.PlayerAction, type));
    }

    /// <summary>
    /// When a device is detached, remove PlayerInfo from MenuSelectionContainer and
    /// despawn/destroy the player connected to it.
    /// </summary>
    private void OnDeviceDetach(InputDevice device){
        for(int i = menuSelectionContainer.playerData.Count - 1; i > -1; i--){
            MenuSelectionContainer.PlayerInfo data = menuSelectionContainer.playerData[i];
            if(data.playerAction.Device.GUID.Equals(device.GUID)){
                // Reset PlayerType array and MenuSelectionContainer
                isPlayerTypeUsed[i] = false;
                menuSelectionContainer.playerData.Remove(data);

                // Destroy the PlayerAction and the player
                GameObject player = currentPlayers[i];
                currentPlayers.Remove(player);
                player.GetComponent<RigidBodyInput>().PlayerAction.Destroy();
                Destroy(player);
            }
        }
    }

    /// <summary>
    /// Returns true if there are as many players as buttons activated.
    /// </summary>
    private bool IsTimeToStart(){
        int count = 0;
        foreach(PlayerButton b in playerButtons){
            if(b.IsColliding)
                count++;
        }
        return count == currentPlayers.Count && count > 1;
    }

    /// <summary>
    /// Returns true if there is only one player left and he enters back area.
    /// </summary>
    private bool IsTimeToGoBack(){
        int count = 0;
        foreach(PlayerButton b in backButtons){
            if(b.IsColliding)
                count++;
        }
        return count > 0 && currentPlayers.Count < 1;
    }

    private IEnumerator SwitchScene(string levelName){
        isSwitchingScene = true;
        yield return new WaitForSeconds(transitionTimeout);

         FishEyeTransition fishEye = new FishEyeTransition()
        {
            nextScene = levelName,
            duration = 0.2f,
            size = 0.0f,
            zoom = 10.0f,
            colorSeparation = 5.0f,
            fishEyeShader = transitionShader
        };

        TransitionKit.instance.transitionWithDelegate(fishEye);
    }

    void OnDisable(){
        // Destroy the defaultActions and unregister event
        LeanTween.reset();
        InputManager.OnDeviceDetached -= inputDevice => OnDeviceDetach(inputDevice);
        defaultGamepadAction.Destroy();
        defaultKeyboardAction.Destroy();
    }
}
