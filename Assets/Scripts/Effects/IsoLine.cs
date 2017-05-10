using UnityEngine;

[RequireComponent(typeof(Camera))]
public class IsoLine : RaycastCornerImageEffect
{
    [FancyHeader("Isoline", "Isoline image effect parameters")]
    [SerializeField]
    private Color lineColor = Color.white;

    [SerializeField]
    private float colorExponent = 1f;

    [SerializeField]
    [Range(0f, 1f)]
    private float lineThickness = 0.5f;

    [SerializeField]
    private float lineCount = 5f;

    [SerializeField]
    private Vector4 lineDirection = Vector3.zero;

    [SerializeField]
    private float fadeOutDistance = 2f;

    public Vector4 LineDirection { get { return lineDirection; } set { lineDirection = value; } }
    public float LineCount { get { return lineCount; } set { lineCount = value; } }
    public float LineThickness { get { return lineThickness; } set { lineThickness = value; } }
    public Color LineColor { get { return lineColor; } set { lineColor = value; } }
    public Vector3 OriginPosition { get; set; }

    protected override void SetMaterialParameters(Material m)
    {
        m.SetVector("_WorldSpaceCameraPosition", cam.transform.position);
        m.SetFloat("_FadeOutDistance", fadeOutDistance);
        m.SetVector("_WorldSpaceOrigin", OriginPosition);
        m.SetColor("_Color", lineColor);
        m.SetFloat("_ColorExponent", colorExponent);
        m.SetFloat("_LineThickness", lineThickness);
        m.SetFloat("_LineCount", lineCount);
        m.SetVector("_LineDirection", lineDirection);
    }
}