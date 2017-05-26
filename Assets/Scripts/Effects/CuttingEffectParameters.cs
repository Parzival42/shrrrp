using UnityEngine;

[CreateAssetMenu(fileName = "CuttingEffectParameters", menuName = "Effect/CutEffectParam", order = 2)]
public class CuttingEffectParameters : ScriptableObject
{
    [Comment("General effect parameter container for the cutting event itself.")]

    [FancyHeader("Timescale", "Timescale effect parameters")]
    public float timeDestination = 0.1f;
    public float timeEffectDuration = 0.2f;
    public LeanTweenType timeEase = LeanTweenType.easeInOutCubic;

    [FancyHeader("FOV", "FOV effect parameters")]
    public float fovAdd = 15f;
    public float fovTime = 0.3f;
    public LeanTweenType fovEase = LeanTweenType.easeInOutCubic;

    [FancyHeader("Player", "Player tween parameters")]
    public float playerHeightOffset = 3.5f;
    public float playerTime = 0.36f;
    public LeanTweenType playerEase = LeanTweenType.easeOutExpo;
}