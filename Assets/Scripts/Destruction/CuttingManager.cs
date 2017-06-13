using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CuttingManager  {

	void Cut(Transform planeTransform, Mesh planeMesh);

	void Cut(Transform planeTransform, Mesh planeMesh, float delay);
}
