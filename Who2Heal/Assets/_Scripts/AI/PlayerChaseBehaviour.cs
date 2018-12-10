using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerChaseBehaviour : BasicFlockingBehaviour 
{
    public DetectVolume AllyDetection;
    public float DesiredRadiusToPlayer;

    protected override void UpdateAllyPositions()
    {
        UnitMover player = AllyDetection.DetectedObjects.Where(go => go.tag.Equals("Player")).Select(go => go.GetComponent<UnitMover>()).FirstOrDefault();
        if(player != null)
        {
            Vector3 targetPoint = player.GetComponent<Collider>().bounds.center;
            if (HasLineOfSight(targetPoint))
            {
                LastSeenAlly seenEvent = new LastSeenAlly()
                {
                    Position = player.transform.position,
                    Velocity = player.MoveVector,
                    TimeSeen = Time.timeSinceLevelLoad,
                    IsPlayer = true
                };

                allyPositions[player.gameObject.GetInstanceID()] = seenEvent;
            }
        }
    }

    // do we have a place to move to
    public override bool ShouldProcessUpdate()
    {
        // we have the player to move to and we are not close enough
        return allyPositions.Count > 0 && Vector3.Distance(allyPositions.First().Value.Position, this.transform.position) > DesiredRadiusToPlayer;
    }

    // do the move computation
    public override void ProcessUpdate(UnitMover unitMover)
    {
        float distToPlayer = Vector3.Distance(allyPositions.First().Value.Position, this.transform.position);

        if (distToPlayer <= DesiredRadiusToPlayer) // just right hang out
        {
            unitMover.Move(Vector3.zero);
        }
        else // flock
        {
            Vector3 resultingMove = ComputeFLockingMove();
            unitMover.Move(resultingMove);
        }
    }

}
