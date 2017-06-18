using UnityEngine;

/// <summary>
/// Freezes the child rotation.
/// </summary>
public class ChildRotationStop : MonoBehaviour
{
    private Quaternion initialRotation;

    private void Start ()
    {
        initialRotation = transform.rotation;
	}
	
	private void LateUpdate ()
    {
        transform.rotation = initialRotation;
	}
}