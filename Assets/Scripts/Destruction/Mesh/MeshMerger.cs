using UnityEngine;

public interface MeshMerger{
	MeshContainer Merge(MeshContainer recipient, MeshContainer mergee);
}
