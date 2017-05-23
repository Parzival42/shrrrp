using UnityEngine;
using System.Collections;
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
    private bool enableRotationTween = true;

	[SerializeField]
    private float rotOffset = 0.2f;

	 [SerializeField]
    private float rotTime = 0.8f;

    [SerializeField]
    private LeanTweenType easeType = LeanTweenType.easeOutSine;

	private void Start ()
    {
        Transform transf = transform;
        RectTransform rect = GetComponent<RectTransform>();
        if(rect != null)
            transf = rect;

        if(enablePositionTweenX){
            if(rect != null)
                LeanTween.moveX(rect, rect.anchoredPosition.x + posOffset, posTime).setLoopPingPong().setEase(easeType);
            else
                LeanTween.moveX(gameObject, transform.position.x + posOffset, posTime).setLoopPingPong().setEase(easeType);
        }

        if(enablePositionTweenY){
            if(rect != null)
                LeanTween.moveY(rect, rect.anchoredPosition.y + posOffset, posTime * 0.8f).setLoopPingPong().setEase(easeType);
            else
                LeanTween.moveY(gameObject, transform.position.y + posOffset, posTime * 0.8f).setLoopPingPong().setEase(easeType);
        }


        if(enableScaleTween){
		    LeanTween.scaleX(gameObject, transf.localScale.x * scaleFactor, scaleTime).setLoopPingPong().setEase(easeType);
            LeanTween.scaleY(gameObject, transf.localScale.x * scaleFactor, scaleTime * 0.8f).setLoopPingPong().setEase(easeType);
		    LeanTween.scaleZ(gameObject, transf.localScale.z * scaleFactor, scaleTime * 0.6f).setLoopPingPong().setEase(easeType);
        }
        
        if(enableRotationTween){
		    LeanTween.rotateX(gameObject, transf.localEulerAngles.x + rotOffset, rotTime).setLoopPingPong().setEase(easeType);
            LeanTween.rotateY(gameObject, transf.localEulerAngles.y + rotOffset, rotTime * 0.6f).setLoopPingPong().setEase(easeType);
        }
    }
}
