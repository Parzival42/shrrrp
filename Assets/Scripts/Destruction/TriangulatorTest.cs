using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangulatorTest : MonoBehaviour {
	//private Vector2[] polygon;

	private List<Vector2> polygon;
	private List<bool> polygonOrientation;

	// Use this for initialization
	void Start () {
		polygon = GetComponent<Poly2DCreator>().GetPolygon();
		polygonOrientation = new List<bool>();


		Triangulate(polygon);
	}

    private void Triangulate(List<Vector2> polygon)
    {
		float time = Time.realtimeSinceStartup;

		
        for(int i = 0; i < polygon.Count; i++){
			polygonOrientation.Add(Helper.isReflex(polygon, i));	
		}
	
		bool pInTriangle = Helper.isPointInTriangle(new Vector2(0,0), new Vector2(-3,-3), new Vector2(0,4), new Vector2(3,0)); 

		float duration = Time.realtimeSinceStartup - time;
		Debug.Log("pInTri: "+ duration);
    }
}
