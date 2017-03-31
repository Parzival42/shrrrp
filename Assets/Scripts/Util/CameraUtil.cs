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
}