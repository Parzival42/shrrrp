using UnityEngine;

[ExecuteInEditMode]
public abstract class BaseImageEffect : MonoBehaviour
{
    [FancyHeader("Base Image Effect", "General image effect settings")]
    [SerializeField]
    protected Shader currentShader;

    private Material currentMaterial;
    public Material material
    {
        get
        {
            if (currentMaterial == null)
            {
                currentMaterial = new Material(currentShader);
                currentMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return currentMaterial;
        }
    }

    protected virtual void Start()
    {
        if (!SystemInfo.supportsImageEffects)
            enabled = false;
        else if (!currentShader && !currentShader.isSupported)
            enabled = false;
    }

    protected virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (currentShader != null)
        {
            SetMaterialParameters(material);
            Graphics.Blit(source, destination, material);
        }
        else
            Graphics.Blit(source, destination);
    }

    protected virtual void OnDisable()
    {
        if (currentMaterial)
            DestroyImmediate(currentMaterial);
    }

    protected abstract void SetMaterialParameters(Material m);
}