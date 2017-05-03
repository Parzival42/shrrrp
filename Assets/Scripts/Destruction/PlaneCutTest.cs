using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SliceCreator))]
public class PlaneCutTest : MonoBehaviour {
    
    // mesh (should be a plane) that determines the plane orientation
	[SerializeField]
	private GameObject referencePlane;

	[SerializeField]
	private bool cut;

    // the plane object that is used for determining the vertex distribution
	private Plane cuttingPlane;

    // indices need to be adjusted when the vertices are distributed to the left and right mesh
    private int[] vertexPosChange;

    // mesh that is about to be cut
    Mesh M;    

    // triangles that intersect the plane
	private List<ConflictTriangle> conflictTriangles;

    // slice creator 
    [SerializeField]
    private SliceCreator sliceCreator;

    [SerializeField]
    private SliceCreator staticSliceCreator;


    [SerializeField]
    private FlatMeshMerger meshMerger;


    // leftover meshes 
    private MeshContainer leftMesh = new MeshContainer();

    private MeshContainer rightMesh = new MeshContainer();

    // cut triangles are stored here
    private MeshContainer splitMeshLeft = new MeshContainer();

    private MeshContainer splitMeshRight = new MeshContainer();

    private List<Vector3> polygonVertices = new List<Vector3>();

    private OutlinePreparator outlinePreparator;

    private List<Vector3> capPoints;

    private void AssignSplitTriangles(ConflictTriangle t, Vector3 d, Vector3 e, MeshContainer major, MeshContainer minor){
        minor.Vertices.Add(transform.InverseTransformPoint(t.a));
        minor.Vertices.Add(transform.InverseTransformPoint(d));
        minor.Vertices.Add(transform.InverseTransformPoint(e));
        
        major.Vertices.Add(transform.InverseTransformPoint(d));
        major.Vertices.Add(transform.InverseTransformPoint(e));
        major.Vertices.Add(transform.InverseTransformPoint(t.c));
        major.Vertices.Add(transform.InverseTransformPoint(t.b));

        if(t.negative){
            minor.Indices[0].Add(minor.Vertices.Count-1);
            minor.Indices[0].Add(minor.Vertices.Count-2);
            minor.Indices[0].Add(minor.Vertices.Count-3);

            major.Indices[0].Add(major.Vertices.Count-4);
            major.Indices[0].Add(major.Vertices.Count-3);
            major.Indices[0].Add(major.Vertices.Count-2);

            major.Indices[0].Add(major.Vertices.Count-4);
            major.Indices[0].Add(major.Vertices.Count-2);
            major.Indices[0].Add(major.Vertices.Count-1);
            
        }else{
            minor.Indices[0].Add(minor.Vertices.Count-3);
            minor.Indices[0].Add(minor.Vertices.Count-2);
            minor.Indices[0].Add(minor.Vertices.Count-1);

            major.Indices[0].Add(major.Vertices.Count-2);
            major.Indices[0].Add(major.Vertices.Count-3);
            major.Indices[0].Add(major.Vertices.Count-4);

            major.Indices[0].Add(major.Vertices.Count-1);
            major.Indices[0].Add(major.Vertices.Count-2);
            major.Indices[0].Add(major.Vertices.Count-4);
        }
    }

    private void SplitTriangle(Plane p, ConflictTriangle t, MeshContainer left, MeshContainer right){
        Ray ray1 = new Ray(t.a, t.b - t.a);
        Ray ray2 = new Ray(t.a, t.c - t.a);

        Vector3 d = new Vector3();
        Vector3 e = new Vector3();

        float rayDistance1;
        if (p.Raycast(ray1, out rayDistance1)){
            d= ray1.GetPoint(rayDistance1);          
        }else {
            return;
        }

        float rayDistance2;
        if (p.Raycast(ray2, out rayDistance2)){
            e= ray2.GetPoint(rayDistance2);           
        }else{
            return;
        }

        //outlinePreparator.AddVertexConnection(d,e);

        bool leftMajor = (t.aLeft && t.bLeft) || (t.aLeft && t.cLeft) || (t.bLeft && t.cLeft) ? true : false; 
//outlinePreparator.Add(transform.InverseTransformPoint(d),transform.InverseTransformPoint(e));
//outlinePreparator.Add(transform.InverseTransformPoint(e),transform.InverseTransformPoint(d));

        if(leftMajor){
            AssignSplitTriangles(t, d, e, left, right);
            //outlinePreparator.Add(d,e);
            if(t.negative){
                outlinePreparator.Add(transform.InverseTransformPoint(e),transform.InverseTransformPoint(d));
            }else{
                outlinePreparator.Add(transform.InverseTransformPoint(d),transform.InverseTransformPoint(e));
            }
            
            Debug.DrawLine(d,e);
        }else{
            AssignSplitTriangles(t,d,e,right,left);
            //outlinePreparator.Add(e,d);
            if(t.negative){
                outlinePreparator.Add(transform.InverseTransformPoint(d),transform.InverseTransformPoint(e));
            }else{
                outlinePreparator.Add(transform.InverseTransformPoint(e),transform.InverseTransformPoint(d));
            }

           // Debug.DrawLine(e,d);
        }
        Debug.Log("split triangle executed");
        //Debug.DrawLine(d,e);    
         
    }

