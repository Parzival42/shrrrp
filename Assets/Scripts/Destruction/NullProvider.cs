using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullProvider : CuttingManager
{
    public void Cut(Transform planeTransform, Mesh planeMesh)
    {
        Debug.Log("Null Provider Action");
    }

    public void Cut(Transform planeTransform, Mesh planeMesh, float delay)
    {
        Cut(planeTransform, planeMesh);
    }
}
