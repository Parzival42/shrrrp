using UnityEngine;

[RequireComponent(typeof(Camera))]
public abstract class ReplacementShader : MonoBehaviour
{
    [FancyHeader("Settings", "Replacement Shader Settings")]
    [SerializeField]
    protected Shader replacementShader;

    protected Camera cam;
    protected bool shaderEnabled = false;

    protected virtual void Awake()
    {
        cam = GetComponent<Camera>();
    }

	protected virtual void Start ()
    {
	}

    public virtual void ReplaceShader()
    {
        shaderEnabled = true;
        cam.SetReplacementShader(replacementShader, "");
    }

    public virtual void DisableShader()
    {
        shaderEnabled = false;
        cam.ResetReplacementShader();
    }

    protected virtual void OnDisable()
    {
        DisableShader();
    }

    //protected virtual void OnEnable()
    //{
    //    ReplaceShader();
    //}

    protected virtual void Update()
    {
        if(shaderEnabled)
            SetShaderParameters();
    }

    /// <summary>
    /// The specific shader parameters of each replacement shader should
    /// be set in here.
    /// </summary>
    protected abstract void SetShaderParameters();
}