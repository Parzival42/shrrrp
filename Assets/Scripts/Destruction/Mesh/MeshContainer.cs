using System.Collections.Generic;
using UnityEngine;

public class MeshContainer {

	#region variables
	private List<Vector3> vertices;

	private List<int>[] indices;
	
	private List<Vector3> normals;
	
	private List<Vector2> uvs;
	#endregion

	#region properties
	public List<Vector3> Vertices{
		get{return vertices;}
	}

	public List<int>[] Indices{
		get{return indices;}
	}

	public List<Vector3> Normals{
		get{return normals;}
	}

	public List<Vector2> Uvs{
		get{return uvs;}
	}
	#endregion

	#region methods
	public MeshContainer(int subMeshCount){
		vertices = new List<Vector3>();
		indices = new List<int>[subMeshCount];
		for(int i = 0; i < indices.Length; i++){
			indices[i] = new List<int>();
		}
		normals = new List<Vector3>();
		uvs = new List<Vector2>();
	}

	public MeshContainer(Mesh mesh, bool deepCopy){
		if(deepCopy){
			DeepCopy(mesh);
		}else{
			ShallowCopy(mesh);
		}
	}

	private void DeepCopy(Mesh mesh){
		vertices = new List<Vector3>();
		vertices.AddRange(mesh.vertices);

		normals = new List<Vector3>();
		normals.AddRange(mesh.normals);

		uvs = new List<Vector2>();
		uvs.AddRange(mesh.uv);

		indices = new List<int>[mesh.subMeshCount];
		for(int i = 0; i < mesh.subMeshCount; i++){
			indices[i] = new List<int>();
			indices[i].AddRange(mesh.GetIndices(i));
		}
	}

	private void ShallowCopy(Mesh mesh){
		mesh.GetVertices(vertices);
		mesh.GetNormals(normals);
		mesh.GetUVs(0, uvs);
		
		indices = new List<int>[mesh.subMeshCount];
		for(int i = 0; i < mesh.subMeshCount; i++){
			indices[i].AddRange(mesh.GetIndices(i));
		}
	}

	public MeshContainer(): this(1){}

	public Mesh GetMesh(){
		Mesh mesh = new Mesh();
		mesh.SetVertices(vertices);
		for(int i = 0; i < indices.Length; i++){
			mesh.SetIndices(indices[i].ToArray(), MeshTopology.Triangles, i);
		}
		mesh.SetNormals(normals);
		mesh.SetUVs(0, uvs);
		mesh.RecalculateBounds();
		return mesh;
	}
	#endregion
}
