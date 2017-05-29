using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerButton : MonoBehaviour
{
    [FancyHeader("Tween options", "Button tween options")]
    [SerializeField]
    private float yOffset = 0.2f;

    [SerializeField]
    private float tweenTime = 0.3f;

    [SerializeField]
    private LeanTweenType easeType = LeanTweenType.easeOutExpo;

    [FancyHeader("Light")]
    [SerializeField]
    private Light infoLight;

    [SerializeField]
    private float lightIntensity = 1.36f;

    [FancyHeader("Debug Information")]
    [SerializeField]
    private bool isColliding = false;

    public bool IsColliding { get { return isColliding; } }

    private float originalHeight;

	private void Start () {
        originalHeight = transform.position.y;
	}

    private void OnTriggerStay(Collider other)
    {
        isColliding = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isColliding = false;
        LeanTween.moveY(gameObject, originalHeight, tweenTime).setEase(easeType);
        LeanTween.value(lightIntensity, 0f, tweenTime).setEase(easeType)
            .setOnUpdate((float value) => {
                infoLight.intensity = value;
            });
    }

    private void OnTriggerEnter(Collider other)
    {
        LeanTween.moveY(gameObject, originalHeight - yOffset, tweenTime).setEase(easeType);
        LeanTween.value(0f, lightIntensity, tweenTime).setEase(easeType)
            .setOnUpdate((float value) => {
                infoLight.intensity = value;
            });
    }
}