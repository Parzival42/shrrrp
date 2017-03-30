using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidBodyInput : MonoBehaviour
{
    #region Public fields
    [Header("Movement Settings")]
    [SerializeField]
    private float movementSpeed = 0.1f;

    [SerializeField]
    private float rotationSpeed = 10f;
    #endregion

    #region Internal Members
    private Rigidbody rigid;
    private InputHandler inputHandler = null;
    #endregion

    #region Properties
    public Rigidbody Rigid { get { return rigid; } }
    public float MovementSpeed { get { return movementSpeed; } }
    public float RotationSpeed { get { return rotationSpeed; } }
    #endregion

    private void Start ()
    {
        rigid = GetComponent<Rigidbody>();
        inputHandler = new RigidBodyInputHandler(this);
	}
	
	private void Update ()
    {
        inputHandler.HandleInput();
	}
}