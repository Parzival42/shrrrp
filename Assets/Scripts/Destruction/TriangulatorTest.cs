﻿using System.Collections.Generic;
using UnityEngine;

public class TriangulatorTest {

    public MeshContainer Triangulate(List<Vector3> polygon, int projectCoordA, int projectCoordB)
    {
		//float time = Time.realtimeSinceStartup;
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

			//Debug.Log("Polygon is size of 3 only!!!!!!");
			return result;
		}

		//the index of the leftmost vertex
		int startIndex = 0;
		Vector3 left = polygon[startIndex];

		//find the leftmost vertex
		for(int i = 0; i < polygon.Count; i++){
			if(polygon[i][projectCoordA] < left[projectCoordA] || (polygon[i][projectCoordA] == left[projectCoordA] &&(polygon[i][projectCoordB] < left[projectCoordB]))){
				startIndex = i;
				left = polygon[startIndex];
			}
		}

		//Debug.DrawLine(left, left+Vector3.up, Color.red, 5.0f, false);

		//construct the first triangle using the leftmost vertex and its neighbours
		Vector3[] triangle = new Vector3[3];
		Helper.FillTriangle(startIndex, polygon, triangle);

		//get the triangle orientation (based on this orientation the other triangle orientations are meassured)
		bool orientation = Helper.orientation(triangle, projectCoordA, projectCoordB);
		bool convex = true;
		for(int i = 0; i < polygon.Count; i++){
			Helper.FillTriangle(i, polygon, triangle);
			if(Helper.orientation(triangle, projectCoordA, projectCoordB) != orientation){
				Debug.Log("orientation different");
				convex  = false;
			}
		}

		Debug.Log("Polygon is convex: "+convex);

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
				if(Helper.orientation(triangle, projectCoordA, projectCoordB) != orientation){
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
					if(Helper.PointInTriangle(polygon[reflexIndices[j]], polygon[prev], polygon[index], polygon[next], projectCoordA, projectCoordB)){
						Debug.Log("not an ear");
						isEar = false;
						break;
					}
				}

				//the ear is really an ear
				if(isEar){
					Vector3 start = polygon[Helper.GetNextIndex(polygon, index)];
					Vector3 end = polygon[polygon.Count-1];

					int count = -1;
					while(!Helper.Vector2IsIdentical(start[projectCoordA], start[projectCoordB],end[projectCoordA], end[projectCoordB])){
						count++;
						//Debug.Log(count);
						if(Helper.GetNextIndex(polygon, index)+count >= polygon.Count){
							break;
						}
						start = polygon[Helper.GetNextIndex(polygon, index)+count];

						if(Helper.Vector2IsIdentical(start[projectCoordA], start[projectCoordB], polygon[prev][projectCoordA], polygon[prev][projectCoordB])||
							Helper.Vector2IsIdentical(start[projectCoordA], start[projectCoordB], polygon[index][projectCoordA], polygon[index][projectCoordB]) ||
							Helper.Vector2IsIdentical(start[projectCoordA], start[projectCoordB], polygon[next][projectCoordA], polygon[index][projectCoordB])){
							
							continue;
						}			

						if(Helper.isPointInTriangle(start, polygon[prev], polygon[index], polygon[next], projectCoordA, projectCoordB)){
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

		//Debug.Log("polygon leftovers: "+ polygon.Count);

		//float duration = Time.realtimeSinceStartup - time;
		//Debug.Log("triangulation duration: "+ duration);
		return result;
    }
}
