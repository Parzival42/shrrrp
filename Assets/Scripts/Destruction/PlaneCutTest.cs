using System.Collections.Generic;
using UnityEngine;

public class PlaneCutTest : MonoBehaviour {

    
	[SerializeField]
	private GameObject referencePlane;

	[SerializeField]
	private bool cut;

	private Plane cuttingPlane;

    private int vertexCount;

    private int[] vertexPosChange;

    MeshFilter MF;
    Mesh M;    
    Vector3[] normals;
    Vector2[] uvs;

    GameObject upper;
    GameObject lower;

    private int upperVertices;
    private List<Vector3> upperVertexList = new List<Vector3>();
    private List<Vector3> lowerVertexList = new List<Vector3>();

    private List<Vector2> lowUVs;
    private List<Vector2> upperUVs;

    private List<int>[] lowIndices;
    private List<int>[] upIndices;

	private List<Tri> conflictTriangles;

    private List<Vector3> lowNormals;
    private List<Vector3> upNormals;

    private int lowerVertices;

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
        // Debug.DrawLine(t.a, t.b);
        // Debug.DrawLine(t.b, t.c);
        // Debug.DrawLine(t.c, t.a);

    }



    void StartSplitInTwo()
    {
        if (GetComponent<MeshFilter>() != null)
        {
            MF = GetComponent<MeshFilter>();
            M = MF.mesh;
        }
        
		normals = M.normals;
        uvs = M.uv;       
        vertexCount = M.vertexCount;
       
        vertexPosChange = new int[vertexCount];

        lowUVs = new List<Vector2>();
        upperUVs = new List<Vector2>();

        lowIndices = new List<int>[M.subMeshCount];
        upIndices = new List<int>[M.subMeshCount];
		conflictTriangles = new List<Tri>();

        for (int i = 0; i < lowIndices.Length; i++)
        {
            lowIndices[i] = new List<int>();
            upIndices[i] = new List<int>();
        }

        upNormals = new List<Vector3>();
        lowNormals = new List<Vector3>();

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




    private void DetermineVertexPositions(Vector3[] vertices)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            if (cuttingPlane.GetSide(transform.TransformPoint(vertices[i])))
            {
                lowerVertices++;
                lowerVertexList.Add(vertices[i]);
                lowUVs.Add(uvs[i]);

                lowNormals.Add(normals[i]);
                vertexPosChange[i] = lowerVertexList.Count - 1;
            }
            else
            {
                upperVertices++;
                upperVertexList.Add(vertices[i]);
                upperUVs.Add(uvs[i]);

                upNormals.Add(normals[i]);
                vertexPosChange[i] = upperVertexList.Count - 1;
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
                        upIndices[submesh].Add(subMeshIndices[i]);
                        upIndices[submesh].Add(subMeshIndices[i + 1]);
                        upIndices[submesh].Add(subMeshIndices[i + 2]);
					}
                    else
                    {
                        lowIndices[submesh].Add(subMeshIndices[i]);
                        lowIndices[submesh].Add(subMeshIndices[i + 1]);
                        lowIndices[submesh].Add(subMeshIndices[i + 2]);
                    }
                }else{
                    if(a && c){
                        conflictTriangles.Add(new Tri(transform.TransformPoint(vertices[subMeshIndices[i + 1]]), transform.TransformPoint(vertices[subMeshIndices[i]]), transform.TransformPoint(vertices[subMeshIndices[i + 2]]),b,a,c));
                    }else if(a && b){
                        conflictTriangles.Add(new Tri(transform.TransformPoint(vertices[subMeshIndices[i + 2]]), transform.TransformPoint(vertices[subMeshIndices[i]]), transform.TransformPoint(vertices[subMeshIndices[i + 1]]),c,a,b));
                    }else{
                        conflictTriangles.Add(new Tri(transform.TransformPoint(vertices[subMeshIndices[i]]), transform.TransformPoint(vertices[subMeshIndices[i+1]]), transform.TransformPoint(vertices[subMeshIndices[i + 2]]),a,b,c));
                    }                   
				}
            }
        }

        for(int i = 0; i < conflictTriangles.Count; i++){
            SplitTriangle(cuttingPlane, conflictTriangles[i]);
        }
         //SplitTriangle(cuttingPlane, conflictTriangles[0]);

        DetermineVertexPositions(vertices);

        if(upperVertices != 0)
        {
            upper = new GameObject("upper");
            
            upper.transform.position = transform.position;

            upper.transform.rotation = transform.rotation;
            upper.transform.localScale = transform.localScale;

            Mesh mesh = new Mesh();
            mesh.SetVertices(upperVertexList);

            mesh.subMeshCount = upIndices.Length;
            for (int i = 0; i < upIndices.Length; i++)
            {
                mesh.SetIndices(ApplyIndexChange(upIndices[i], vertexPosChange), MeshTopology.Triangles, i);
            }

            mesh.SetNormals(upNormals);
            mesh.SetUVs(0, upperUVs);

            
			MeshRenderer renderer = upper.AddComponent<MeshRenderer>();
			MeshFilter filter = upper.AddComponent<MeshFilter>();

			filter.mesh = mesh;
			renderer.material = GetComponent<MeshRenderer>().material;
			filter.sharedMesh.RecalculateBounds();

            BoxCollider box = upper.AddComponent<BoxCollider>();
            box.center = filter.sharedMesh.bounds.center;
            box.size = filter.sharedMesh.bounds.size;

			upper.AddComponent<Rigidbody>();

        }

        if (lowerVertices != 0)
        {
            lower = new GameObject("lower");

            lower.transform.position = transform.position;

            lower.transform.rotation = transform.rotation;
            lower.transform.localScale = transform.localScale;

            Mesh meshLow = new Mesh();
            meshLow.SetVertices(lowerVertexList);
            meshLow.subMeshCount = lowIndices.Length;


            for (int i = 0; i < lowIndices.Length; i++)
            {
                meshLow.SetIndices(ApplyIndexChange(lowIndices[i], vertexPosChange), MeshTopology.Triangles, i);
            }

            meshLow.SetNormals(lowNormals);
            meshLow.SetUVs(0, lowUVs);

           	MeshRenderer renderer = lower.AddComponent<MeshRenderer>();
			MeshFilter filter = lower.AddComponent<MeshFilter>();

			filter.mesh = meshLow;
			renderer.material = GetComponent<MeshRenderer>().material;
			filter.sharedMesh.RecalculateBounds();

            BoxCollider box = lower.AddComponent<BoxCollider>();
            box.center = filter.sharedMesh.bounds.center;
            box.size = filter.sharedMesh.bounds.size;

        }
    }
}
