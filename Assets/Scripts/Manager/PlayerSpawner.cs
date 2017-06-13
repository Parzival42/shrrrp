using UnityEngine;

/// <summary>
/// Spawns the player based on the information of the MenuSelectionContainer.
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    [Comment("'Real' player spawner which uses the MenuSelectionContainer information to spawn the players. If no spawn info is found the PlayerSpawnerEditor is used.", 30f)]
    [FancyHeader("Settings", "Player Spawner settings")]
    [SerializeField]
    private bool debugLog = true;

    #region Internal variables
    MenuSelectionContainer menuSelectionContainer;
    #endregion

    private void Start ()
    {
        menuSelectionContainer = GetMenuSelectionContainer();

        if (menuSelectionContainer != null)
            InitiateSpawn();
        else
            Debug.Log("[PlayerSpawner]: No MenuSelectionContainer -> No regular player spawn.");
	}

    private void InitiateSpawn()
    {
        StartupEffects startupEffects = FindObjectOfType<StartupEffects>();
        if (startupEffects != null)
        {
            startupEffects.OnStartupFinished += SpawnPlayers;
        }
        else
        {
            Debug.Log("[PlayerSpawner]: No StartUpEffects found, spawn players immediately.", gameObject);
            SpawnPlayers();
        }
    }

    private void SpawnPlayers()
    {
        foreach (MenuSelectionContainer.PlayerInfo playerInfo in menuSelectionContainer.playerData)
        {
            GameObject spawnedPlayer = SpawnPointHelper.SpawnPlayer(menuSelectionContainer.playerPrefab, playerInfo.playerType);
            RigidBodyInput rigidInput = spawnedPlayer.GetComponent<RigidBodyInput>();

            // TODO: Somehow distinguish between keyboard and gamepad!
            rigidInput.PlayerAction = playerInfo.playerAction;
        }
    }

    private MenuSelectionContainer GetMenuSelectionContainer()
    {
        MenuSelectionContainer container = FindObjectOfType<MenuSelectionContainer>();
        return container;
    }
}