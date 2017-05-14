using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Triangulator 
{

	public static MeshContainer Triangulate( List<Vector3> vertices, int pointCoordA, int pointCoordB )
	{
		MeshContainer mesh = new MeshContainer();
		mesh.Vertices.AddRange(vertices);

		int numberOfPoints = vertices.Count;
		List<int> usePoints = new List<int>();
		for(int p=0; p<numberOfPoints; p++){
			usePoints.Add(p);
		}
		int numberOfUsablePoints = usePoints.Count;
		
        if (numberOfPoints < 3){
			Debug.Log("new triangulator triangles: "+mesh.Indices[0].Count);
            return mesh;
		}
		
		int it = 100;
		while(numberOfUsablePoints > 3)
		{
			for(int i=0; i<numberOfUsablePoints; i++)
			{
				int a,b,c;
				
				a=usePoints[i];
				
				if(i>=numberOfUsablePoints-1)
					b=usePoints[0];
				else
					b=usePoints[i+1];
				
				if(i>=numberOfUsablePoints-2)
					c=usePoints[(i+2)-numberOfUsablePoints];
				else
					c=usePoints[i+2];
				
				Vector2 pA = new Vector2(vertices[b][pointCoordA], vertices[b][pointCoordB]);
				Vector2 pB = new Vector2(vertices[a][pointCoordA], vertices[a][pointCoordB]);
				Vector2 pC = new Vector2(vertices[c][pointCoordA], vertices[c][pointCoordB]);
				
				float dA = Vector2.Distance(pA,pB);
				float dB = Vector2.Distance(pB,pC);
				float dC = Vector2.Distance(pC,pA);
				
				float angle = Mathf.Acos((Mathf.Pow(dB,2)-Mathf.Pow(dA,2)-Mathf.Pow(dC,2))/(2*dA*dC))*Mathf.Rad2Deg *
				 Mathf.Sign(Sign(new Vector2(vertices[a][pointCoordA], vertices[a][pointCoordB]),new Vector2(vertices[b][pointCoordA], vertices[b][pointCoordB]),new Vector2(vertices[c][pointCoordA], vertices[c][pointCoordB])));
				if(angle < 0)
				{
					continue;//angle is not reflex
				}
				
				bool freeOfIntersections = true;
				for(int p=0; p<numberOfUsablePoints; p++)
				{
					int pu = usePoints[p];
					if(pu==a || pu==b || pu==c)
						continue;
					
					if(IntersectsTriangle2(new Vector2(vertices[a][pointCoordA], vertices[a][pointCoordB]),new Vector2(vertices[b][pointCoordA], vertices[b][pointCoordB]),new Vector2(vertices[c][pointCoordA], vertices[c][pointCoordB]),new Vector2(vertices[pu][pointCoordA], vertices[pu][pointCoordB])))
					{
						freeOfIntersections=false;
						break;
					}
				}
				
				if(freeOfIntersections)
				{
					mesh.Indices[0].Add(a);
					mesh.Indices[0].Add(b);
					mesh.Indices[0].Add(c);
					usePoints.Remove(b);
					it=100;
					numberOfUsablePoints = usePoints.Count;
					i--;
					break;
				}
			}
			it--;
			if(it<0)
				break;
		}
		
		mesh.Indices[0].Add(usePoints[0]);
		mesh.Indices[0].Add(usePoints[1]);
		mesh.Indices[0].Add(usePoints[2]);
		//mesh.Indices[0].Reverse();
			
		Debug.Log("new triangulator triangles: "+mesh.Indices[0].Count);
		
		return mesh;
	}
	
	private static bool IntersectsTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
	{
		bool b1, b2, b3;

		b1 = Sign(P, A, B) < 0.0f;
		b2 = Sign(P, B, C) < 0.0f;
		b3 = Sign(P, C, A) < 0.0f;
		
		return ((b1 == b2) && (b2 == b3));
	}
	
	private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
	{
		return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
	}
					
	private static bool IntersectsTriangle2(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
	{
			float planeAB = (A.x-P.x)*(B.y-P.y)-(B.x-P.x)*(A.y-P.y);
			float planeBC = (B.x-P.x)*(C.y-P.y)-(C.x - P.x)*(B.y-P.y);
			float planeCA = (C.x-P.x)*(A.y-P.y)-(A.x - P.x)*(C.y-P.y);
			return Sign(planeAB)==Sign(planeBC) && Sign(planeBC)==Sign(planeCA);
	}
	
	private static int Sign(float n) 
	{
		return (int)(Mathf.Abs(n)/n);
	}
}