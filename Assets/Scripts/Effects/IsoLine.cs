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

    private static readonly string WORLD_SPACE_CAMERA = "_WorldSpaceCameraPosition";
    private static readonly string FADE_OUT_DISTANCE = "_FadeOutDistance";
    private static readonly string WORLD_SPACE_ORIGIN = "_WorldSpaceOrigin";
    private static readonly string COLOR = "_Color";
    private static readonly string COLOR_EXPONENT = "_ColorExponent";
    private static readonly string LINE_THICKNESS = "_LineThickness";
    private static readonly string LINE_COUNT = "_LineCount";
    private static readonly string LINE_DIRECTION = "_LineDirection";

    protected override void SetMaterialParameters(Material m)
    {
        m.SetVector(WORLD_SPACE_CAMERA, cam.transform.position);
        m.SetFloat(FADE_OUT_DISTANCE, fadeOutDistance);
        m.SetVector(WORLD_SPACE_ORIGIN, OriginPosition);
        m.SetColor(COLOR, lineColor);
        m.SetFloat(COLOR_EXPONENT, colorExponent);
        m.SetFloat(LINE_THICKNESS, lineThickness);
        m.SetFloat(LINE_COUNT, lineCount);
        m.SetVector(LINE_DIRECTION, lineDirection);
    }
}