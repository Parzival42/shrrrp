using System.Collections;
using UnityEngine;

public class MainMenu : BaseMenu {

	[SerializeField]
	private GameObject menuEnvironment;
	
	[SerializeField]
	public GameObject cuttingPlane;
	
	[SerializeField]
	[RangeAttribute(0.0f,4.0f)]
	private float sceneSwitchDelayTime = 0.4f;

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

		// Clouds should stay when loading next scene
		DontDestroyOnLoad(menuEnvironment);
	}

	/// <summary>
	/// Exits the game and ignores the parameter given.
	/// </summary>
	public override void LoadPreviousLevel(string levelName){
		Application.Quit();
	}

	/// <summary>
	/// Loads the next scene and cuts the given mesh.
	/// </summary>
	public override void LoadNextLevel(string levelName){
		CuttingManagerLocator.GetInstance.Cut(cuttingPlane.transform, cuttingPlane.GetComponent<MeshFilter>().mesh);
		StartCoroutine("DelayedLoadNextLevel", levelName);
	}

	/// <summary>
	/// Delays the scene switch by the specified amount of time
	/// </summary>
	private IEnumerator DelayedLoadNextLevel(string levelName){
		yield return new WaitForSeconds(sceneSwitchDelayTime);
		base.LoadNextLevel(levelName);
	}

	
}
