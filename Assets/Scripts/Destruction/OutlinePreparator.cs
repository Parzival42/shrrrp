using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OutlinePreparator{

	private List<VertexNeighbourInfo> neighbourData = new List<VertexNeighbourInfo>();

	public void Add(Vector3 a, Vector3 b){
		neighbourData.Add(new VertexNeighbourInfo(a,b));
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

		if(neighbourData.Count>0){
			VertexNeighbourInfo origin = neighbourData[0];

			orderedPolygon.Add(origin.Origin);			
			orderedPolygon.Add(origin.Neighbour);

            while(!Helper.VectorIsIdentical(orderedPolygon[0], orderedPolygon[orderedPolygon.Count - 1])){
                origin = FindNeighbour(orderedPolygon[orderedPolygon.Count - 1]);
                if (origin == null)
                {
                    break;
                }
                orderedPolygon.Add(origin.Neighbour);
            }

            orderedPolygon.RemoveAt(orderedPolygon.Count - 1);
           
			//VisualizePolygonFlow(orderedPolygon);
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
