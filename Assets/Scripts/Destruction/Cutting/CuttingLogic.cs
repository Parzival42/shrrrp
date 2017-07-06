using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using CielaSpike;
using Random = UnityEngine.Random;

// ReSharper disable SuggestVarOrType_Elsewhere

[RequireComponent(typeof(SliceCreator))]
public class CuttingLogic : MonoBehaviour
{

    // indices need to be adjusted when the vertices are distributed to the left and right mesh
    private int[] vertexPosChange;

    // triangles that intersect the plane
    private List<ConflictTriangle> conflictTriangles;

    private SliceCreator sliceCreator;

    private MeshMerger meshMerger;

    // leftover meshes 
    private MeshContainer leftMesh = new MeshContainer();

    private MeshContainer rightMesh = new MeshContainer();

    private OutlinePreparator outlinePreparator;

    private List<MeshContainer> capMeshes;

    private Matrix4x4 worldToLocal;
    private Matrix4x4 localToWorld;

    private List<List<Vector3>> capMeshesOutlinePolygonCopy;

    private MeshContainer leftSimplifiedColliderMesh;
    private MeshContainer rightSimplifiedColliderMesh;

    private readonly Dictionary<Vector3, int> leftCutPointIndices = new Dictionary<Vector3, int>();

    private readonly Dictionary<Vector3, int> rightCutPointIndices = new Dictionary<Vector3, int>();

    private Vector3 cuttingPlaneNormal;

    private int getCutPointIndex(Matrix4x4 worldToLocalMatrix, Vector3 vertex, MeshContainer mesh, Dictionary<Vector3, int> cutPointIndices)
    {
        if(!cutPointIndices.ContainsKey(vertex)){
            mesh.Vertices.Add(worldToLocalMatrix.MultiplyPoint3x4(vertex));
            cutPointIndices.Add(vertex, mesh.Vertices.Count-1);
            mesh.Normals.Add(new Vector3());
        }

        return cutPointIndices[vertex];
    }
    
    private void AssignSplitTriangles(Matrix4x4 worldToLocalMatrix, ConflictTriangle t, Vector3 d, Vector3 e, MeshContainer major, MeshContainer minor, Dictionary<Vector3,int> majorCutPointIndices, Dictionary<Vector3, int> minorCutPointIndices)
    {
        if (t.negative)
        {
            minor.Indices[t.subMeshIndex].Add(getCutPointIndex(worldToLocalMatrix, e, minor, minorCutPointIndices));
            minor.Indices[t.subMeshIndex].Add(getCutPointIndex(worldToLocalMatrix, d, minor, minorCutPointIndices));
            minor.Indices[t.subMeshIndex].Add(t.indexA);

            major.Indices[t.subMeshIndex].Add(getCutPointIndex(worldToLocalMatrix, d, major, majorCutPointIndices));
            major.Indices[t.subMeshIndex].Add(getCutPointIndex(worldToLocalMatrix, e, major, majorCutPointIndices));
            major.Indices[t.subMeshIndex].Add(t.indexC);

            major.Indices[t.subMeshIndex].Add(getCutPointIndex(worldToLocalMatrix, d, major, majorCutPointIndices));
            major.Indices[t.subMeshIndex].Add(t.indexC);
            major.Indices[t.subMeshIndex].Add(t.indexB);
        }
        else
        {
            minor.Indices[t.subMeshIndex].Add(t.indexA);
            minor.Indices[t.subMeshIndex].Add(getCutPointIndex(worldToLocalMatrix, d, minor, minorCutPointIndices));
            minor.Indices[t.subMeshIndex].Add(getCutPointIndex(worldToLocalMatrix, e, minor, minorCutPointIndices));

            major.Indices[t.subMeshIndex].Add(t.indexC);
            major.Indices[t.subMeshIndex].Add(getCutPointIndex(worldToLocalMatrix, e, major, majorCutPointIndices));
            major.Indices[t.subMeshIndex].Add(getCutPointIndex(worldToLocalMatrix, d, major, majorCutPointIndices));

            major.Indices[t.subMeshIndex].Add(t.indexB);
            major.Indices[t.subMeshIndex].Add(t.indexC);
            major.Indices[t.subMeshIndex].Add(getCutPointIndex(worldToLocalMatrix, d, major, majorCutPointIndices));
        }
    }

