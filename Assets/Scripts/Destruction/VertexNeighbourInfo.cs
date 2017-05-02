using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexNeighbourInfo {
	
	private Vector3 origin;
	private List<Vector3> neighbours;

	public VertexNeighbourInfo(Vector3 origin, Vector3 neighbour){
		this.origin = origin;
		neighbours = new List<Vector3>();
		neighbours.Add(neighbour);
	}

	public void Replace(Vector3 replacer){
		if(Helper.VectorIsIdentical(origin, replacer)){
			origin = replacer;
		}

		for(int i =0 ; i < neighbours.Count; i++){
			if(Helper.VectorIsIdentical(neighbours[i], replacer)){
				neighbours[i] = replacer;
			}
		}
	}

	public Vector3 Origin{
		get{return origin;}
	}

	public List<Vector3> Neighbours{
		get{return neighbours;}
	}
}
