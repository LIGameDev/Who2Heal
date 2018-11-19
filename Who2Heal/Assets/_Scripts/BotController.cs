using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitModel))]
[RequireComponent(typeof(UnitMover))]
public class BotController : UnitController {
    
    Vector3 targetMovePoint;
    float targetMoveTimer;

    protected override void Start () {
        base.Start(); 

        targetMovePoint = transform.position; 
        targetMoveTimer = Random.Range(1f, 3f);
    }
	
	protected override void Update () {
        base.Update(); 

        // Temp behavior: wander near origin
        targetMoveTimer = Mathf.MoveTowards(targetMoveTimer, 0f, Time.deltaTime); 
        if (targetMoveTimer == 0f)
        {
            // Pick new target position
            targetMovePoint = new Vector3(Random.Range(-8f, 8f), 0f, Random.Range(-8f, 8f)); 

            targetMoveTimer = Random.Range(1.5f, 5f); 
        }

        Vector3 toTarget = targetMovePoint - transform.position;
        if (toTarget.magnitude > 1f)
        {
            unitMover.Move(toTarget.normalized);
        }
    }
    
}
