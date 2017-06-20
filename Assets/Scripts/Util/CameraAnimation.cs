using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimation : MonoBehaviour {

	GameObject cube;
	public Transform center;
	public Vector3 axis = Vector3.up;
	public Vector3 desiredPosition;
	public float radius = 2.0f;
	public float radiusSpeed = 0.5f;
	public float rotationSpeed = 80.0f;
	public PlayerManager playerManager;
	public GameManager gameManager;

	private Animator playerAnimator;

	public bool use = false;

	[SerializeField] private CuttingEffectParameters cuttingEffectParameters;

	[SerializeField] private LeanTweenType easeType;
 
	void Start ()
	{

		playerManager = FindObjectOfType<PlayerManager>();
		gameManager = FindObjectOfType<GameManager>();

		if (!playerManager || !gameManager)
		{
			Debug.LogError("Player or Game Manager is null!");
		}

		gameManager.OnGameEnded += OnGameEnd;
		center = Camera.main.transform;
		transform.position = (transform.position - center.position).normalized * radius + center.position;
		radius = 10.0f;
		
		OnGameEnd();
		
	}
	
	public AnimationClip GetAnimationClip(string name) {
		if (!playerAnimator) return null; // no animator
 
		foreach (AnimationClip clip in playerAnimator.runtimeAnimatorController.animationClips) {
			if (clip.name == name) {
				return clip;
			}
		}
		return null; // no clip by that name
	}
     
	void Update () {
		if (use)
		{
			transform.RotateAround (center.position, axis, rotationSpeed * Time.unscaledDeltaTime);
			desiredPosition = (transform.position - center.position).normalized * radius + center.position;
			transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.unscaledDeltaTime * radiusSpeed);
		}
		
	}

	void OnGameEnd()
	{
		StartCoroutine(WaitForAnimationStart());
	}

	private IEnumerator WaitForAnimationStart()
	{
		yield return new WaitForSeconds(1.5f);
		
		Debug.Log("camera animation started");
		GameObject winner = FindObjectOfType<Player>().gameObject;
		
		if (winner != null)
		{
			
			PlayCameraAnimation();
			
			/*RigidBodyInput input = winner.GetComponent<RigidBodyInput>();
			RigidBodyInputHandler inputHandler = (RigidBodyInputHandler) input.InputController;
			inputHandler.PerformCutJump();*/
			
			playerAnimator = winner.GetComponent<Animator>();
			if (playerAnimator)
			{
				playerAnimator.SetTrigger("Cut");
				playerAnimator.speed = 1.0f;
			
			}
			PlayCutAnimation();
			//center = Camera.main.transform;
			center = winner.transform;
			transform.position = (transform.position - center.position).normalized * radius + center.position;
			radius = 5.0f;
			use = true;
			
			TimeFreeze();
		}
	}
	
	private void TimeFreeze()
	{
		float originalTime = Time.timeScale;
		LeanTween.value(originalTime, 0.0f, 0.5f).setEase(easeType)
			.setOnUpdate((float value) =>
			{
				Time.timeScale = value;

			}).setUseEstimatedTime(true).setOnComplete(()=> { playerAnimator.StopPlayback();});
	}

	private void PlayCutAnimation()
	{
		
	}

	private void PlayCameraAnimation()
	{
		//throw new System.NotImplementedException();
	}
}
