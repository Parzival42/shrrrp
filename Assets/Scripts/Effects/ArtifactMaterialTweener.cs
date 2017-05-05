using UnityEngine;

public class ArtifactMaterialTweener : MonoBehaviour
{
    [FancyHeader("Amount parameter")]
    [SerializeField]
    private float amountMin = 2f;

    [SerializeField]
    private float amountMax = 5f;

    [SerializeField]
    private float amountDistanceThreshold = 2f;

    private static readonly string PARAM_AMOUNT = "_Amount";
    private PlayerManager playerManager;
    private Player[] players = null;
    private Material material;

	private void Start ()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        playerManager.OnAllPlayersFound += InitPlayers;
        material = GetComponent<Renderer>().material;
	}

    private void InitPlayers(Player[] players)
    {
        this.players = players;
        playerManager.OnAllPlayersFound -= InitPlayers;
    }

	private void Update ()
    {
        if (players != null)
        {
            float nearestDistance = GetNearestPlayerDistance();
            HandleAmountParameter(nearestDistance);
        }
	}

    private void HandleAmountParameter(float nearestDistance)
    {
        material.SetFloat(PARAM_AMOUNT, GetAmountFor(amountMin, amountMax, nearestDistance));
    }

    private float GetAmountFor(float min, float max, float currentValue)
    {
        float t = Mathf.InverseLerp(min, max, currentValue);
        return Mathf.Lerp(max, min, t);
    }

    private float GetNearestPlayerDistance()
    {
        float minDistance = float.MaxValue;
        foreach (Player p in players)
        {
            float distance = Vector3.Distance(p.transform.position, transform.position);
            if (distance < minDistance)
                minDistance = distance;
        }
        return minDistance;
    }
}