using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper {

	#region variables
	private static Vector3 prev = new Vector3();
	private static Vector3 current = new Vector3();
	private static Vector3 next = new Vector3();

	private static float epsilon = Mathf.Epsilon*5.0f;
	#endregion



	#region methods


	/**
	 * calculates whether or not a vertex of the polygon is concave/reflex using the determinate
	 */
	public static bool orientation(List<Vector3> p, int index){	
		prev = index == 0 ? p[p.Count-1] : p[index-1];
		current = p[index];
		next = index == p.Count-1 ? p[0] : p[index+1];
		return orientation(prev, current, next);			
	}

	public static bool orientation(Vector3[] triangle){
		return orientation(triangle[0], triangle[1], triangle[2]);
	}

	public static bool orientation(Vector3 prev, Vector3 current, Vector3 next){
		float determinate = (current.x - prev.x) * (next.y -prev.y) - (next.x - prev.x) * (current.y - prev.y); 
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
	public static bool isPointInTriangle(Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2) {
		var A = 1/2 * (-p1.y * p2.x + p0.y * (-p1.x + p2.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y);
		var sign = A < 0 ? -1 : 1;
		var s = (p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y) * sign;
		var t = (p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y) * sign;
		
		return s > 0 && t > 0 && (s + t) < 2 * A * sign;
	}

	public static bool FloatIsIdentical(float a, float b){
		return Mathf.Approximately(a,b);
	}

	public static bool FloatIsIdentical(float a, float b, float epsilon){
		return a > b - epsilon && a < b + epsilon;
	}

	public static bool VectorIsIdentical(Vector3 a, Vector3 b){
		return Vector3.SqrMagnitude(a-b)< 1e-005f;
	}

	public static void FillTriangle(int index, List<Vector3> polygon, Vector3[] triangle){
		int prev = GetPreviousIndex(polygon, index);
		int next = GetNextIndex(polygon, index);

		triangle[0] = polygon[prev];
		triangle[1] = polygon[index];
		triangle[2] = polygon[next];
	}


	/**
	 * Calculates where a vector is positioned in comparison to another
	 */
	 public static int GetRelativePosition(Vector3 reference, Vector3 target){
		 if(VectorIsIdentical(reference, target)){
			 return 0;
		 }

		 if(target.x < reference.x || 
		 	(FloatIsIdentical(target.x, reference.x) && target.y < reference.y) || 
			(FloatIsIdentical(target.x, reference.x) && FloatIsIdentical(target.y, reference.y) && target.z < reference.z)){

			return -1;	
		 }else{
			 return 1;
		 }
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


	#endregion
}
