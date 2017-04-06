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

		lineRenderer.positionCount = segments+1;
		lineRenderer.SetPositions(points);
	}

	public List<Vector2> GetPolygon(){
		List<Vector2> polygon = new List<Vector2>();
		for(int i = 0; i < points.Length-1; i++){
			polygon.Add(new Vector2(points[i].x, points[i].y));
		}		
		return polygon;
	}
}
