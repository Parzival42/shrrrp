using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	#region Internal Members
	PlayerType playerType;
	#endregion

	#region Properties
    public PlayerType PlayerType
    {
		get { return playerType; }
		set { playerType = value; }
	}

    public int PlayerLives { get { return playerLives; } set { playerLives = value; } }
    #endregion

    #region Public Fields
    [FancyHeader("Player settings")]
    [SerializeField]
    private int playerLives = 3;

	[SerializeField]
 	[FancyHeader("Material Objects", "Assign materials based on PlayerType")]
	private PlayerMaterialObject[] playerMaterials;

	[SerializeField]
	private Renderer body;

	[SerializeField]
	private Renderer hair;

	[SerializeField]
	private Renderer[] scarf;
	#endregion

    private void Start ()
    {
        SetPlayerName();
        ChoosePlayerMaterial();
	}

    private void SetPlayerName()
    {
        gameObject.name = PlayerType.ToString();
    }

    private void ChoosePlayerMaterial()
    {
		foreach(PlayerMaterialObject materials in playerMaterials){
			if(playerType.Equals(materials.playerType)){
				body.material = materials.body;
				hair.material = materials.hair;
				foreach(Renderer r in scarf)
					r.material = materials.scarf;
			}
		}
    }
}
