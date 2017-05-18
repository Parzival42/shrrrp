using UnityEngine;

[CreateAssetMenu(fileName = "SlicePhysicsProperties", menuName = "Physics/PhysicsProperties", order = 1)]
public class SlicePhysicsProperties : ScriptableObject {
	public float baseMass = 1000;
	public float drag = 0.1f;
	public float angularDrag = 0.05f;
	public RigidbodyConstraints constraints;
	public PhysicMaterial physicMaterial;

}