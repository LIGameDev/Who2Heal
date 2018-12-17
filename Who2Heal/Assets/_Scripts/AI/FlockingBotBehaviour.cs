using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlockingBotBehaviour : BasicFlockingBehaviour 
{
    public DetectVolume AllyDetection;
    public float SeperationRadius;
    public UnitModel.Team DesiredTeam;

    protected override void UpdateAllyPositions()
    {
        // keep an update on who we can see
        foreach (UnitMover unit in FindUnits(DesiredTeam))
        {
            Vector3 targetPoint = unit.GetComponent<Collider>().bounds.center;
            if (HasLineOfSight(targetPoint))
            {
                LastSeenAlly seenEvent = new LastSeenAlly()
                {
                    Position = unit.transform.position,
                    Velocity = unit.MoveVector,
                    TimeSeen = Time.timeSinceLevelLoad,
                    IsPlayer = unit.gameObject.tag.Equals("Player")
                };

                allyPositions[unit.gameObject.GetInstanceID()] = seenEvent;
            }
        }

        foreach (LastSeenAlly lastSeen in allyPositions.Values)
            DrawCross(lastSeen.Position, lastSeen.TimeSeen);
    }

    // do we have a place to move to
    public override bool ShouldProcessUpdate()
    {
        return allyPositions.Count > 0;
    }

    // do the move computation
    public override void ProcessUpdate(UnitMover unitMover)
    {
        Vector3 flockCenter = ComputeFlockCenter(allyPositions.Values);
        float distToFlockCenter = Vector3.Distance(flockCenter, this.transform.position);

        if (distToFlockCenter <= SeperationRadius) // just right hang out
        {
            unitMover.Move(Vector3.zero);
        }
        else // flock
        {
            Vector3 resultingMove = ComputeFLockingMove(SeperationRadius);
            unitMover.Move(resultingMove);
        }
    }

    private List<UnitMover> FindUnits(UnitModel.Team targetTeam)
    {
        List<UnitMover> unitMovers = new List<UnitMover>();

        foreach(UnitModel unit in AllyDetection.DetectedObjects.Select(go => go.GetComponent<UnitModel>()))
        {
            if(unit != null && unit.gameObject != this.gameObject && unit.team == targetTeam)
            {
                unitMovers.Add(unit.GetComponent<UnitMover>());
            }
        }

        return unitMovers;

    }
}
