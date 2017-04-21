using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Choose player type!")]
    [SerializeField]
    private PlayerType player = PlayerType.Player1;
    public PlayerType Player { get { return player; } }

	private void Start ()
    {
        CheckOtherPlayerTypes();
	}

    /// <summary>
    /// Checks if other spawn points have the same player type.
    /// This should not happen.
    /// </summary>
    private void CheckOtherPlayerTypes()
    {
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();

        foreach (SpawnPoint p in spawnPoints)
        {
            // Ignore self
            if (!p.gameObject.Equals(gameObject))
            {
                if (Player == p.Player)
                    Debug.LogError("Spawnpoints with same player type detected!", gameObject);
            }
        }
    }
}