    void StartSplitInTwo()
    {
        if (GetComponent<MeshFilter>() != null)
        {
            M = GetComponent<MeshFilter>().mesh;
        }
        vertexPosChange = new int[M.vertexCount];
		conflictTriangles = new List<ConflictTriangle>();

        float startTime = Time.realtimeSinceStartup;

        //adjust plane orientation
		Mesh pM = referencePlane.GetComponent<MeshFilter>().mesh;
		Transform pT = referencePlane.transform;
		cuttingPlane.Set3Points(pT.TransformPoint(pM.vertices[pM.triangles[0]]), pT.TransformPoint(pM.vertices[pM.triangles[1]]),pT.TransformPoint(pM.vertices[pM.triangles[2]]));

        //determine whether the triangle indices belong to the left or right  
        DetermineIndexPositions();       

        //determine whether the vertices belongto the left or right
        DetermineVertexPositions(M.vertices, M.normals, M.uv);

        outlinePreparator = gameObject.AddComponent<OutlinePreparator>();

        //split the plane-intersecting triangles
        for(int i = 0; i < conflictTriangles.Count; i++){
            SplitTriangle(cuttingPlane, conflictTriangles[i], splitMeshLeft, splitMeshRight);
        }

        //create correct cap polygon
        
        List<Vector3> capOutlinePolygon = outlinePreparator.PrepareOutlinePolygon();
        capPoints = capOutlinePolygon;
        //capOutlinePolygon = gameObject.AddComponent<Poly2DCreator>().GetPolygon();
        Debug.Log("polygon size: "+capOutlinePolygon.Count);
        //  if(capPoints != null && capPoints.Count >1){
        //    Debug.Log("cap vertices: "+capPoints.Count);
		// 	for(int i = 0; i < capPoints.Count-1; i++){
		// 		DebugExtension.DebugArrow(capPoints[i], capPoints[i+1]-capPoints[i], Color.black, 10.0f);
		// 	}

		// 	DebugExtension.DebugArrow(capPoints[capPoints.Count-1], capPoints[0]-capPoints[capPoints.Count-1], Color.black, 10.0f);
		// }

        TriangulatorTest triangualtor = GetComponent<TriangulatorTest>();
        MeshContainer cap = triangualtor.Triangulate(capOutlinePolygon);

        
        
        //create the slices as gameobjects and add needed components
        if (rightMesh.Vertices.Count != 0)
        {
            ApplyIndexChange(rightMesh.Indices, vertexPosChange);
            rightMesh = meshMerger.Merge(rightMesh, splitMeshLeft);
            rightMesh = meshMerger.Merge(rightMesh, cap);
            sliceCreator.CreateSlice(transform, rightMesh);
        }
        
        if(leftMesh.Vertices.Count != 0)
        {
            //index change is necessary as the vertex count is different for the two new meshes -> indices need to be adjusted
            ApplyIndexChange(leftMesh.Indices, vertexPosChange);
            leftMesh = meshMerger.Merge(leftMesh, splitMeshRight);
            Helper.FlipTriangles(cap.Indices);
            
            leftMesh = meshMerger.Merge(leftMesh, cap);
            staticSliceCreator.CreateSlice(transform, leftMesh);
        }
        
        

      


        Debug.Log("Time needed: "+ (Time.realtimeSinceStartup - startTime));

        // kill the original object
      	Destroy(this.gameObject);
    }

	void Update () {
     
		
		if(cut){
			StartSplitInTwo();	
            //GetComponent<TriangulatorTest>().Triangulate(polygonVertices);	
		}
	}

    private void AddPolygonVertex(Vector3 vertex){
        for(int i = 0; i < polygonVertices.Count; i++){
            if(Helper.VectorIsIdentical(polygonVertices[i], vertex)){
                return;
            }
        }
        polygonVertices.Add(vertex);
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

     private void ApplyIndexChange(List<int>[] indices, int[] lookUp){
         for(int i = 0; i < indices.Length; i++){
            for(int j = 0; j < indices[0].Count; j++){
                indices[i][j] = lookUp[indices[i][j]]; 
            }
         }
     }

    private void DetermineIndexPositions()
    {
        int above = 0;
        int below = 0;

        List<int> leftVertices = new List<int>();
        List<int> rightVertices = new List<int>();
        

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
                        conflictTriangles.Add(new ConflictTriangle(transform.TransformPoint(vertices[subMeshIndices[i + 1]]), transform.TransformPoint(vertices[subMeshIndices[i]]), transform.TransformPoint(vertices[subMeshIndices[i+2]]),b,a,c, true));
                    }else if(a == b){
                        conflictTriangles.Add(new ConflictTriangle(transform.TransformPoint(vertices[subMeshIndices[i + 2]]), transform.TransformPoint(vertices[subMeshIndices[i]]), transform.TransformPoint(vertices[subMeshIndices[i + 1]]),c,a,b, false));
                    }else if(b == c){
                        conflictTriangles.Add(new ConflictTriangle(transform.TransformPoint(vertices[subMeshIndices[i]]), transform.TransformPoint(vertices[subMeshIndices[i+1]]), transform.TransformPoint(vertices[subMeshIndices[i + 2]]),a,b,c, false));
                    }                   
				}
            }
        }
    }

}