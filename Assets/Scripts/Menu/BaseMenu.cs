﻿using UnityEngine;
using Prime31.TransitionKit;

public class BaseMenu : MonoBehaviour {

	
    [SerializeField]
    private Shader transitionShader;

	[SerializeField]
    private string previousLevel;

	[SerializeField]
	private GameObject menuEnvironment;
	
	[SerializeField]
	private GameObject menuMusic;

	void OnEnable(){
		// register Cancel event at Input module
		InputModuleActionAdapter.OnCancel += LoadPreviousLevel;

		// Clouds should stay when loading next scene but only instantiate once
		GameObject environment = GameObject.FindGameObjectWithTag("MenuEnvironment");
		if (environment == null)
		{
			environment = Instantiate(menuEnvironment, Vector3.zero, Quaternion.identity);
			DontDestroyOnLoad(environment);
		}

		// Same with music but this persists longer than the menu scripts
		GameObject music = GameObject.FindGameObjectWithTag("MenuMusic");
		if (music != null) return;
		music = Instantiate(menuMusic, Vector3.zero, Quaternion.identity);
		DontDestroyOnLoad(music);
	}

	/// <summary>
	/// Performs the switch to next scene.
	/// </summary>
	public virtual void LoadNextLevel(string levelName){
		PerformSceneSwitch(levelName);
	}

	/// <summary>
	/// Performs the switch to previous scene or exits the game.
	/// </summary>
	public virtual void LoadPreviousLevel(){
		PerformSceneSwitch(previousLevel);
	}

	/// <summary>
	/// Transitions between the scenes.
	/// </summary>
	public virtual void PerformSceneSwitch(string levelName)
    {
        FishEyeTransition fishEye = new FishEyeTransition()
        {
            nextScene = levelName,
            duration = 0.5f,
            size = 0.0f,
            zoom = 10.0f,
            colorSeparation = 5.0f,
            fishEyeShader = transitionShader
        };

        TransitionKit.instance.transitionWithDelegate(fishEye);
    }

	void OnDisable(){
		LeanTween.reset();
		InputModuleActionAdapter.OnCancel -= LoadPreviousLevel;
	}

}
