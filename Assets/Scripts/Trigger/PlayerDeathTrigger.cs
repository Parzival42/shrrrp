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
    private GameManager gameManager;
    private Camera cam;
    private bool gameEnded = false;
    #endregion

    protected override void Start()
    {
        base.Start();
        cam = CameraUtil.GetMainCamera();
        CheckForPlayerManager();

        gameManager = FindObjectOfType<GameManager>();
        gameManager.OnGameEnded += () => {
            gameEnded = true;
        };
    }

    private void CheckForPlayerManager()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        if (playerManager == null)
            Debug.LogError("No PlayerManager in scene!", gameObject);
    }

    protected override void PerformOnTriggerAction(Collider other)
    {
        if (!gameEnded)
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

                    IngameSoundManager soundManager = FindObjectOfType<IngameSoundManager>();
                    if (soundManager != null)
                        soundManager.HandlePlayerRespawn();
                }
                PerformPlayerDeathEffects(player);
            }
            else
                Debug.LogError("No player script attached on player!", gameObject);
        }
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