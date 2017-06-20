using UnityEngine;

public class PlayerDeathTrigger : TriggerAction
{
    #region Inspector variables
    [FancyHeader("Death Trigger Settings")]
    [SerializeField]
    private ParticleSystem deathParticles;

    [FancyHeader("Respawn settings")]
    [SerializeField]
    private float respawnWaitTime = 1.5f;

    [SerializeField]
    private GameObject respawnCylinder;
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
            if (player.KillPlayer())
            {
                playerManager.PlayerDied(player);
            }
            else
            {
                Vector3 respawnPosition = SpawnPointHelper.GetRespawnPosition(player.PlayerType);
                RigidBodyInput playerInput = player.GetComponent<RigidBodyInput>();

                PerformRespawnTween(playerInput, respawnPosition);
                // - Otherwise, decrement one life and respawn player with a fancy tween
                // - Spawn the player on a spawn platform with cool cylinder stuff (like in Super Smash Bros.)
            }
            PerformPlayerDeathEffects(player);
        }
        else
            Debug.LogError("No player script attached on player!", gameObject);

    }

    private void PerformRespawnTween(RigidBodyInput playerInput, Vector3 respawnPosition)
    {
        LeanTween.delayedCall(respawnWaitTime, () => {
            GameObject spawnCylinder = Instantiate(respawnCylinder, respawnPosition, respawnCylinder.transform.rotation);
            Vector3 spawnCylinderScale = spawnCylinder.transform.localScale;
            spawnCylinder.transform.localScale = Vector3.zero;

            LeanTween.scale(spawnCylinder, spawnCylinderScale, 0.4f).setEase(LeanTweenType.easeOutBack)
                .setOnComplete(() => {
                    playerInput.Rigid.velocity = Vector3.zero;
                    playerInput.transform.position = respawnPosition;
                    playerInput.StartSpawnTween();
                });
        });
    }

    private void PerformPlayerDeathEffects(Player player)
    {
        SpawnDeathParticles(player.transform.position);
        CameraUtil.CameraShake(cam, 2.4f, 0.11f, 0.9f);
    }

    private void SpawnDeathParticles(Vector3 position)
    {
        if (deathParticles != null)
            Instantiate(deathParticles, position, deathParticles.transform.rotation);
    }
}