using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Poly2DCreator : MonoBehaviour {

	[SerializeField]
	private int segments;

	[SerializeField]
	private float size;

	private LineRenderer lineRenderer;

	private Vector3[] points;


	void Awake () {
		lineRenderer = gameObject.GetComponent<LineRenderer>();
		
		points = new Vector3[segments+1];
		float radius = 1.0f * (size +1);
        for(int i = 0; i < segments; i++){
            float angle = 2*Mathf.PI*i/(float)segments;
            float x = radius*(float)Mathf.Cos(angle)+ Random.Range(0.0f,1.0f) * size;
            float y = radius*(float)Mathf.Sin(angle)+ Random.Range(0.0f,1.0f) * size;
			points[i] = (new Vector3(x,y,0));
        }
		points[points.Length-1] = points[0];

		// points = new Vector3[9];
		// points[0] = new Vector3(5,0,0);
		// points[1] = new Vector3(0,0,0);
		// points[2] = new Vector3(-5,0,0);
		// points[3] = new Vector3(-10,0,0);
		
		// points[4] = new Vector3(-10,5,0);

		// points[5] = new Vector3(-5,5,0);
		
		// points[6] = new Vector3(0,5,0);
		// points[7] = new Vector3(5,5,0);
		// points[8] = new Vector3(5,0,0);
		// lineRenderer.positionCount = 9;

		lineRenderer.positionCount = segments+1;
		lineRenderer.SetPositions(points);
	}

	public List<Vector3> GetPolygon(){
		List<Vector3> polygon = new List<Vector3>();
		for(int i = 0; i < points.Length-1; i++){
			polygon.Add(points[i]);
		}		
		return polygon;
	}
}