    private void SplitTriangle(Plane p, ConflictTriangle t, MeshContainer left, MeshContainer right, Dictionary<Vector3,int> leftCutPointIndices, Dictionary<Vector3,int> rightCutPointIndices)
    {
        Ray ray1 = new Ray(t.a, t.b - t.a);
        Ray ray2 = new Ray(t.a, t.c - t.a);

        Vector3 d;
        Vector3 e;

        float rayDistance1;
        if (p.Raycast(ray1, out rayDistance1))
        {
            d = ray1.GetPoint(rayDistance1);
        }
        else
        {
            return;
        }

        float rayDistance2;
        if (p.Raycast(ray2, out rayDistance2))
        {
            e = ray2.GetPoint(rayDistance2);
        }
        else
        {
            return;
        }

        bool leftMajor = (t.aLeft && t.bLeft) || (t.aLeft && t.cLeft) || (t.bLeft && t.cLeft);

        if (leftMajor)
        {
            AssignSplitTriangles(worldToLocal, t, d, e, right, left, rightCutPointIndices, leftCutPointIndices);

            if (t.negative)
            {
                outlinePreparator.Add(d, e);
            }
            else
            {
                outlinePreparator.Add(e, d);
            }
        }
        else
        {
            AssignSplitTriangles(worldToLocal, t, d, e, left, right, leftCutPointIndices, rightCutPointIndices);

            if (t.negative)
            {
                outlinePreparator.Add(e, d);
            }
            else
            {
                outlinePreparator.Add(d, e);
            }
        }
    }

    public IEnumerator CuttingCoroutine(Plane cuttingPlane, MeshContainer mesh, SlicePhysicsProperties slicePhysicsProperties, bool suddenDeath, float delay)
    {
        yield return new WaitForSeconds(delay);

        yield return Ninja.JumpToUnity;

        localToWorld = transform.localToWorldMatrix;
        worldToLocal = transform.worldToLocalMatrix;

        sliceCreator = GetComponent<SliceCreator>();
        meshMerger = GetComponent<MeshMerger>();

        if (sliceCreator == null || meshMerger == null)
        {
            Debug.LogError("slice creator or mesh merger is null");
            yield break;
        }

        cuttingPlaneNormal = cuttingPlane.normal;
        yield return Ninja.JumpBack;

        StartSplitInTwo(cuttingPlane, mesh, slicePhysicsProperties);

        yield return Ninja.JumpToUnity;

        yield return new WaitForFixedUpdate();

        CreateStuff(cuttingPlane, slicePhysicsProperties, suddenDeath);
    }


    public void CreateStuff(Plane cuttingPlane, SlicePhysicsProperties slicePhysicsProperties, bool suddenDeath)
    {
        bool rightDissolve = false;
        bool leftDissolve = false;

        if (suddenDeath)
        {
            if (Random.Range(0.0f, 1.0f) < 0.5f)
            {
                rightDissolve = true;
            }
            else
            {
                leftDissolve = true;
            }
        }
        
        if (rightMesh.Vertices.Count != 0)
        {
            sliceCreator.CreateSlice(transform, rightMesh, rightSimplifiedColliderMesh.GetMesh(), cuttingPlaneNormal, slicePhysicsProperties, rightDissolve);
        }

        if (leftMesh.Vertices.Count != 0)
        {
            sliceCreator.CreateSlice(transform, leftMesh, leftSimplifiedColliderMesh.GetMesh(), -cuttingPlaneNormal, slicePhysicsProperties, leftDissolve);
        }

        // kill the original object or not
        if (transform.parent != null && !transform.parent.CompareTag("IndestructibleParent"))
        {
            Destroy(transform.parent.gameObject);
        }
        Destroy(this.gameObject);
    }


