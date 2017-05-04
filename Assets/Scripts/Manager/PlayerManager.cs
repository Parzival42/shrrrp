using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void AllPlayerDeadHandler();
public delegate void PlayerDiedHandler(Player player);

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
    public event PlayerDiedHandler OnPlayerDied;
    #endregion

    #region Properties
    public int SessionPlayerCount { get { return sessionPlayerCount; } }
    public int CurrentPlayerCount { get { return currentPlayerCount; } }
    #endregion

    private void Start ()
    {
        OnPlayerDied += HandlePlayerDeath;
        FindAllPlayers();
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

        Debug.Log("[PlayerManager]: " + sessionPlayerCount + " spawned!");
    }

    private void HandlePlayerDeath(Player player)
    {
        currentPlayerCount--;
        if (CurrentPlayerCount <= 0)
            AllPlayersDied();
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
    #endregion
}