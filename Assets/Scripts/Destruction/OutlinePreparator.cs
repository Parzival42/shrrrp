using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OutlinePreparator{

	private List<VertexNeighbourInfo> neighbourData = new List<VertexNeighbourInfo>();
  
	public void Add(Vector3 a, Vector3 b){
		neighbourData.Add(new VertexNeighbourInfo(a,b));
	}

	private int FindNeighbour(Vector3 origin){
		for(int i = 0; i < neighbourData.Count; i++){
			if(Helper.VectorIsIdentical(neighbourData[i].Origin, origin)){
                return i;
			}
		}
		return -1;
	}

	public List<Vector3> PrepareOutlinePolygon(){
        List<Vector3> orderedPolygon = new List<Vector3>();
        
		if(neighbourData.Count>0){
			VertexNeighbourInfo origin = neighbourData[0];
			orderedPolygon.Add(origin.Origin);			
			orderedPolygon.Add(origin.Neighbour);

            while(!Helper.VectorIsIdentical(orderedPolygon[0], orderedPolygon[orderedPolygon.Count - 1])){
                int next = FindNeighbour(orderedPolygon[orderedPolygon.Count - 1]);
                if (next == -1)
                {
                    break;
                }
                orderedPolygon.Add(neighbourData[next].Neighbour);
            }
            orderedPolygon.RemoveAt(orderedPolygon.Count - 1);

		}

        return orderedPolygon;
	}



    public List<List<Vector3>> PrepareOutlinePolygons()
    {
        List<List<Vector3>> orderedPolygon = new List<List<Vector3>>();
        int currentList = -1;

        while (neighbourData.Count > 2)
        {
            Debug.Log("test");
            VertexNeighbourInfo origin = neighbourData[0];
            orderedPolygon.Add(new List<Vector3>());
            currentList++;

            orderedPolygon[currentList].Add(origin.Origin);
            orderedPolygon[currentList].Add(origin.Neighbour);
            neighbourData.RemoveAt(0);

            while (!Helper.VectorIsIdentical(orderedPolygon[currentList][0], orderedPolygon[currentList][orderedPolygon[currentList].Count - 1]))
            {
                int next = FindNeighbour(orderedPolygon[currentList][orderedPolygon[currentList].Count - 1]);
                if (next == -1)
                {
                    break;
                }
                orderedPolygon[currentList].Add(neighbourData[next].Neighbour);
                neighbourData.RemoveAt(next);
            }
            orderedPolygon[currentList].RemoveAt(orderedPolygon[currentList].Count - 1);


            if (orderedPolygon[currentList].Count < 3)
            {
                orderedPolygon.RemoveAt(currentList);
                currentList--;
            }
        }
       
        return orderedPolygon;
    }
}
