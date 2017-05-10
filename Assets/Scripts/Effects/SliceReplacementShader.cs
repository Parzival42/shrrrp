﻿using UnityEngine;

public class SliceReplacementShader : ReplacementShader
{
    [FancyHeader("Slice Shader", "Slice Replacement Shader")]
    [SerializeField]
    [Range(0f, 1f)]
    private float sliceThickness = 0.5f;

    [SerializeField]
    private Vector4 sliceDirection = new Vector4(1, 0, 1, 0);

    public float SliceThickness { get { return sliceThickness; } set { sliceThickness = value; } }
    public Vector4 SliceDirection { get { return sliceDirection; } set { sliceDirection = value; } }

    protected override void SetShaderParameters()
    {
        Shader.SetGlobalFloat("_SliceThickness", sliceThickness);
        Shader.SetGlobalVector("_SliceDirection", sliceDirection);
    }
}