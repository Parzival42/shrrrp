using System.Collections;
using UnityEngine;

public class MainMenu : BaseMenu {
	
	[SerializeField]
	public GameObject cuttingPlane;

	void Start () {
		// Find the MenuSelectionContainer and reset it OR initialize it if not found
		MenuSelectionContainer menuSelectionContainer = (MenuSelectionContainer)FindObjectOfType(typeof(MenuSelectionContainer));

		if(menuSelectionContainer != null){
			menuSelectionContainer.ResetContainer();
		} else {
			GameObject containerObject = new GameObject();
			containerObject.name = "MenuSelectionContainer";
			containerObject.AddComponent(typeof(MenuSelectionContainer));
		}
	}

	/// <summary>
	/// Exits the game.
	/// </summary>
	public override void LoadPreviousLevel(){
		Application.Quit();
	}

	/// <summary>
	/// Loads the next scene and cuts the given mesh.
	/// </summary>
	public override void LoadNextLevel(string levelName){
		//CuttingManagerLocator.GetInstance.Cut(cuttingPlane.transform, cuttingPlane.GetComponent<MeshFilter>().mesh);
		base.LoadNextLevel(levelName);
	}
	
}
