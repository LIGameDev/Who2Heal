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
    Vector3 moveVec;
    Quaternion moveRot;

    void Start ()
    {
        body = GetComponent<Rigidbody>();

        moveVec = Vector3.zero;
        moveRot = Quaternion.identity; 
    }
	
	void Update ()
    {
		if (rotateToMoveDirection && moveVec != Vector3.zero)
        {
            moveRot = Quaternion.AngleAxis(Quaternion.LookRotation(moveVec).eulerAngles.y, Vector3.up);
        }

        // Apply deceleration
        if (moveVec.magnitude > 0f) //inputVec.magnitude <= 0f &&
        {
            if (moveVec.magnitude > moveDecel)
                moveVec -= moveVec.normalized * moveDecel;
            else
                moveVec = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        body.angularVelocity = Vector3.zero;
        body.velocity = Vector3.zero; 

        body.MoveRotation(Quaternion.Slerp(body.rotation, moveRot, rotationLerpRate));
        body.MovePosition(body.position + moveVec * Time.fixedDeltaTime);
    }

    public void Move(Vector3 inputVec)
    {
        // Apply acceleration 
        moveVec += inputVec * moveAccel;

        // Limit speed
        if (moveVec.magnitude > moveSpeed)
        {
            moveVec = moveVec.normalized * moveSpeed; 
        }

    }
}
