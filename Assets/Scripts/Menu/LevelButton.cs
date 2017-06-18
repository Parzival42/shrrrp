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
	
	[SerializeField]
	private AudioClip selectClip;
	
	[SerializeField]
	private float selectVolume = 1f;
	
	[SerializeField]
	private float selectPitch = 1f;
	
	[SerializeField]
	private AudioClip clickClip;
	
	[SerializeField]
	private float clickVolume = 1f;
	
	[SerializeField]
	private float clickPitch = 1f;
	
	[SerializeField]
	private bool isInitSelected = false;

	private Vector3 initScale;
	private GameObject sphere;
	private MenuSelectionContainer menuSelectionContainer;

	void Start(){
		menuSelectionContainer = (MenuSelectionContainer)FindObjectOfType(typeof(MenuSelectionContainer));
		sphere = GetComponentsInChildren<Transform>()[1].gameObject;
		initScale = sphere.transform.localScale;
	}

    public void OnSelect(BaseEventData eventData)
    {
		// Fill level name in MenuSelectionContainer
	    string mapName = name.Equals("RandomMap") ? GetComponent<MapRandomizer>().GetRandomMap() : name;
		menuSelectionContainer.levelName = mapName;
		
        LeanTween.scale(sphere, initScale * scaleFactor, tweenDuration).setEase(easeTypeSelect);
		
	    if(!isInitSelected)
	   		SoundManager.SoundManagerInstance.Play(selectClip, Vector3.zero, selectVolume, selectPitch, AudioGroup.Menu);
	    else
		    isInitSelected = false;
    }

	public void OnDeselect(BaseEventData data)
    {
        LeanTween.scale(sphere, initScale, tweenDuration).setEase(easeTypeDeselect);
    }

	public void Clicked()
	{
		DontDestroyOnLoad(SoundManager.SoundManagerInstance.Play(clickClip, Vector3.zero, clickVolume, clickPitch, AudioGroup.Menu));
	}
}