using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour 
{
    public Vector3 TargetRotation;

	private void LateUpdate()
	{
        this.transform.rotation = Quaternion.Euler(TargetRotation);
	}
}
