using System.Collections.Generic;
using UnityEngine;

public delegate void AllPlayerDeadHandler();
public delegate void OnePlayerLeftHandler(Player lastPlayer);
public delegate void PlayerDiedHandler(Player player);
public delegate void AllPlayersFoundHandler(Player[] players);

/// <summary>
/// Keeps track of the players (Alive, Dead, How Many, ...).
/// This script should be started after all players are spawned!
/// </summary>
public class PlayerManager : MonoBehaviour
{
    #region Inspector variables
    [Comment("Keeps track of players (Alive, Dead, ...). This script should be started after all players are spawned!")]
    [Tooltip("Specifies if player events should be sent.")]
    [SerializeField]
    private bool sendEvents = true;
    #endregion

    #region Internal variables
    private static string PLAYER_TAG = "Player";
    private readonly Dictionary<PlayerType, Player> playersByType = new Dictionary<PlayerType, Player>();
    private int sessionPlayerCount = -1;
    private int currentPlayerCount = -1;
    #endregion

    #region Events
    public event AllPlayerDeadHandler OnAllPlayersDied;
    public event OnePlayerLeftHandler OnOnePlayerLeft;
    public event PlayerDiedHandler OnPlayerDied;
    public event AllPlayersFoundHandler OnAllPlayersFound;
    #endregion

    #region Properties
    public int SessionPlayerCount { get { return sessionPlayerCount; } }
    public int CurrentPlayerCount { get { return currentPlayerCount; } }
    #endregion

    private void Start ()
    {
        OnPlayerDied += HandlePlayerDeath;

        StartupEffects startupEffects = FindObjectOfType<StartupEffects>();
        if (startupEffects != null)
            startupEffects.OnStartupFinished += FindAllPlayers;
        else
        {
            Debug.Log("No <b>StartupEffects</b> script found.", gameObject);
            FindAllPlayers();
        }
	}

    /// <summary>
    /// Searches for all players and saves them once into the player dictionary.
    /// </summary>
    private void FindAllPlayers()
    {
        GameObject[] playerGameObjects = GameObject.FindGameObjectsWithTag(PLAYER_TAG);

        foreach (GameObject p in playerGameObjects)
        {
            Player player = p.GetComponent<Player>();
            if (player != null)
            {
                if (playersByType.ContainsKey(player.PlayerType))
                    Debug.LogError("[PlayerManager]: 2 same players are spawned!", gameObject);
                else
                    playersByType.Add(player.PlayerType, player);
            }
            else
                Debug.LogError("[PlayerManager]: GameObject with player tag was found which has no Player-Script!", gameObject);
        }

        // Assign initial player count
        sessionPlayerCount = playersByType.Count;
        currentPlayerCount = sessionPlayerCount;

        Player[] players = new Player[playersByType.Count];
        playersByType.Values.CopyTo(players, 0);
        AllPlayersFound(players);

        Debug.Log("[PlayerManager]: " + sessionPlayerCount + " spawned!");
    }

    private void HandlePlayerDeath(Player player)
    {
        currentPlayerCount--;
        DeactivatePlayer(player);

        if (CurrentPlayerCount == 1)
        {
            Player lastPlayer = GetLastPlayer();
            Debug.Log("[PlayerManager]: Player " + lastPlayer.PlayerType.ToString() + " won!", gameObject);
            OnePlayerLeft(lastPlayer);
        }

        if (CurrentPlayerCount <= 0)
        {
            Debug.Log("[PlayerManager]: All players died!", gameObject);
            AllPlayersDied();
        }
    }

    private Player GetLastPlayer()
    {
        foreach (KeyValuePair<PlayerType, Player> player in playersByType)
        {
            if (player.Value.gameObject.activeSelf)
                return player.Value;
        }
        return null;
    }

    private void DeactivatePlayer(Player player)
    {
        Debug.Log("[PlayerManager]: Player " + player.PlayerType.ToString() + " died.", gameObject);
        player.gameObject.SetActive(false);
    }

    #region Event methods
    /// <summary>
    /// Event method which should be called from outside when a player dies.
    /// </summary>
    public void PlayerDied(Player player)
    {
        if (OnPlayerDied != null && sendEvents)
            OnPlayerDied(player);
    }

    private void AllPlayersDied()
    {
        if (OnAllPlayersDied != null && sendEvents)
            OnAllPlayersDied();
    }

    private void OnePlayerLeft(Player lastPlayer)
    {
        if (OnOnePlayerLeft != null)
            OnOnePlayerLeft(lastPlayer);
    }

    private void AllPlayersFound(Player[] players)
    {
        if (OnAllPlayersFound != null)
            OnAllPlayersFound(players);
    }
    #endregion
}