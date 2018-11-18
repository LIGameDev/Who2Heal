using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UnitMover : MonoBehaviour {

    public float moveAccel = 2.0f;
    public float moveDecel = 1.8f; 
    public float moveSpeed = 10f;

    public bool rotateToMoveDirection = true; 
    public float rotationLerpRate = 0.5f; 

    Rigidbody body;
    Quaternion moveRot;

    public Vector3 MoveVector
    {
        get;
        private set;
    }

    void Start ()
    {
        body = GetComponent<Rigidbody>();

        MoveVector = Vector3.zero;
        moveRot = Quaternion.identity; 
    }
	
	void Update ()
    {
		if (rotateToMoveDirection && MoveVector != Vector3.zero)
        {
            moveRot = Quaternion.AngleAxis(Quaternion.LookRotation(MoveVector).eulerAngles.y, Vector3.up);
        }

        // Apply deceleration
        if (MoveVector.magnitude > 0f) //inputVec.magnitude <= 0f &&
        {
            if (MoveVector.magnitude > moveDecel)
                MoveVector -= MoveVector.normalized * moveDecel;
            else
                MoveVector = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        body.angularVelocity = Vector3.zero;
        body.velocity = Vector3.zero; 

        body.MoveRotation(Quaternion.Slerp(body.rotation, moveRot, rotationLerpRate));
        body.MovePosition(body.position + MoveVector * Time.fixedDeltaTime);
    }

    public void Move(Vector3 inputVec)
    {
        // Apply acceleration 
        MoveVector += inputVec * moveAccel;

        // Limit speed
        if (MoveVector.magnitude > moveSpeed)
        {
            MoveVector = MoveVector.normalized * moveSpeed; 
        }

    }
}
