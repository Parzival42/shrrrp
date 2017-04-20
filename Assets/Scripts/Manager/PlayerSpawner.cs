using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
/// <summary>
/// Helper class for player spawning. Manages InControl controller 
/// assignment for development in order to start the scene normally with
/// controls.
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    [Header("Player/Slot assignment")]
    [SerializeField]
    private PlayerType[] playerTypes;

    [SerializeField]
    private ControllerSlot[] controllerSlots;

    [Header("Player prefabs")]
    [SerializeField]
    private GameObject[] playerPrefabs;

    private void Start ()
    {
        Initialize();
	}

    private void Initialize()
    {
        if (playerTypes == null || playerTypes.Length == 0)
            Debug.LogError("Player types not assigned!", gameObject);
        if (controllerSlots == null || controllerSlots.Length == 0)
            Debug.LogError("Player slots not assigned!", gameObject);
        if (playerTypes.Length != controllerSlots.Length)
            Debug.LogError("Count of player slots and player types must be the same!", gameObject);
        if (playerPrefabs == null || playerPrefabs.Length == 0)
            Debug.LogError("No player prefabs assigned :(.", gameObject);
    }
}
#endif