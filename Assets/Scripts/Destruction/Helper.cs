using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper {

	#region variables
	private static Vector2 prev = new Vector2();
	private static Vector2 current = new Vector2();
	private static Vector2 next = new Vector2();
	#endregion



	#region methods


	/**
	 * calculates whether or not a vertex of the polygon is concave/reflex using the determinate
	 */
	public static bool isReflex(List<Vector2> p, int index){	
		prev = index == 0 ? p[p.Count-1] : p[index-1];
		current = p[index];
		next = index == p.Count-1 ? p[0] : p[index+1];
		
		float determinate = (current.x - prev.x) * (next.y -current.y) - (next.x - current.x) * (current.y - prev.y); 
		return determinate > 0 ? false : true;	
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





	#endregion

}
