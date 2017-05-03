﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OutlinePreparator  {

	private List<Vector3> addedVertices = new List<Vector3>();

	private List<Vector3> addedNeighbours = new List<Vector3>();
	private int size = -1;
	private Dictionary<Vector3, List<Vector3>> neighbours = new Dictionary<Vector3, List<Vector3>>();

	private List<VertexNeighbourInfo> neighbourData = new List<VertexNeighbourInfo>();


	public void Add(Vector3 a, Vector3 b){
		neighbourData.Add(new VertexNeighbourInfo(a,b));
		Debug.Log("connection added!");

		for(int i = 0; i < neighbourData.Count; i++){
			neighbourData[i].Replace(a);		
			neighbourData[i].Replace(b);
		}	
	}
	public void AddVertexConnection(Vector3 a, Vector3 b){
		AddNeighbour(a,b);
		AddNeighbour(b,a);		
		size++;
	}

	private void AddNeighbour(Vector3 origin, Vector3 neighbour){
		int index = -1;
		for(int i = 0; i < neighbourData.Count; i++){
			if(Helper.VectorIsIdentical(neighbourData[i].Origin, origin)){
				index = i;	
			}
		}

		if(index == -1){			
			neighbourData.Add(new VertexNeighbourInfo(origin, neighbour));
			Debug.Log(neighbourData[neighbourData.Count-1].Origin);
			Debug.Log("new connection");

				
		}else{
			neighbourData[index].Neighbours.Add(neighbour);
			Debug.Log("connection already there");
		}

		for(int i = 0; i < neighbourData.Count; i++){
			if(index==-1){
				neighbourData[i].Replace(origin);
			}
			neighbourData[i].Replace(neighbour);

		}	

		

		// if(neighbours.ContainsKey(key)){
			
		// 	neighbours.TryGetValue(key, out n);
		// 	n.Add(neighbour);
		// }else{
		// 	n = new List<Vector3>();
		// 	Debug.Log("new connection");
		// 	n.Add(neighbour);
		// 	neighbours.Add(key, n);
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
		List<Vector3> orderedPolygon = new List<Vector3>();

		VertexNeighbourInfo origin = neighbourData[0];
		orderedPolygon.Add(origin.Origin);
		for(int i = 0; i < neighbourData.Count-1; i++){
			orderedPolygon.Add(origin.Neighbours[0]);
			origin = FindNeighbour(origin.Neighbours[0]);
		}
	
		VisualizePolygonFlow(orderedPolygon);
	
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
}