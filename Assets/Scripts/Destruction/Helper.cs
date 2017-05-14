﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper {

	#region variables
	private static Vector3 prev = new Vector3();
	private static Vector3 current = new Vector3();
	private static Vector3 next = new Vector3();

	private static Vector2 compareA = new Vector2();

	private static Vector2 compareB = new Vector2();
	private static float epsilon = Mathf.Epsilon*5.0f;
	#endregion



	#region methods


	/**
	 * calculates whether or not a vertex of the polygon is concave/reflex using the determinate
	 */
	// public static bool orientation(List<Vector3> p, int index){	
	// 	prev = index == 0 ? p[p.Count-1] : p[index-1];
	// 	current = p[index];
	// 	next = index == p.Count-1 ? p[0] : p[index+1];
	// 	return orientation(prev, current, next);			
	// }

	public static bool orientation(Vector3[] triangle, int projectCoordA, int projectCoordB){
		return orientation(triangle[0], triangle[1], triangle[2], projectCoordA, projectCoordB);
	}

	public static bool orientation(Vector3 prev, Vector3 current, Vector3 next, int projectCoordA, int projectCoordB){
		float determinate = (current[projectCoordA] - prev[projectCoordA]) * (next[projectCoordB] -prev[projectCoordB]) - (next[projectCoordA] - prev[projectCoordA]) * (current[projectCoordB] - prev[projectCoordB]); 
		return determinate > 0;
	}

	/**
	 * gets the next valid index
	 */
	 public static int GetNextIndex(List<Vector3> polygon, int index){
		 return index < polygon.Count - 1 ? index + 1 : 0;
	 }

	 /**
	 * gets the previous valid index
	 */
	 public static int GetPreviousIndex(List<Vector3> polygon, int index){
		return index > 0 ? index - 1 : polygon.Count - 1;
	 }

	/**
	 * calculates whether or not a point lies within the specified triangle
	 */
	public static bool isPointInTriangle(Vector3 p, Vector3 p0, Vector3 p1, Vector3 p2, int projectCoordA, int projectCoordB) {
		var A = 1/2 * (-p1[projectCoordB] * p2[projectCoordA] + p0[projectCoordB] * (-p1[projectCoordA] + p2[projectCoordA]) + p0[projectCoordA] * (p1[projectCoordB] - p2[projectCoordB]) + p1[projectCoordA] * p2[projectCoordB]);
		var sign = A < 0 ? -1 : 1;
		var s = (p0[projectCoordB] * p2[projectCoordA] - p0[projectCoordA] * p2[projectCoordB] + (p2[projectCoordB] - p0[projectCoordB]) * p[projectCoordA] + (p0[projectCoordA] - p2[projectCoordA]) * p[projectCoordB]) * sign;
		var t = (p0[projectCoordA] * p1[projectCoordB] - p0[projectCoordB] * p1[projectCoordA] + (p0[projectCoordB] - p1[projectCoordB]) * p[projectCoordA] + (p1[projectCoordA] - p0[projectCoordA]) * p[projectCoordB]) * sign;
		
		return s > 0 && t > 0 && (s + t) < 2 * A * sign;
	}

	public static bool FloatIsIdentical(float a, float b){
		return FloatIsIdentical(a,b, 1e-0006f);
	}

	public static bool FloatIsIdentical(float a, float b, float epsilon){
		return a > b - epsilon && a < b + epsilon;
	}

	public static bool VectorIsIdentical(Vector3 a, Vector3 b){
		return Vector3.SqrMagnitude(a-b)< 1e-008f;
	}

	public static bool Vector2IsIdentical(float x1, float y1, float x2, float y2){
		compareA.Set(x1,y1);
		compareB.Set(x2,y2);
		return Vector2.SqrMagnitude(compareA-compareB)< 1e-0005f;
	}

	public static void FillTriangle(int index, List<Vector3> polygon, Vector3[] triangle){
		int prev = GetPreviousIndex(polygon, index);
		int next = GetNextIndex(polygon, index);

		triangle[0] = polygon[prev];
		triangle[1] = polygon[index];
		triangle[2] = polygon[next];
	}


	 //mesh util?
	 public static void FlipTriangles(List<int>[] indices){
		for(int i = 0; i < indices.Length; i++){
			for(int j = 0; j < indices[i].Count; j+=3){
				int temp = indices[i][j];
				indices[i][j] = indices[i][j+2];
				indices[i][j+2] = temp;
			}
		}
	 }

	 public static void DrawTriangles(MeshContainer container){

		List<int>[] triangles = container.Indices;

		for(int i = 0; i < triangles.Length; i++){
			for(int j = 0; j < triangles[i].Count; j+=3){
				Debug.DrawLine(container.Vertices[triangles[i][j]], container.Vertices[triangles[i][j+1]], Color.white, 10.0f);
				Debug.DrawLine(container.Vertices[triangles[i][j+1]], container.Vertices[triangles[i][j+2]], Color.white, 10.0f);
				Debug.DrawLine(container.Vertices[triangles[i][j+2]], container.Vertices[triangles[i][j]], Color.white, 10.0f);
			}
		}
	 }

	 public static void UnProjectVertices(Transform transform, MeshContainer meshContainer){
		for(int i = 0; i < meshContainer.Vertices.Count; i++){
			meshContainer.Vertices[i] = transform.InverseTransformPoint(meshContainer.Vertices[i]);
		}
	 }


	 public static bool IsPolygonClockwise(List<Vector3> vertices, int projectCoordA, int projectCoordB){
		float sum = 0.0f;

		for(int i = 0; i < vertices.Count; i++){
			int next = Helper.GetNextIndex(vertices, i);
			sum += (vertices[next][projectCoordA] - vertices[i][projectCoordA])*(vertices[next][projectCoordB] + vertices[i][projectCoordB]);
		}

		return sum < 0.0f;
	 }


	#endregion
}
