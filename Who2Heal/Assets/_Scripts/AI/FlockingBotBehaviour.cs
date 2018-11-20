using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlockingBotBehaviour: ABotBehaviour 
{
    public DetectVolume AllyDetection;
    public float SeperationRadius;
    public float DesiredRadiusToPlayer;
    public float ForgetTimeInSeconds = 5f;

    private Dictionary<int, LastSeenAlly> allyPositions = new Dictionary<int, LastSeenAlly>();

    protected void Update()
    {
        foreach(UnitMover unit in FindUnits(UnitModel.Team.Ally))
        {
            if (HasLineOfSight(unit.transform.position))
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

    protected void LateUpdate()
    {
        // forget allies we havent seen in a while
        var removeIds = allyPositions.Where(kvp => Time.timeSinceLevelLoad - kvp.Value.TimeSeen > ForgetTimeInSeconds).Select(kvp => kvp.Key);
        foreach (int id in removeIds)
            allyPositions.Remove(id);
    }

    public override bool ShouldProcessUpdate()
    {
        return allyPositions.Count > 0;
    }

    public override void ProcessUpdate (UnitMover unitMover) 
    {
        LastSeenAlly playerSeenEvent = allyPositions.Values.Where(ally => ally.IsPlayer).FirstOrDefault();
        float distToPlayer = -1;
        if(playerSeenEvent != null)
          distToPlayer = Vector3.Distance(playerSeenEvent.Position, this.transform.position);

        if (distToPlayer <= DesiredRadiusToPlayer) // we are close enough to the player just spread out
        {
            Vector3 seperation = ComputeSeperation(allyPositions.Values);
            Vector3 resultingMove = seperation;
            resultingMove.Normalize();
            unitMover.Move(resultingMove);
        }
        else // flock
        {
            Vector3 allignment = ComputeAlignment(allyPositions.Values);
            Vector3 cohesion = ComputeCohesion(allyPositions.Values);
            Vector3 seperation = ComputeSeperation(allyPositions.Values);

            Vector3 resultingMove = allignment + cohesion + seperation;
            resultingMove.Normalize();

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

    private bool HasLineOfSight(Vector3 point)
    {
        RaycastHit hit;
        float dist = Vector3.Distance(transform.position, point);
        Vector3 dir = (point - transform.position).normalized;
        LayerMask enviLayer = LayerMask.GetMask(new string[] {"Environment"});

        if (Physics.Raycast(transform.position, dir, out hit, dist, enviLayer))
        {
            Debug.DrawRay(transform.position, dir * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
            return false;
        }

        return true;
    }

    private Vector3 ComputeAlignment(IEnumerable<LastSeenAlly> agents)
    {
        Vector3 alignment = Vector3.zero;
        foreach (LastSeenAlly agent in agents)
        {
            alignment += agent.Velocity;
        }
        return (alignment / agents.Count()).normalized;
    }

    private Vector3 ComputeCohesion(IEnumerable<LastSeenAlly> agents)
    {
        Vector3 centerOfMass = Vector3.zero;
        foreach (LastSeenAlly agent in agents)
        {
            centerOfMass += agent.Position;
        }

        centerOfMass /= agents.Count();

        return (centerOfMass - this.transform.position).normalized;
    }

    private Vector3 ComputeSeperation(IEnumerable<LastSeenAlly> agents)
    {
        float closestGuyDist = float.MaxValue;

        Vector3 seperationVector = Vector3.zero;
        foreach (LastSeenAlly agent in agents)
        {
            seperationVector += agent.Position - this.transform.position;
            float dist = Vector3.Distance(agent.Position, this.transform.position);
            if (dist < closestGuyDist)
                closestGuyDist = dist;
        }

        seperationVector *= -1;
        seperationVector.Normalize();

        // scale the seperation so that it is strongest when close to people
        // waek when close to the range edge
        float scaleFactor = Mathf.Lerp(1, 0, Mathf.Clamp01(closestGuyDist / SeperationRadius));

        return seperationVector * scaleFactor;
    }

    private void DrawCross(Vector3 point, float timeSeen)
    {
        Vector3 dir1 = new Vector3(1, 0, -1);
        Vector3 dir2 = new Vector3(1, 0, 1);
        float radius = 1f;
        Color c = Color.Lerp(Color.magenta, Color.cyan, (Time.timeSinceLevelLoad - timeSeen) / ForgetTimeInSeconds);

        Debug.DrawLine(transform.position, point, c);
        //Debug.DrawLine(point - dir1 * radius, point + dir1 * radius, c);
        //Debug.DrawLine(point - dir2 * radius, point + dir2 * radius, c);
    }

    private class LastSeenAlly
    {
        public bool IsPlayer;
        public Vector3 Position;
        public Vector3 Velocity;
        public float TimeSeen;
    }
}
