using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartupEffects : MonoBehaviour
{
    #region Inspector
    [FancyHeader("Slice effect", "Settings for intro slicing")]
    [SerializeField]
    private float sliceTime = 0.4f;

    [SerializeField]
    private LeanTweenType sliceEase = LeanTweenType.easeInCirc;

    [FancyHeader("FOV effect")]
    [SerializeField]
    private float fovAdd = 20;

    [SerializeField]
    private float fovTime = 0.7f;

    [SerializeField]
    private LeanTweenType fovEase = LeanTweenType.easeInCirc;

    [FancyHeader("Color overlay")]
    [SerializeField]
    private float overlayTime = 0.5f;

    [SerializeField]
    private LeanTweenType overlayEase = LeanTweenType.easeInOutCirc;

    [SerializeField]
    [Range(0f, 1f)]
    private float overlayMax = 0.8f;
    #endregion

    #region Internal members
    private Camera cam;
    #endregion

    private void Start()
    {
        cam = CameraUtil.GetMainCamera();
        StartIntroSliceEffect();
        StartFovEffect();
        StartColorOverlay();
    }

    private void StartIntroSliceEffect()
    {
        SliceReplacementShader slicer = cam.GetComponent<SliceReplacementShader>();
        slicer.ReplaceShader();
        
        LeanTween.value(gameObject, 1f, 0f, sliceTime).setEase(sliceEase)
            .setOnUpdate((float value) => {
                slicer.SliceThickness = value;
            })
            .setOnComplete(() => {
                slicer.DisableShader();
                slicer.enabled = false;
            });
    }

    private void StartFovEffect()
    {
        LeanTween.value(gameObject, cam.fieldOfView, cam.fieldOfView + fovAdd, fovTime).setEase(fovEase)
            .setOnUpdate((float value) => {
                cam.fieldOfView = value;
            }).setLoopClamp().setLoopPingPong(1).setDelay(sliceTime * 0.5f)
            .setOnComplete(() => {
                LeanTween.reset();
            });
    }

    private void StartColorOverlay()
    {
        ColorOverlay overlay = cam.GetComponent<ColorOverlay>();
        LeanTween.value(gameObject, 0f, overlayMax, overlayTime)
            .setEase(overlayEase)
            .setDelay(sliceTime * 0.7f)
            .setOnUpdate((float value) => {
                overlay.OverlayStrength = value;
            })
            .setLoopClamp()
            .setLoopPingPong(1)
            .setOnComplete(() => {
                overlay.enabled = false;
            });
    }
}