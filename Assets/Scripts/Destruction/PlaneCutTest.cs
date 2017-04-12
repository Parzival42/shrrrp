using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SliceCreator))]
public class PlaneCutTest : MonoBehaviour {
    
	[SerializeField]
	private GameObject referencePlane;

	[SerializeField]
	private bool cut;

	private Plane cuttingPlane;

    private int[] vertexPosChange;

    Mesh M;    

    // 
	private List<Tri> conflictTriangles;

    // slice creator 
    [SerializeField]
    private SliceCreator sliceCreator;

    // leftover meshes 
    private MeshContainer leftMesh = new MeshContainer();

    private MeshContainer rightMesh = new MeshContainer();

    // cutted triangles are stored here
    private MeshContainer splitMeshLeft = new MeshContainer();

    private MeshContainer splitMeshRight = new MeshContainer();

    public struct Tri{
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;

        public bool aLeft;
        public bool bLeft;
        public bool cLeft;

        public Tri(Vector3 a, Vector3 b, Vector3 c, bool aLeft, bool bLeft, bool cLeft){
            this.a = a;
            this.b = b;
            this.c = c;

            this.aLeft = aLeft;
            this.bLeft = bLeft;
            this.cLeft = cLeft;
        }
    }

    private void SplitTriangle(Plane p, Tri t){
        Ray ray1 = new Ray(t.a, t.b - t.a);
        Ray ray2 = new Ray(t.a, t.c - t.a);

        Vector3 d = new Vector3();
        Vector3 e = new Vector3();
        float rayDistance1;
        if (p.Raycast(ray1, out rayDistance1)){
            d= ray1.GetPoint(rayDistance1);
        }

        float rayDistance2;
        if (p.Raycast(ray2, out rayDistance2)){
            e= ray2.GetPoint(rayDistance2);
        }
        
        Debug.DrawLine(d,e);     
    }



    void StartSplitInTwo()
    {
        if (GetComponent<MeshFilter>() != null)
        {
            M = GetComponent<MeshFilter>().mesh;
        }
       
        vertexPosChange = new int[M.vertexCount];

		conflictTriangles = new List<Tri>();
    
		Mesh pM = referencePlane.GetComponent<MeshFilter>().mesh;
		Transform pT = referencePlane.transform;
		cuttingPlane.Set3Points(pT.TransformPoint(pM.vertices[pM.triangles[0]]), pT.TransformPoint(pM.vertices[pM.triangles[1]]),pT.TransformPoint(pM.vertices[pM.triangles[2]]));

      
        SplitInTwo();

      	Destroy(this.gameObject);
    }

	void Update () {
		if(cut){
			StartSplitInTwo();			
		}
	}

    private void DetermineVertexPositions(Vector3[] vertices, Vector3[] normals, Vector2[] uvs)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            if (cuttingPlane.GetSide(transform.TransformPoint(vertices[i])))
            {
                rightMesh.Vertices.Add(vertices[i]);
                rightMesh.Uvs.Add(uvs[i]);
                rightMesh.Normals.Add(normals[i]);
               
                vertexPosChange[i] = rightMesh.Vertices.Count -1;
            }
            else
            {
                leftMesh.Vertices.Add(vertices[i]);
                leftMesh.Uvs.Add(uvs[i]);
                leftMesh.Normals.Add(normals[i]);

                vertexPosChange[i] = leftMesh.Vertices.Count -1;
            }
        }
    }
   
    private int[] ApplyIndexChange(List<int> indices, int[] lookUp)
    {
        int[] indexArray = new int[indices.Count];
        for (int i = 0; i < indices.Count; i++)
        {
            indexArray[i] = lookUp[indices[i]];
        }

        return indexArray;
    }

     private void ApplyIndexChange(List<int>[] indices, int[] lookUp){

         for(int i = 0; i < indices.Length; i++){
            for(int j = 0; j < indices[0].Count; j++){
                indices[i][j] = lookUp[indices[i][j]]; 
            }
         }
     }

    private void SplitInTwo()
    {
        int above = 0;
        int below = 0;

        Vector3[] vertices = M.vertices;

        for (int submesh = 0; submesh < M.subMeshCount; submesh++)
        {
            int[] subMeshIndices = M.GetIndices(submesh);

            for (int i = 0; i < subMeshIndices.Length; i += 3)
            {
                above = 0;
                below = 0;
                
                bool a = false;
                bool b = false;
                bool c = false;

                if (cuttingPlane.GetSide(transform.TransformPoint(vertices[subMeshIndices[i]])))
                {
                    a= true;
                    below++;
                }
                else
                {
                    a = false;
                    above++;
                }

                if (cuttingPlane.GetSide(transform.TransformPoint(vertices[subMeshIndices[i+1]])))
                {
                    b = true;
                    below++;
                }
                else
                {
                    b = false;
                    above++;
                }
                if (cuttingPlane.GetSide(transform.TransformPoint(vertices[subMeshIndices[i+2]])))
                {
                    c = true;
                    below++;
                }
                else
                {
                    c = false;
                    above++;
                }

                if (above == 3 || below == 3)
                {
                    if (above == 3)
                    {
                        leftMesh.Indices[submesh].Add(subMeshIndices[i]);
                        leftMesh.Indices[submesh].Add(subMeshIndices[i + 1]);
                        leftMesh.Indices[submesh].Add(subMeshIndices[i + 2]);              
					}
                    else
                    {
                        rightMesh.Indices[submesh].Add(subMeshIndices[i]);
                        rightMesh.Indices[submesh].Add(subMeshIndices[i + 1]);
                        rightMesh.Indices[submesh].Add(subMeshIndices[i + 2]);
                    }
                }else{                   
                    if(a == c){
                        conflictTriangles.Add(new Tri(transform.TransformPoint(vertices[subMeshIndices[i + 1]]), transform.TransformPoint(vertices[subMeshIndices[i]]), transform.TransformPoint(vertices[subMeshIndices[i + 2]]),b,a,c));
                    }else if(a == b){
                        conflictTriangles.Add(new Tri(transform.TransformPoint(vertices[subMeshIndices[i + 2]]), transform.TransformPoint(vertices[subMeshIndices[i]]), transform.TransformPoint(vertices[subMeshIndices[i + 1]]),c,a,b));
                    }else if(b == c){
                        conflictTriangles.Add(new Tri(transform.TransformPoint(vertices[subMeshIndices[i]]), transform.TransformPoint(vertices[subMeshIndices[i+1]]), transform.TransformPoint(vertices[subMeshIndices[i + 2]]),a,b,c));
                    }                   
				}
            }
        }

        for(int i = 0; i < conflictTriangles.Count; i++){
            SplitTriangle(cuttingPlane, conflictTriangles[i]);
        }

        DetermineVertexPositions(vertices, M.normals, M.uv);

        if(leftMesh.Vertices.Count != 0)
        {
            ApplyIndexChange(leftMesh.Indices, vertexPosChange);
            sliceCreator.CreateSlice(transform, leftMesh);
        }

        if (rightMesh.Vertices.Count != 0)
        {
            ApplyIndexChange(rightMesh.Indices, vertexPosChange);
            sliceCreator.CreateSlice(transform, rightMesh);
        }
    }
}
