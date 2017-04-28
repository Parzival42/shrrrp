using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "PlayerMaterialObject", menuName = "Player/MaterialObject", order = 1)]
public class PlayerMaterialObject : ScriptableObject {
    public PlayerType playerType = PlayerType.Player1;
    public Material body;
	public Material hair;
	public Material scarf;
}
