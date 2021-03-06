﻿using Prime31.TransitionKit;
using System.Collections;
using UnityEngine;
using InControl;

public class LevelEndManager : MonoBehaviour
{
    private static readonly string BASE_MENU_NAME = "MainMenu";
    [Comment("Handles routines which should be called at the end of the game (scene change, etc, ...).")]

    #region Inspector variables
    [FancyHeader("Settings")]
    [SerializeField]
    private float sceneChangeDelay = 3f;

    [SerializeField]
    private Shader transitionShader;

    [SerializeField]
    [Tooltip("After this time the players can skip with any button.")]
    private float allowSkipTime = 3f;
    #endregion

    #region Internal variables
    private PlayerManager playerManager;
    private bool alreadyCalled = false;
    private float currentSkipTime = 0f;
    private bool skipped = false;

    private bool gameEnded = false;
    #endregion

    private void Start ()
    {
        InitializePlayerManager();
	}

    private void Update()
    {
        if (gameEnded)
            HandleSkipInput();
    }

    private void HandleSkipInput()
    {
        if (currentSkipTime > allowSkipTime && !skipped && Input.anyKeyDown)
        {
            skipped = true;
            PerformSceneChange();
        }
        currentSkipTime += Time.deltaTime;
    }

    private void InitializePlayerManager()
    {
        playerManager = FindObjectOfType<PlayerManager>();

        if (playerManager == null)
            Debug.LogError("[UIManager]: No PlayerManager found!", gameObject);
        else
        {
            playerManager.OnAllPlayersDied += HandleLevelEnd;
            playerManager.OnOnePlayerLeft += (Player player) => { HandleLevelEnd(); };
        }
    }

    private void HandleLevelEnd()
    {
        gameEnded = true;
        if (!alreadyCalled)
        {
            AudioSource musicAudioSource = CameraUtil.GetMainCamera().gameObject.GetComponent<AudioSource>();
            if (musicAudioSource != null)
            {
                LeanTween.value(musicAudioSource.volume, 0f, sceneChangeDelay)
                    .setOnUpdate((float value) =>
                    {
                        musicAudioSource.volume = value;
                    });
            }
            StartCoroutine(StartNextLevel());
            alreadyCalled = true;
        }
    }

    IEnumerator StartNextLevel()
    {
        yield return new WaitForSeconds(sceneChangeDelay);
        PerformSceneChange();
    }

    private void PerformSceneChange()
    {
        FishEyeTransition fishEye = new FishEyeTransition()
        {
            nextScene = BASE_MENU_NAME,
            duration = 0.2f,
            size = 0.0f,
            zoom = 10.0f,
            colorSeparation = 5.0f,
            fishEyeShader = transitionShader
        };

        LeanTween.reset();
        TransitionKit.instance.transitionWithDelegate(fishEye);
    }
}