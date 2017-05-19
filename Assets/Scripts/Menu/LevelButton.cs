using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
	[SerializeField]
	private LeanTweenType easeTypeSelect;

	[SerializeField]
	private LeanTweenType easeTypeDeselect;

	[SerializeField]
	private float tweenDuration = 0.2f;

	[SerializeField]
	private float scaleFactor = 1.2f;

	private Vector3 initScale;
	private GameObject sphere;
	private GameObject plane;
	private MenuSelectionContainer menuSelectionContainer;
	private BaseMenu baseMenu;

	void Start(){
		menuSelectionContainer = (MenuSelectionContainer)FindObjectOfType(typeof(MenuSelectionContainer));
		baseMenu = (BaseMenu)FindObjectOfType(typeof(BaseMenu));
		sphere = GetComponentsInChildren<Transform>()[1].gameObject;
		plane =  GetComponentsInChildren<Transform>()[2].gameObject;
		initScale = sphere.transform.localScale;
	}

    public void OnSelect(BaseEventData eventData)
    {
		// Fill level name in MenuSelectionContainer
		menuSelectionContainer.levelName = gameObject.name;
		// Choose the cutting plane for the scene switch
		baseMenu.cuttingPlane = plane;
		
        LeanTween.scale(sphere, initScale * scaleFactor, tweenDuration).setEase(easeTypeSelect);
    }

	public void OnDeselect(BaseEventData data)
    {
        LeanTween.scale(sphere, initScale, tweenDuration).setEase(easeTypeDeselect);
    }
}