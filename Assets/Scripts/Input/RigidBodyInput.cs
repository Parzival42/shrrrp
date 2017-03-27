using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidBodyInput : MonoBehaviour
{
    #region Public fields
    [SerializeField]
    private float movementSpeed = 2f;
    #endregion

    #region Internal Members
    private Rigidbody rigid;
    private InputHandler inputHandler = null;
    #endregion

    #region Properties
    public Rigidbody Rigid { get { return rigid; } }
    public float MovementSpeed { get { return movementSpeed; } }
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

    private void FixedUpdate()
    {
    }
}