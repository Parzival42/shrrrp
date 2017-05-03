using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangulatorTest : MonoBehaviour {

	private void ShowTriangles(MeshContainer cap){
		int c = 0;
		for(int i = 0; i < cap.Indices[0].Count; i+=3){
			Debug.DrawLine(cap.Vertices[cap.Indices[0][i]], cap.Vertices[cap.Indices[0][i+1]], Color.white, 5.0f, false);
			Debug.DrawLine(cap.Vertices[cap.Indices[0][i+1]], cap.Vertices[cap.Indices[0][i+2]], Color.white, 5.0f, false);
			Debug.DrawLine(cap.Vertices[cap.Indices[0][i+2]], cap.Vertices[cap.Indices[0][i]], Color.white, 5.0f, false);
			c++;
		}

		Mesh mesh = new Mesh();
		mesh.name = "cap";
		mesh.SetVertices(cap.Vertices);
		mesh.SetIndices(cap.Indices[0].ToArray(), MeshTopology.Triangles, 0);
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		Debug.Log("triangles: "+c);
	}

    public MeshContainer Triangulate(List<Vector3> polygon)
    {
		float time = Time.realtimeSinceStartup;
		MeshContainer result = new MeshContainer();
		int removedVertices = 0;

		result.Vertices.AddRange(polygon);

		//holds the clipped triangles
		List<Vector3> newTriangles = new List<Vector3>();

		//if the polygon only contains three vertices -> those three form a triangle -> done
		if(polygon.Count==3){
			result.Indices[0].Add(0);
			result.Indices[0].Add(1);
			result.Indices[0].Add(2);
			return result;
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

		//Debug.DrawLine(left, left+Vector3.up, Color.red, 5.0f, false);

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
					Vector3 start = polygon[Helper.GetNextIndex(polygon, index)];
					Vector3 end = polygon[polygon.Count-1];

					int count = -1;
					while(!Helper.VectorIsIdentical(start,end)){
						count++;
						//Debug.Log(count);
						start = polygon[Helper.GetNextIndex(polygon, index)+count];

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
			
			result.Indices[0].Add(Helper.GetPreviousIndex(polygon, ear)+removedVertices);
			result.Indices[0].Add(ear+removedVertices);
			result.Indices[0].Add(Helper.GetNextIndex(polygon, ear)+removedVertices);

			// result.Indices[0].Add(Helper.GetNextIndex(polygon, ear)+removedVertices);
			// result.Indices[0].Add(ear+removedVertices);
			// result.Indices[0].Add(Helper.GetPreviousIndex(polygon, ear)+removedVertices);
			

			polygon.Remove(triangle[1]);
			removedVertices++;
		}

		float duration = Time.realtimeSinceStartup - time;
		Debug.Log("triangulation duration: "+ duration);

		//ShowTriangles(result);

		return result;
    }
}
