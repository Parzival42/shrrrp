using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexNeighbourInfo {
	
	private Vector3 origin;
	private Vector3 neighbour;

	public VertexNeighbourInfo(Vector3 origin, Vector3 neighbour){
		this.origin = origin;
        this.neighbour = neighbour;	
	}

	public Vector3 Origin{
		get{return origin;}
	}

	public Vector3 Neighbour{
		get{return neighbour;}
	}
}
