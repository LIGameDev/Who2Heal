using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlockingBotBehaviour: ABotBehaviour 
{
    public DetectVolume AllyDetection;
    public float SeperationRadius;
    public float DesiredRadiusToPlayer;
    public float ForgetTimeInSeconds = 5f;

    [SerializeField]
    private float allignmentPercent = 0.05f;

    [SerializeField]
    private float cohesionPercent = 0.8f;

    [SerializeField]
    private float seperationPercent = 0.15f;


    private Dictionary<int, LastSeenAlly> allyPositions = new Dictionary<int, LastSeenAlly>();

    protected void Update()
    {
        // keep an update on who we can see
        foreach(UnitMover unit in FindUnits(UnitModel.Team.Ally))
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

    protected void LateUpdate()
    {
        // forget allies we havent seen in a while
        var removeIds = allyPositions.Where(kvp => Time.timeSinceLevelLoad - kvp.Value.TimeSeen > ForgetTimeInSeconds).Select(kvp => kvp.Key).ToArray();
        foreach (int id in removeIds)
            allyPositions.Remove(id);
    }

    // do we have a place to move to
    public override bool ShouldProcessUpdate()
    {
        return allyPositions.Count > 0;
    }

    // do the move computation
    public override void ProcessUpdate (UnitMover unitMover) 
    {
        Vector3 flockCenter = ComputeFlockCenter(allyPositions.Values);
        float distToFlockCenter = Vector3.Distance(flockCenter, this.transform.position);

        if (distToFlockCenter <= SeperationRadius) // to close spread out
        {
            Vector3 seperation = ComputeSeperation(allyPositions.Values);
            Vector3 resultingMove = seperation;
            resultingMove.Normalize();
            unitMover.Move(resultingMove);
        }
        else if (distToFlockCenter <= DesiredRadiusToPlayer) // just right hang out
        {
            unitMover.Move(Vector3.zero);
        }
        else // flock
        {
            Vector3 allignment = ComputeAlignment(allyPositions.Values);
            Vector3 cohesion = ComputeCohesion(allyPositions.Values);
            Vector3 seperation = ComputeSeperation(allyPositions.Values);

            Vector3 resultingMove = (allignment * allignmentPercent) + (cohesion * cohesionPercent) + (seperation * seperationPercent);
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
        Vector3 startPoint = this.GetComponent<Collider>().bounds.center;
        float dist = Vector3.Distance(startPoint, point);
        Vector3 dir = (point - startPoint).normalized;
        LayerMask enviLayer = LayerMask.GetMask(new string[] {"Environment"});

        if (Physics.Raycast(startPoint, dir, out hit, dist, enviLayer))
        {
            Debug.DrawRay(startPoint, dir * hit.distance, Color.cyan);
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
        Vector3 centerOfMass = ComputeFlockCenter(agents);
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

    private Vector3 ComputeFlockCenter(IEnumerable<LastSeenAlly> agents)
    {
        Vector3 centerOfMass = Vector3.zero;
        int agentCount = 0;
        foreach (LastSeenAlly agent in agents)
        {
            centerOfMass += agent.Position;

            agentCount += 1;
            if (agent.IsPlayer) // make the player prefered
                agentCount += 3;
        }

        centerOfMass /= agentCount;

        return centerOfMass;
    }

    private void DrawCross(Vector3 point, float timeSeen)
    {
        Vector3 dir1 = new Vector3(1, 0, -1);
        Vector3 dir2 = new Vector3(1, 0, 1);
        float radius = 1f;
        Color c = Color.Lerp(Color.magenta, Color.cyan, (Time.timeSinceLevelLoad - timeSeen) / ForgetTimeInSeconds);

        //Debug.DrawLine(transform.position, point, c);
        Debug.DrawLine(point - dir1 * radius, point + dir1 * radius, c);
        Debug.DrawLine(point - dir2 * radius, point + dir2 * radius, c);
    }

    private class LastSeenAlly
    {
        public bool IsPlayer;
        public Vector3 Position;
        public Vector3 Velocity;
        public float TimeSeen;
    }
}
