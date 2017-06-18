using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameSoundManager : MonoBehaviour {

    [FancyHeader("Player death sound")]
    [SerializeField]
    private AudioClip playerDeath;
    
    [SerializeField]
    private float playerDeathVolume = 1;
    
    [SerializeField]
    private float playerDeathPitch = 1;

	private PlayerManager playerManager;

	private void Start()
	{
		InitializePlayerManager();
	}

	private void InitializePlayerManager()
	{
		playerManager = FindObjectOfType<PlayerManager>();

		if (playerManager == null)
			Debug.LogError("[UIManager]: No PlayerManager found!", gameObject);
		else
		{
			playerManager.OnAllPlayersDied += HandleAllPlayerDead;
			playerManager.OnOnePlayerLeft += HandleOnePlayerLeft;
			playerManager.OnPlayerDied += HandlePlayerDied;
			playerManager.OnAllPlayersFound += HandleAllPlayersFound;
		}
	}

	private void HandleAllPlayersFound(Player[] players)
	{
	}

	private void HandleOnePlayerLeft(Player winner)
	{
	}

	private void HandlePlayerDied(Player deadPlayer)
	{
		SoundManager.SoundManagerInstance.Play(playerDeath, Vector3.zero, playerDeathVolume, playerDeathPitch,
			AudioGroup.Character);
	}

	private void HandleAllPlayerDead()
	{
	}
}
