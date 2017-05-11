using UnityEngine;

public class ColorOverlay : BaseImageEffect
{
    [FancyHeader("Color overlay", "Setting for color overlay effect")]
    [SerializeField]
    private Color overlayColor = Color.white;

    [SerializeField]
    [Range(0f, 1f)]
    private float overlayStrength = 0.5f;

    public Color OverlayColor { get { return overlayColor; } set { overlayColor = value; } }
    public float OverlayStrength { get { return overlayStrength; } set { overlayStrength = value; } }

    private static readonly string COLOR_OVERLAY = "_ColorOverlay";
    private static readonly string COLOR_STRENGTH = "_OverlayStrength";

    protected override void SetMaterialParameters(Material m)
    {
        m.SetColor(COLOR_OVERLAY, overlayColor);
        m.SetFloat(COLOR_STRENGTH, overlayStrength);
    }
}