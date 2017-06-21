using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.PostProcessing.Utilities;
using UnityEngine.SceneManagement;

public class CameraAnimation : MonoBehaviour {

	private GameManager gameManager;

	private Animator playerAnimator;
	
	private Vector3 desiredCameraPosition;
	
	private Transform targetTransform;

	private PostProcessingController postProcessingController;

	[SerializeField] private Vector3 cameraOffset;
	
	[SerializeField] private LeanTweenType translationEaseType;

	[SerializeField] private LeanTweenType fovEaseType;


	[SerializeField] private float timeScaleDuration;


	[SerializeField] private float translationDuration;

	[SerializeField] private float minTargetDistance;

	[SerializeField] private float xRotation;

	[SerializeField] private float zoomAperture;
 
	void Start ()
	{

		gameManager = FindObjectOfType<GameManager>();
		postProcessingController = GetComponent<PostProcessingController>();

		if (!gameManager || !postProcessingController)
		{
			Debug.LogError("Game Manager or PostProcessingBehaviour is null!");
		}

		gameManager.OnGameEnded += OnGameEnd;
		
		OnGameEnd();
		
	}
	
	void OnGameEnd()
	{
		StartCoroutine(WaitForAnimationStart());
	}

	private IEnumerator WaitForAnimationStart()
	{
		yield return new WaitForSeconds(1.5f);
		
		GameObject winner = FindObjectOfType<Player>().gameObject;
		
		if (winner != null)
		{
			PrepareForAnimation(winner);
			StartAnimation();
			DestroyTheWholeUniverseTwice();
		}
	}

	private void DestroyTheWholeUniverseTwice()
	{
		List<GameObject> rootObjects = new List<GameObject>();
		Scene scene = SceneManager.GetActiveScene();
		scene.GetRootGameObjects( rootObjects );
  
		for (int i = 0; i < rootObjects.Count; ++i)
		{
			GameObject gameObject = rootObjects[ i ];
			if(gameObject.hideFlags == HideFlags.None && gameObject.layer ==LayerMask.NameToLayer("Ground"))
			{
				gameObject.AddComponent<DissolveObject>();	
			}
		}
	}

	private void PrepareForAnimation(GameObject winner)
	{
		playerAnimator = winner.GetComponent<Animator>();
		
		targetTransform = winner.transform;
		desiredCameraPosition = targetTransform.position - new Vector3(0, 0, minTargetDistance) + cameraOffset;

		Rigidbody rigidbody = winner.GetComponent<Rigidbody>();
		rigidbody.useGravity = false;

		RigidBodyInput input = winner.GetComponent<RigidBodyInput>();
		input.AllowMovement = false;
		input.enabled = false;

		LeanTween.reset();
		LeanTween.rotateY(winner, -90, translationDuration).setUseEstimatedTime(true);

		ParticleSystem particleSystem = winner.GetComponentInChildren<ParticleSystem>();
	
		particleSystem.Emit(1);
	}

	private void StartAnimation()
	{
		
		LeanTween.move(gameObject, desiredCameraPosition, translationDuration).setEase(translationEaseType).setUseEstimatedTime(true);
		LeanTween.rotateX(gameObject, xRotation, translationDuration).setEase(translationEaseType).setUseEstimatedTime(true);
		LeanTween.value(gameObject, Time.timeScale, 0.0f, timeScaleDuration).setEase(LeanTweenType.linear).setOnUpdate(
			(float value) =>
			{
				Time.timeScale = value;
			}).setOnComplete(() => { Time.timeScale = 0.0f; });
		
		playerAnimator.Play("EndCut");


		LeanTween.value(gameObject, postProcessingController.depthOfField.aperture, zoomAperture, translationDuration)
			.setOnUpdate(
				(float value) =>
				{
					postProcessingController.depthOfField.aperture = value;
				}).setUseEstimatedTime(true);
		
		
		FocusPuller focusPuller = gameObject.AddComponent<FocusPuller>();
		focusPuller.target = targetTransform;


		MeshInstancer meshInstancer = FindObjectOfType<MeshInstancer>();
		meshInstancer.Translation = Vector2.zero;
		
		ColorOverlay overlay = GetComponent<ColorOverlay>();
		overlay.enabled = true;
		LeanTween.value(gameObject, 0f, 0.9f, 0.24f)
			.setEase(LeanTweenType.easeInOutCirc)
			.setDelay(translationDuration)
			.setOnUpdate((float value) => {
				overlay.OverlayStrength = value;
			})
			.setLoopClamp()
			.setLoopPingPong(1)
			.setUseEstimatedTime(true)
			.setOnComplete(() => {
				overlay.enabled = false;
			});

	}
	
		
	

}
