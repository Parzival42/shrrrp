using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUtil
{
    public static string CAMERA_TAG = "MainCamera";
    public static string CAMERA_NAME = "Main Camera";

    /// <summary>
    /// Tries to fetch the current main camera.
    /// </summary>
    /// <returns>Returns null if no camera was found.</returns>
    public static Camera GetMainCamera()
    {
        Camera cam = null;

        cam = GameObject.FindGameObjectWithTag(CAMERA_TAG).GetComponent<Camera>();

        if (cam == null)
            cam = GameObject.Find(CAMERA_TAG).GetComponent<Camera>();
        if(cam == null)
            cam = GameObject.Find(CAMERA_NAME).GetComponent<Camera>();

        if (cam == null)
            Debug.LogError("No main camera was found in the scene.");

        return cam;
    }

    /// <summary>
    /// Produces a camera shake with the given camera.
    /// </summary>
    /// <param name="cam">Camera</param>
    /// <param name="shakeAmount">The shake amount in degrees</param>
    /// <param name="shakePeriodTime">Period of each shake.</param>
    /// <param name="dropOffTime">Time for the shake to ease out to nothing.</param>
    public static void CameraShake(Camera cam, float shakeAmount, float shakePeriodTime, float dropOffTime)
    {
        LTDescr shakeTween = LeanTween.rotateAroundLocal(cam.gameObject, Vector3.up, shakeAmount, shakePeriodTime)
            .setEase(LeanTweenType.easeShake)
            .setLoopClamp()
            .setRepeat(-1);

        // Slow the camera shake down to zero
        LeanTween.value(cam.gameObject, shakeAmount, 0f, dropOffTime)
            .setOnUpdate((float value) => {
                shakeTween.setTo(Vector3.right * value);
            })
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() => {
                LeanTween.cancel(shakeTween.uniqueId);
            });
    }
}