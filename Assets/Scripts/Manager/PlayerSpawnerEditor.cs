using UnityEngine;
using InControl;

#if UNITY_EDITOR
/// <summary>
/// Helper class for player spawning. Manages InControl controller 
/// assignment for development in order to start the scene normally with
/// controls.
/// </summary>
public class PlayerSpawnerEditor : MonoBehaviour
{
    [Comment("The PlayerSpawner is only a script for development. It only spawns when you directly start playing in a level.", 15)]
    [FancyHeader("Assign Slots", "Player/Controller assignment")]
    [SerializeField]
    private PlayerType[] playerTypes;

    [SerializeField]
    private ControllerType[] controllerTypes;

    [FancyHeader("Player prefabs", "Assign different prefabs")]
    [SerializeField]
    private GameObject[] playerPrefabs;

    private void Start ()
    {
        StartupEffects startupEffects = FindObjectOfType<StartupEffects>();
        if (startupEffects != null)
            startupEffects.OnStartupFinished += PerformPlayerSpawn;
        else
        {
            Debug.Log("No <b>StartupEffects</b> script found -> Spawn player without delay", gameObject);
            PerformPlayerSpawn();
        }
	}

    private void PerformPlayerSpawn()
    {
        // Only run this script if the level is started withouth menu cycle
        if (IsLevelStartedDirectlyInEditor())
        {
            Debug.Log("Direct Editor level start! Spawn players via <b>PlayerSpawner</b>.", gameObject);
            Initialize();
            SpawnPlayersAtSpawnPoints();
        }
    }

    private void Initialize()
    {
        if (playerTypes == null || playerTypes.Length == 0)
            Debug.LogError("Player types not assigned!", gameObject);
        if (controllerTypes == null || controllerTypes.Length == 0)
            Debug.LogError("Player slots not assigned!", gameObject);
        if (playerTypes.Length != controllerTypes.Length)
            Debug.LogError("Count of player slots and player types must be the same!", gameObject);
        if (playerPrefabs == null || playerPrefabs.Length == 0)
            Debug.LogError("No player prefabs assigned :(.", gameObject);

        GetSelectedKeyboardCount();
    }

    private int GetSelectedKeyboardCount()
    {
        int selectedKeyboards = 0;
        for (int i = 0; i < controllerTypes.Length; i++)
        {
            if (controllerTypes[i] == ControllerType.Keyboard)
                selectedKeyboards++;
        }

        if (selectedKeyboards > 1)
            Debug.LogError("Only one keyboard is supported at the same time!", gameObject);

        return selectedKeyboards;
    }

    private void SpawnPlayersAtSpawnPoints()
    {
        int keyboardCount = GetSelectedKeyboardCount();
        int gamepadIndex = 0;

        // Loop through all devices (GamePads) and an eventual additional keyboard iteration (max. 1)
        for (int i = 0; i < InputManager.Devices.Count + keyboardCount; i++) {
            int playerType = (int) playerTypes[i];

            // Spawn player at specific spawn position
            GameObject spawnedPlayer = SpawnPointHelper.SpawnPlayer(playerPrefabs[playerType], playerTypes[i]);
            RigidBodyInput input = spawnedPlayer.GetComponent<RigidBodyInput>();

            // Assign different (Gamepad/Keyboard) player action
            if (controllerTypes[i] == ControllerType.Controller)
            {
                input.PlayerAction = StandardPlayerAction.CreateStandardGamePadBinding(InputManager.Devices[gamepadIndex]);
                gamepadIndex++;
            }
            else if (controllerTypes[i] == ControllerType.Keyboard)
                input.PlayerAction = StandardPlayerAction.CreateStandardKeyboardBinding(InputManager.ActiveDevice);

            // Stop, since there are not enough controllers for device assignment
            if ((i + 1) >= playerTypes.Length)
                break;
        }
    }

    /// <summary>
    /// If there is no 'MenuSelectionContainer' object in the scene,
    /// it is assumed that the level is started directly in the editor (without menu cycle).
    /// </summary>
    private bool IsLevelStartedDirectlyInEditor()
    {
        MenuSelectionContainer selectionContainer = FindObjectOfType<MenuSelectionContainer>();
        return selectionContainer == null;
    }
}
#endif