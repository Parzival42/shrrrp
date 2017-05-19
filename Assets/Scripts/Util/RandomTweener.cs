using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class RandomTweener : MonoBehaviour
{
    [SerializeField]
    private bool enablePositionTweenX = true;

    [SerializeField]
    private bool enablePositionTweenY = true;

    [SerializeField]
    private float posOffset = 10f;

    [SerializeField]
    private float posTime = 0.8f;

    [SerializeField]
    private bool enableScaleTween = true;

	[SerializeField]
    private float scaleFactor = 1.5f;

	 [SerializeField]
    private float scaleTime = 0.8f;

	[SerializeField]
    private float rotOffset = 0.2f;

	 [SerializeField]
    private float rotTime = 0.8f;

    [SerializeField]
    private LeanTweenType easeType = LeanTweenType.easeOutSine;

	private void Start ()
    {
        RectTransform rect = GetComponent<RectTransform>();

        if(enablePositionTweenX)
            LeanTween.moveX(rect, rect.anchoredPosition.x + posOffset, posTime).setLoopPingPong().setEase(easeType);

        if(enablePositionTweenY)
            LeanTween.moveY(rect, rect.anchoredPosition.y + posOffset, posTime * 0.8f).setLoopPingPong().setEase(easeType);

        if(enableScaleTween){
		    LeanTween.scaleX(gameObject, rect.localScale.x * scaleFactor, scaleTime).setLoopPingPong().setEase(easeType);
            LeanTween.scaleY(gameObject, rect.localScale.x * scaleFactor, scaleTime * 0.8f).setLoopPingPong().setEase(easeType);
		    LeanTween.scaleZ(gameObject, rect.localScale.z * scaleFactor, scaleTime * 0.6f).setLoopPingPong().setEase(easeType);
        }
        
		LeanTween.rotateX(gameObject, rect.localEulerAngles.x + rotOffset, rotTime).setLoopPingPong().setEase(easeType);
        LeanTween.rotateY(gameObject, rect.localEulerAngles.y + rotOffset, rotTime * 0.6f).setLoopPingPong().setEase(easeType);
    }
}
