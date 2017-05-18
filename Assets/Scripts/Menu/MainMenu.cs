using System.Collections;
using UnityEngine;

public class MainMenu : BaseMenu {

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
	/// Exits the game and ignores the parameter given.
	/// </summary>
	public override void LoadPreviousLevel(string levelName){
		Application.Quit();
	}
	
}
