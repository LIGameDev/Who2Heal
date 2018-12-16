using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitChaseBehaviour : BasicFlockingBehaviour 
{
    public DetectVolume AllyDetection;
    public float DesiredRadiusToPlayer;
    public List<string> targetTags;

    protected override void UpdateAllyPositions()
    {
        IEnumerable<UnitMover> targetObjs = AllyDetection.DetectedObjects.Where(go => targetTags.Contains(go.tag)).Select(go => go.GetComponent<UnitMover>());
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
        if (allyPositions.Count > 0)
        {
            foreach (LastSeenAlly allyPos in allyPositions.Values)
            {
                if (Vector3.Distance(allyPos.Position, this.transform.position) > DesiredRadiusToPlayer)
                {
                    // we have the unit to move to and we are not close enough
                    return true;
                }
            }
            return false;
        } else
        {
            return false;
        }
    }

    // do the move computation
    public override void ProcessUpdate(UnitMover unitMover)
    {
        Vector3 resultingMove = ComputeFLockingMove();
        unitMover.Move(resultingMove);
    }

}
