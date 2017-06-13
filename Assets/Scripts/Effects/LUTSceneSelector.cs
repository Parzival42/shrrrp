using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing.Utilities;
using UnityEngine.PostProcessing;

[RequireComponent(typeof(Camera), typeof(PostProcessingController))]
public class LUTSceneSelector : MonoBehaviour
{
    [FancyHeader("Scene LUT", "Scene specific LUT")]
    [SerializeField]
    private Texture2D sceneLUT;

    private PostProcessingController controller;

	private void Start ()
    {
        controller = GetComponent<PostProcessingController>();
        ChangeToSceneLUT();
	}

    private void ChangeToSceneLUT()
    {
        controller.controlUserLut = true;
        controller.enableUserLut = true;
        controller.userLut.lut = sceneLUT;
    }
}