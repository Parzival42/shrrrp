using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Prime31.TransitionKit;

public class BaseMenu : MonoBehaviour {

	
    [SerializeField]
    private Shader transitionShader;

	/// <summary>
	/// Performs the switch to next scene.
	/// </summary>
	public virtual void LoadNextLevel(string levelName){
		PerformSceneSwitch(levelName);
	}

	/// <summary>
	/// Performs the switch to previous scene or exits the game.
	/// </summary>
	public virtual void LoadPreviousLevel(string levelName){
		PerformSceneSwitch(levelName);
	}

	/// <summary>
	/// Transitions between the scenes.
	/// </summary>
	public virtual void PerformSceneSwitch(string levelName)
    {
        FishEyeTransition fishEye = new FishEyeTransition()
        {
            nextScene = levelName,
            duration = 0.2f,
            size = 0.0f,
            zoom = 10.0f,
            colorSeparation = 5.0f,
            fishEyeShader = transitionShader
        };

        TransitionKit.instance.transitionWithDelegate(fishEye);
    }

}
