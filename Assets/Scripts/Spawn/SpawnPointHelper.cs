using UnityEngine;

public class SpawnPointHelper
{
    private static string SPAWN_TAG = "Spawn";
    private static string RESPAWN_SPAWN_TAG = "AfterDeathSpawn";

    /// <summary>
    /// Spawns the given player prefab at the spawn point position which is searched via
    /// the PlayerType. The instantiated player is returned.
    /// If there was no spawn point with the player type found, null is returned.
    /// </summary>
    public static GameObject SpawnPlayer(GameObject playerPrefab, PlayerType playerType)
    {
        GameObject[] spawns = GameObject.FindGameObjectsWithTag(SPAWN_TAG);
        foreach (GameObject s in spawns)
        {
            SpawnPoint spawnPoint = s.GetComponent<SpawnPoint>();
            if (spawnPoint && spawnPoint.Player == playerType)
            {
                GameObject instantiatedPlayer = GameObject.Instantiate(playerPrefab, spawnPoint.transform.position, playerPrefab.transform.rotation);
                instantiatedPlayer.GetComponent<Player>().PlayerType = playerType;
                return instantiatedPlayer;
            }
        }
        return null;
    }

    /// <summary>
    /// Returns the respawn position of the given player gameobject with the
    /// specified PlayerType.
    /// </summary>
    /// <param name="player">Player gameobject which should be respawned.</param>
    /// <param name="playerType">Type of the player.</param>
    public static Vector3 GetRespawnPosition(PlayerType playerType)
    {
        GameObject[] spawns = GameObject.FindGameObjectsWithTag(RESPAWN_SPAWN_TAG);
        foreach (GameObject s in spawns)
        {
            SpawnPoint spawnPoint = s.GetComponent<SpawnPoint>();

            if (spawnPoint && spawnPoint.Player == playerType)
                return s.transform.position;
        }
        return Vector3.zero;
    }
}