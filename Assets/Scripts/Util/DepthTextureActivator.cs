using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class DepthTextureActivator : MonoBehaviour
{
    [Comment("The only purpose of this script is to activate set the depth mode of the camera."
        + "Some scripts require the depth to be written.")]

    [FancyHeader("Depth Mode", "Choose a depth mode!")]
    [SerializeField]
    private DepthTextureMode depthMode = DepthTextureMode.DepthNormals;

	private void Start ()
    {
        Camera cam = GetComponent<Camera>();
        cam.depthTextureMode = depthMode;
    }
}
