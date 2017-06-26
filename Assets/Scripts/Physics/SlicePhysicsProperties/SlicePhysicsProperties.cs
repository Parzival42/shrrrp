using UnityEngine;

[CreateAssetMenu(fileName = "SlicePhysicsProperties", menuName = "Physics/PhysicsProperties", order = 1)]
public class SlicePhysicsProperties : ScriptableObject {
	public float referenceVolume = 100;
	public float baseMass = 50;
	public float drag = 0.1f;
	public float angularDrag = 0.05f;
	public PhysicMaterial physicMaterial;
	public float[] volumeThresholds = new float[1];
	public float[] massThresholds = new float[1];
	public bool useThresholds = false;
	public float cuttingForce = 10.0f;
	public float minVolume = 0.1f;
}