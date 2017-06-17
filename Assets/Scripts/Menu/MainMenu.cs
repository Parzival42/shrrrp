using System.Collections;
using UnityEngine;

public class MainMenu : BaseMenu {
	
	[SerializeField]
	public GameObject cuttingPlane;
	
	[SerializeField]
	[RangeAttribute(0.0f,4.0f)]
	private float sceneSwitchDelayTime = 0.4f;

    [SerializeField]
    private GameObject pronounciationThingy;

    [SerializeField]
    private float pronounciationThingyTweenTime = 0.4f;

	void Start () {
		// Find the MenuSelectionContainer and reset it OR initialize it if not found
		MenuSelectionContainer menuSelectionContainer = (MenuSelectionContainer)FindObjectOfType(typeof(MenuSelectionContainer));

		if(menuSelectionContainer != null){
			menuSelectionContainer.ResetContainer();
		} else {
			GameObject containerObject = new GameObject {name = "MenuSelectionContainer"};
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
	public override void LoadNextLevel(string levelName) {
		CuttingManagerLocator.GetInstance.Cut(cuttingPlane.transform, cuttingPlane.GetComponent<MeshFilter>().mesh);
        if (pronounciationThingy != null) {
            LeanTween.moveY(pronounciationThingy, pronounciationThingy.transform.position.y - 35f, pronounciationThingyTweenTime)
                .setEase(LeanTweenType.easeInBack);
        }
		StartCoroutine(DelayedLoadNextLevel(levelName));
	}

	/// <summary>
	/// Delays the scene switch by the specified amount of time
	/// </summary>
	private IEnumerator DelayedLoadNextLevel(string levelName){
		yield return new WaitForSeconds(sceneSwitchDelayTime);
		base.LoadNextLevel(levelName);
	}

	
}
