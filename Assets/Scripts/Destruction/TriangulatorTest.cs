using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangulatorTest : MonoBehaviour {

	[SerializeField]
	private bool triangulate = false;

	private List<Vector3> polygon;


	private bool once = false;

	// Use this for initialization
	void Start () {
		polygon = GetComponent<Poly2DCreator>().GetPolygon();
	
	}

	void Update(){
		if(triangulate && !once){
			once = true;
			for(int i = 0; i < polygon.Count; i++){
				if(Helper.VectorIsIdentical(polygon[polygon.Count-1], polygon[i])){
					Debug.Log("identical!");
				}
			}
			
			List<Vector3> resultingTriangles = Triangulate(polygon);

			ShowTriangles(resultingTriangles);
		}
	}

	private void ShowTriangles(List<Vector3> vertices){
		for(int i = 0; i < vertices.Count; i+=3){
			Debug.DrawLine(vertices[i], vertices[i+1], Color.white, 5.0f, false);
			Debug.DrawLine(vertices[i+1], vertices[i+2], Color.white, 5.0f, false);
			Debug.DrawLine(vertices[i+2], vertices[i], Color.white, 5.0f, false);
		}
	}

    private List<Vector3> Triangulate(List<Vector3> polygon)
    {
		float time = Time.realtimeSinceStartup;

		//holds the clipped triangles
		List<Vector3> newTriangles = new List<Vector3>();

		//if the polygon only contains three vertices -> those three form a triangle -> done
		if(polygon.Count==3){
			newTriangles.Add(polygon[0]);
			newTriangles.Add(polygon[1]);
			newTriangles.Add(polygon[2]);
			return newTriangles;
		}

		//the index of the leftmost vertex
		int startIndex = 0;
		Vector3 left = polygon[startIndex];

		//find the leftmost vertex
		for(int i = 0; i < polygon.Count; i++){
			if(polygon[i].x < left.x || (Helper.VectorIsIdentical(polygon[i], left)&&(polygon[i].y < left.y))){
				startIndex = i;
				left = polygon[startIndex];
			}
		}

		//construct the first triangle using the leftmost vertex and its neighbours
		Vector3[] triangle = new Vector3[3];
		Helper.FillTriangle(startIndex, polygon, triangle);

		//get the triangle orientation (based on this orientation the other triangle orientations are meassured)
		bool orientation = Helper.orientation(triangle);

		//triangle indices are stored here that contradict the leftmost triangle orientation
		List<int> reflexIndices = new List<int>();

		while(polygon.Count>=3){
			reflexIndices.Clear();
			int ear = -1;
			
			for(int index = 0; index < polygon.Count; index++){

				//if ear has been found, stop searching
				if(ear >= 0){
					break;
				}

				//construct the current testing triangle configuration
				Helper.FillTriangle(index, polygon, triangle);

				//if the orientation is different to the inital one add the triangle index to the reflex list 
				if(Helper.orientation(triangle) != orientation){
					reflexIndices.Add(index);
					continue;
				}

				//assume the triangle to be an ear
				bool isEar = true;

				//get prev and next indices
				int prev = Helper.GetPreviousIndex(polygon, index);
				int next = Helper.GetNextIndex(polygon, index);

				//check for all reflex vertices of they are contained in the ear candiate
				for(int j = 0; j < reflexIndices.Count; j++){
					if(reflexIndices[j] == prev ||reflexIndices[j] == next){
						continue;
					}

					//if a reflex vertex is contained then the triangle is not an ear
					if(Helper.isPointInTriangle(polygon[reflexIndices[j]], polygon[prev], polygon[index], polygon[next])){
						isEar = false;
						break;
					}
				}

				//the ear is really an ear
				if(isEar){
					Vector3 start = polygon[index+1];
					Vector3 end = polygon[polygon.Count-1];

					int count = -1;
					while(!Helper.VectorIsIdentical(start,end)){
						count++;
						//Debug.Log(count);
						start = polygon[index+1+count];

						if(Helper.VectorIsIdentical(start, polygon[prev])||
							Helper.VectorIsIdentical(start, polygon[index]) ||
							Helper.VectorIsIdentical(start, polygon[next])){
							
							continue;
						}			

						if(Helper.isPointInTriangle(start, polygon[prev], polygon[index], polygon[next])){
							isEar = false;
							break;
						}			

						
					}

				}
				if(isEar){
					ear = index;
				}
				
			}

			//if the ear index is not valid exit
			if(ear<0){
				break;
				//return newTriangles;
			}
			
			Helper.FillTriangle(ear, polygon, triangle);
			newTriangles.AddRange(triangle);
			polygon.Remove(triangle[1]);
		}

		float duration = Time.realtimeSinceStartup - time;
		Debug.Log("triangulation duration: "+ duration);

		return newTriangles;
    }
}
