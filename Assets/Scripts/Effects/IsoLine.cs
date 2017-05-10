using UnityEngine;

[RequireComponent(typeof(Camera))]
public class IsoLine : BaseImageEffect
{
    [FancyHeader("Isoline", "Isoline image effect parameters")]
    [SerializeField]
    private Color lineColor = Color.white;

    [SerializeField]
    private float lineThickness = 0.5f;

    [SerializeField]
    private Vector4 lineDirection = Vector3.zero;

    private Camera cam;

    protected override void Start()
    {
        base.Start();
        cam = GetComponent<Camera>();
    }

    protected override void SetMaterialParameters(Material m)
    {
        m.SetVector("_WorldSpaceCameraPosition", cam.transform.position);
        m.SetColor("_Color", lineColor);
        m.SetFloat("_LineThickness", lineThickness);
        m.SetVector("_LineDirection", lineDirection);
    }
}