    public void StartSplitInTwo(Plane cuttingPlane, MeshContainer mesh, SlicePhysicsProperties slicePhysicsProperties)
    {
        vertexPosChange = new int[mesh.Vertices.Count];
        conflictTriangles = new List<ConflictTriangle>();

        //determine whether the vertices belong to the left or right
        DetermineVertexPositions(localToWorld, mesh.Vertices, mesh.Normals, mesh.Uvs, cuttingPlane);

        //determine whether the triangle indices belong to the left or right  
        DetermineIndexPositions(localToWorld, mesh, cuttingPlane);

        //if the cut does not affect the mesh we are done
        if (leftMesh.Vertices.Count == 0 || rightMesh.Vertices.Count == 0)
        {
            return;
        }

        //--- splitting conflict triangles and feed the big mean poly machine
        outlinePreparator = new OutlinePreparator();

        //split the plane-intersecting triangles
        for (int i = 0; i < conflictTriangles.Count; i++)
        {
            SplitTriangle(cuttingPlane, conflictTriangles[i], leftMesh, rightMesh, leftCutPointIndices,
                rightCutPointIndices);
        }

        //create ordered capMeshes polygon
        List<List<Vector3>> capMeshesOutlinePolygon = outlinePreparator.PrepareOutlinePolygons();
        capMeshesOutlinePolygonCopy = new List<List<Vector3>>();
        for (int i = 0; i < capMeshesOutlinePolygon.Count; i++)
        {   
            capMeshesOutlinePolygonCopy.Add(new List<Vector3>());
            for(int j = 0; j <capMeshesOutlinePolygonCopy[i].Count; j++){
                capMeshesOutlinePolygonCopy[i].Add(new Vector3(capMeshesOutlinePolygon[i][j].x, capMeshesOutlinePolygon[i][j].y, capMeshesOutlinePolygon[i][j].z));
            }
            //Debug.Log("capMeshes polygon " + i + " vertexcount: " + capMeshesOutlinePolygon[capMeshesOutlinePolygon.Count - 1].Count);
        }
        //--- end


        //--- approximate vertex projection (check which plane the polygon vertices should be projected on)
        //float minDistance = Vector3.SqrMagnitude(Vector3.ProjectOnPlane(cuttingPlane.normal, Vector3.forward));
        int projectCoordA = 0;
        int projectCoordB = 1;

        // float dist = Vector3.SqrMagnitude(Vector3.ProjectOnPlane(cuttingPlane.normal, Vector3.up));
        // if (dist < minDistance)
        // {
        //     minDistance = dist;
        //     projectCoordA = 0;
        //     projectCoordB = 2;
        // }

        // dist = Vector3.SqrMagnitude(Vector3.ProjectOnPlane(cuttingPlane.normal, Vector3.right));
        // if (dist < minDistance)
        // {
        //     minDistance = dist;
        //     projectCoordA = 1;
        //     projectCoordB = 2;
        // }

        //if(!Helper.IsPolygonClockwise(capMeshesOutlinePolygon, projectCoordA, projectCoordB)){
        //   capMeshesOutlinePolygon.Reverse();
        //   Debug.Log("polygon is counter-clockwise, reversed the order");
        //}
        //--- end


        //--- exact vertex projection on xy plane
        Vector3 rotationAxis = Vector3.Cross(Vector3.forward, cuttingPlane.normal).normalized;
        //DebugExtension.DebugArrow(Vector3.zero, rotationAxis, Color.red, 20.0f);

        float rotationAngle = Mathf.Acos(Vector3.Dot(Vector3.forward, cuttingPlane.normal));
        //Debug.Log("rotation angle is: "+(rotationAngle*Mathf.Rad2Deg));

         Quaternion q = Quaternion.AngleAxis(-rotationAngle*Mathf.Rad2Deg, rotationAxis);
         Quaternion qReverse = Quaternion.AngleAxis(rotationAngle*Mathf.Rad2Deg, rotationAxis);

         for(int i = 0; i < capMeshesOutlinePolygon.Count; i++){
             for(int j = 0; j<capMeshesOutlinePolygon[i].Count; j++){
                 capMeshesOutlinePolygon[i][j] = q*capMeshesOutlinePolygon[i][j];
            }
        }
        //--- end


        //--- triangulation
        capMeshes = new List<MeshContainer>();
        for (int i = 0; i < capMeshesOutlinePolygon.Count; i++)
        {
            capMeshes.Add(Triangulator.Triangulate(capMeshesOutlinePolygon[i], projectCoordA, projectCoordB));

        }
        //--- end

        //--- exact vertex projection
        for(int i = 0; i < capMeshes.Count; i++){
            for(int j = 0; j <capMeshes[i].Vertices.Count; j++){
                capMeshes[i].Vertices[j] = qReverse * capMeshes[i].Vertices[j];
            }
        }
        //--- end

        
        for (int i = 0; i < capMeshes.Count; i++)
        {
            Helper.UnProjectVertices(worldToLocal, capMeshes[i]);
        }

        if (rightMesh.Vertices.Count != 0)
        {
            for (int i = 0; i < capMeshes.Count; i++)
            {
                Helper.FlipTriangles(capMeshes[i].Indices);
                rightMesh = meshMerger.Merge(rightMesh, capMeshes[i]);
            }
            
            rightSimplifiedColliderMesh = Helper.generateSimplifiedMesh(rightMesh, 64);
        }

        if (leftMesh.Vertices.Count != 0)
        {
            for (int i = 0; i < capMeshes.Count; i++)
            {
                Helper.FlipTriangles(capMeshes[i].Indices);
                leftMesh = meshMerger.Merge(leftMesh, capMeshes[i]);
            }

            leftSimplifiedColliderMesh = Helper.generateSimplifiedMesh(leftMesh, 64);
        }
    }

