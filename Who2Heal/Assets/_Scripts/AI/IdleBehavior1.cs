using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehavior1 : ABotBehaviour {

    Vector3 targetMovePoint;
    float targetMoveTimer;

    protected void Start()
    {
        targetMovePoint = transform.position;
        targetMoveTimer = Random.Range(1f, 3f);
    }

    public override bool ShouldProcessUpdate()
    {
        return true;
    }

    public override void ProcessUpdate(UnitMover unitMover)
    {
        // wander randomly for short distances
        targetMoveTimer = Mathf.MoveTowards(targetMoveTimer, 0f, Time.deltaTime);
        if (targetMoveTimer == 0f)
        {
            // Pick new target position
            targetMovePoint = new Vector3(Random.Range(targetMovePoint.x - 8f, targetMovePoint.x + 8f), 0f, Random.Range(targetMovePoint.z - 8f, targetMovePoint.z + 8f));

            targetMoveTimer = Random.Range(1.5f, 5f);
        }

        Vector3 toTarget = targetMovePoint - transform.position;
        if (toTarget.magnitude > 1f)
        {
            unitMover.Move(toTarget.normalized);
        }
    }
}
