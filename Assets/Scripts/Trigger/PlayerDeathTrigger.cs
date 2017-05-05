using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathTrigger : TriggerAction
{
    #region Inspector variables
    [FancyHeader("Death Trigger Settings")]
    [SerializeField]
    private ParticleSystem deathParticles;
    #endregion

    #region Internal variables
    private PlayerManager playerManager;
    private Camera cam;
    #endregion

    protected override void Start()
    {
        base.Start();
        cam = CameraUtil.GetMainCamera();
        CheckForPlayerManager();
    }

    private void CheckForPlayerManager()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        if (playerManager == null)
            Debug.LogError("No PlayerManager in scene!", gameObject);
    }

    protected override void PerformOnTriggerAction(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            playerManager.PlayerDied(player);
            SpawnDeathParticles(player.transform.position);
            CameraUtil.CameraShake(cam, 2.4f, 0.11f, 0.9f);
        }
        else
            Debug.LogError("No player script attached on player!", gameObject);

    }

    private void SpawnDeathParticles(Vector3 position)
    {
        if (deathParticles != null)
            Instantiate(deathParticles, position, deathParticles.transform.rotation);
    }
}