    private void DetermineVertexPositions(Matrix4x4 localToWorldMatrix, List<Vector3> vertices, List<Vector3> normals, List<Vector2> uvs, Plane cuttingPlane)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            if (cuttingPlane.GetSide(localToWorldMatrix.MultiplyPoint3x4(vertices[i])))
            {
                rightMesh.Vertices.Add(vertices[i]);
                rightMesh.Normals.Add(normals[i]);

                vertexPosChange[i] = rightMesh.Vertices.Count - 1;
            }
            else
            {
                leftMesh.Vertices.Add(vertices[i]);
                leftMesh.Normals.Add(normals[i]);

                vertexPosChange[i] = leftMesh.Vertices.Count - 1;
            }
        }
    }

    private void DetermineIndexPositions(Matrix4x4 localToWorldMatrix, MeshContainer mesh, Plane cuttingPlane)
    {
        List<Vector3> vertices = mesh.Vertices;

        for (int submesh = 0; submesh < mesh.Indices.Length; submesh++)
        {
            List<int> subMeshIndices = mesh.Indices[submesh];

            for (int i = 0; i < subMeshIndices.Count; i += 3)
            {
                var left = 0;
                var right = 0;

                bool a = false;
                bool b = false;
                bool c = false;

                if (cuttingPlane.GetSide(localToWorldMatrix.MultiplyPoint3x4(vertices[subMeshIndices[i]])))
                {
                    a = true;
                    right++;
                }
                else
                {
                    left++;
                }

                if (cuttingPlane.GetSide(localToWorldMatrix.MultiplyPoint3x4(vertices[subMeshIndices[i + 1]])))
                {
                    b = true;
                    right++;
                }
                else
                {
                    left++;
                }
                if (cuttingPlane.GetSide(localToWorldMatrix.MultiplyPoint3x4(vertices[subMeshIndices[i + 2]])))
                {
                    c = true;
                    right++;
                }
                else
                {
                    left++;
                }

                if (left == 3)
                {
                    leftMesh.Indices[submesh].Add(vertexPosChange[subMeshIndices[i]]);
                    leftMesh.Indices[submesh].Add(vertexPosChange[subMeshIndices[i + 1]]);
                    leftMesh.Indices[submesh].Add(vertexPosChange[subMeshIndices[i + 2]]);
                }
                else if(right == 3)
                {
                    rightMesh.Indices[submesh].Add(vertexPosChange[subMeshIndices[i]]);
                    rightMesh.Indices[submesh].Add(vertexPosChange[subMeshIndices[i + 1]]);
                    rightMesh.Indices[submesh].Add(vertexPosChange[subMeshIndices[i + 2]]);
                }
                else
                {
                    if (a == c)
                    {
                        conflictTriangles.Add(new ConflictTriangle(localToWorldMatrix.MultiplyPoint3x4(vertices[subMeshIndices[i + 1]]), localToWorldMatrix.MultiplyPoint3x4(vertices[subMeshIndices[i]]), localToWorldMatrix.MultiplyPoint3x4(vertices[subMeshIndices[i + 2]]),
                           submesh, vertexPosChange[subMeshIndices[i + 1]], vertexPosChange[subMeshIndices[i]], vertexPosChange[subMeshIndices[i + 2]], b, a, c, true));
                    }
                    else if (a == b)
                    {
                        conflictTriangles.Add(new ConflictTriangle(localToWorldMatrix.MultiplyPoint3x4(vertices[subMeshIndices[i + 2]]), localToWorldMatrix.MultiplyPoint3x4(vertices[subMeshIndices[i]]), localToWorldMatrix.MultiplyPoint3x4(vertices[subMeshIndices[i + 1]]),
                           submesh, vertexPosChange[subMeshIndices[i + 2]], vertexPosChange[subMeshIndices[i]], vertexPosChange[subMeshIndices[i + 1]], c, a, b, false));
                    }
                    else
                    {
                        conflictTriangles.Add(new ConflictTriangle(localToWorldMatrix.MultiplyPoint3x4(vertices[subMeshIndices[i]]), localToWorldMatrix.MultiplyPoint3x4(vertices[subMeshIndices[i + 1]]), localToWorldMatrix.MultiplyPoint3x4(vertices[subMeshIndices[i + 2]]),
                            submesh, vertexPosChange[subMeshIndices[i]], vertexPosChange[subMeshIndices[i+1]], vertexPosChange[subMeshIndices[i+2]], a, b, c, false));
                    }
                }
            }
        }
    }
}