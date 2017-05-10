using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Special image effect base class where the camera corner rays are
/// encoded into the quad vertices of the image quad.
/// Can be used to calculate the world position easily from
/// the depth buffer.
/// </summary>
public abstract class RaycastCornerImageEffect : BaseImageEffect
{
    protected Camera cam;

    protected override void Start()
    {
        base.Start();
        cam = GetComponent<Camera>();
    }

    protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (currentShader != null)
        {
            SetMaterialParameters(material);
            RaycastCornerBlit(source, destination, material);
        }
        else
            Graphics.Blit(source, destination);
    }

    protected virtual void RaycastCornerBlit(RenderTexture source, RenderTexture dest, Material mat)
    {
        // Compute Frustum Corners
        float camFar = cam.farClipPlane;
        float camFov = cam.fieldOfView;
        float camAspect = cam.aspect;

        float fovWHalf = camFov * 0.5f;

        Vector3 toRight = cam.transform.right * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camAspect;
        Vector3 toTop = cam.transform.up * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

        Vector3 topLeft = (cam.transform.forward - toRight + toTop);
        float camScale = topLeft.magnitude * camFar;

        topLeft.Normalize();
        topLeft *= camScale;

        Vector3 topRight = (cam.transform.forward + toRight + toTop);
        topRight.Normalize();
        topRight *= camScale;

        Vector3 bottomRight = (cam.transform.forward + toRight - toTop);
        bottomRight.Normalize();
        bottomRight *= camScale;

        Vector3 bottomLeft = (cam.transform.forward - toRight - toTop);
        bottomLeft.Normalize();
        bottomLeft *= camScale;

        // Custom Blit, encoding Frustum Corners as additional Texture Coordinates
        RenderTexture.active = dest;

        mat.SetTexture("_MainTex", source);

        GL.PushMatrix();
        GL.LoadOrtho();

        mat.SetPass(0);

        GL.Begin(GL.QUADS);

        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.MultiTexCoord(1, bottomLeft);
        GL.Vertex3(0.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.MultiTexCoord(1, bottomRight);
        GL.Vertex3(1.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.MultiTexCoord(1, topRight);
        GL.Vertex3(1.0f, 1.0f, 0.0f);

        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.MultiTexCoord(1, topLeft);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();
    }
}
