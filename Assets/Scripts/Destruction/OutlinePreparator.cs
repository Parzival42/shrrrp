using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OutlinePreparator : MonoBehaviour {

	private List<Vector3> addedVertices = new List<Vector3>();

	private List<Vector3> addedNeighbours = new List<Vector3>();
	private int size = -1;
	private Dictionary<Vector3, List<Vector3>> neighbours = new Dictionary<Vector3, List<Vector3>>();

	private List<VertexNeighbourInfo> neighbourData = new List<VertexNeighbourInfo>();

	private List<Vector3> orderedPolygon = new List<Vector3>();

	public void Add(Vector3 a, Vector3 b){
		neighbourData.Add(new VertexNeighbourInfo(a,b));
		//Debug.Log("connection added!");

		//Debug.Log(a + " -> " + b);
		//DebugExtension.DebugArrow(a,b-a,Color.blue, 10.0f);

		// for(int i = 0; i < neighbourData.Count; i++){
		// 	neighbourData[i].Replace(a);		
		// 	neighbourData[i].Replace(b);
		// }	
	}

	private VertexNeighbourInfo FindNeighbour(Vector3 origin){
		for(int i = 0; i < neighbourData.Count; i++){
			if(Helper.VectorIsIdentical(neighbourData[i].Origin, origin)){
				return neighbourData[i];
			}
		}
		return null;
	}

	public List<Vector3> PrepareOutlinePolygon(){
		Debug.Log("neighbours: "+ neighbourData.Count);
		if(neighbourData.Count>0){
			VertexNeighbourInfo origin = neighbourData[1];
			orderedPolygon.Add(origin.Origin);
			
			orderedPolygon.Add(origin.Neighbours[0]);
			//Debug.Log(orderedPolygon[0] + " -> " + orderedPolygon[1]);

			for(int i = 0; i < neighbourData.Count-1; i++){
				origin = FindNeighbour(orderedPolygon[orderedPolygon.Count-1]);
				if(origin==null || Helper.VectorIsIdentical(origin.Origin, orderedPolygon[orderedPolygon.Count-2])){
					break;
				}
				orderedPolygon.Add(origin.Neighbours[0]);
				//Debug.Log(orderedPolygon[orderedPolygon.Count-2]+" -> "+orderedPolygon[orderedPolygon.Count-1]);
				//origin = FindNeighbour(origin.Neighbours[0]);
			}
		
			VisualizePolygonFlow(orderedPolygon);
		}

		

		
	
		return orderedPolygon;
	}

	public List<Vector3> PrepareOutlinePolygon( bool visualize){
		List<Vector3> orderedPolygon = new List<Vector3>();

		Debug.Log("amount: "+size+"  :  hashmap size: "+neighbours.Count+"   :  addedVertices size: "+addedVertices.Count+"   epsilon: "+Mathf.Epsilon);

		orderedPolygon.Add(neighbourData[0].Origin);	
		orderedPolygon.Add(neighbourData[0].Neighbours[0]);

		for(int i = 0; i < 3; i++){
			VertexNeighbourInfo n = FindNeighbour(orderedPolygon[orderedPolygon.Count-1]);
			bool found = false;
			for(int j = 0; j < n.Neighbours.Count; j++){
				if(!Helper.VectorIsIdentical(n.Neighbours[j], orderedPolygon[orderedPolygon.Count-2])){
					orderedPolygon.Add(n.Neighbours[j]);
					found = true;
				}

				if(i == 2){
					Debug.DrawLine(orderedPolygon[4], orderedPolygon[4]+Vector3.up, Color.red);
					Debug.DrawLine(orderedPolygon[5], orderedPolygon[5]+Vector3.up, Color.green);
				}
			}

			if(!found){
				break;
			}
		}
	
		if(visualize){
			VisualizePolygonFlow(orderedPolygon);
		}
		return orderedPolygon;
	}



	private void VisualizePolygonFlow(List<Vector3> orderedVertices){
		for(int i = 0; i < orderedVertices.Count-1; i++){
			Debug.DrawLine(orderedVertices[i], orderedVertices[i+1], Color.cyan);
		}

		Debug.DrawLine(orderedVertices[0], orderedVertices[orderedVertices.Count-1], Color.cyan);
	}

	void OnDrawGizmos(){
		Debug.Log("draw");
		if(neighbourData != null && neighbourData.Count >1){
			for(int i = 0; i < neighbourData.Count; i++){
				DebugExtension.DebugArrow(neighbourData[i].Origin, neighbourData[i].Neighbours[0]-neighbourData[i].Origin, Color.black, 10.0f);
			}

			//DebugExtension.DebugArrow(orderedPolygon[0], orderedPolygon[orderedPolygon.Count-1], Color.cyan);
		}
	}
}
