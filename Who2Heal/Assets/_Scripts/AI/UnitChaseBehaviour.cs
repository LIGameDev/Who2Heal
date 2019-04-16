using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitChaseBehaviour : BasicFlockingBehaviour 
{
    public DetectVolume TargetDetection;
    public float DesiredRadiusToTarget;
    public List<string> DesiredTags;

    private LastSeenAlly beelineTarget;

    // should rename to something other than Ally?
    protected override void UpdateAllyPositions()
    {
        IEnumerable<UnitMover> targetObjs = TargetDetection.DetectedObjects.Where(go => go != null && DesiredTags.Contains(go.tag)).Select(go => go.GetComponent<UnitMover>());
        if (targetObjs != null)
        {
            foreach (UnitMover targetObj in targetObjs) {
            Vector3 targetPoint = targetObj.GetComponent<Collider>().bounds.center;
                if (HasLineOfSight(targetPoint))
                {
                    LastSeenAlly seenEvent = new LastSeenAlly()
                    {
                        Position = targetObj.transform.position,
                        Velocity = targetObj.MoveVector,
                        TimeSeen = Time.timeSinceLevelLoad,
                        IsPlayer = targetObj.tag.Equals("Player")
                    };

                    allyPositions[targetObj.gameObject.GetInstanceID()] = seenEvent;
                }
            }
        }
    }

    // do we have a place to move to
    public override bool ShouldProcessUpdate()
    {
        if (allyPositions != null && allyPositions.Count > 0)
        {
            float minDist = float.MaxValue;
            foreach (LastSeenAlly lastSeen in allyPositions.Values)
            {
                float testDist = Vector3.Distance(lastSeen.Position, this.transform.position);
                if (testDist < minDist)
                {
                    minDist = testDist;
                    beelineTarget = lastSeen;
                    if (minDist < DesiredRadiusToTarget)
                    {
                        // already close enough
                        return false;
                    }
                }
            }
            return true;
        }
        return false;
    }

    // do the move computation
    public override void ProcessUpdate(UnitMover unitMover)
    {
        // make a beeline for the target
        Vector3 dir = beelineTarget.Position - this.transform.position;
        dir.Normalize();
        unitMover.Move(dir);
    }

}
