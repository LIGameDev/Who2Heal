using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class BasicFlockingBehaviour : ABotBehaviour
{
    [SerializeField]
    private float allignmentPercent = 0.05f;

    [SerializeField]
    private float cohesionPercent = 0.8f;

    [SerializeField]
    private float seperationPercent = 0.15f;

    [SerializeField]
    private float ForgetTimeInSeconds = 5f;

    protected Dictionary<int, LastSeenAlly> allyPositions = new Dictionary<int, LastSeenAlly>();

    protected abstract void UpdateAllyPositions();

    protected void Update()
    {
        UpdateAllyPositions();
    }

    protected void LateUpdate()
    {
        // forget allies we havent seen in a while
        var removeIds = allyPositions.Where(kvp => Time.timeSinceLevelLoad - kvp.Value.TimeSeen > ForgetTimeInSeconds).Select(kvp => kvp.Key).ToArray();
        foreach (int id in removeIds)
            allyPositions.Remove(id);
    }

    protected bool HasLineOfSight(Vector3 point)
    {
        RaycastHit hit;
        Vector3 startPoint = this.GetComponent<Collider>().bounds.center;
        float dist = Vector3.Distance(startPoint, point);
        Vector3 dir = (point - startPoint).normalized;
        LayerMask enviLayer = LayerMask.GetMask(new string[] { "Environment" });

        if (Physics.Raycast(startPoint, dir, out hit, dist, enviLayer))
        {
            Debug.DrawRay(startPoint, dir * hit.distance, Color.cyan);
            return false;
        }

        return true;
    }

    protected Vector3 ComputeFLockingMove(float desiredSeperation = 1f)
    {
        Vector3 allignment = ComputeAlignment(allyPositions.Values);
        Vector3 cohesion = ComputeCohesion(allyPositions.Values);
        Vector3 seperation = ComputeSeperation(allyPositions.Values, desiredSeperation);

        Vector3 resultingMove = (allignment * allignmentPercent) + (cohesion * cohesionPercent) + (seperation * seperationPercent);
        resultingMove.Normalize();
        return resultingMove;
    }

    protected Vector3 ComputeAlignment(IEnumerable<LastSeenAlly> agents)
    {
        Vector3 alignment = Vector3.zero;
        foreach (LastSeenAlly agent in agents)
        {
            alignment += agent.Velocity;
        }
        return (alignment / agents.Count()).normalized;
    }

    protected Vector3 ComputeCohesion(IEnumerable<LastSeenAlly> agents)
    {
        Vector3 centerOfMass = ComputeFlockCenter(agents);
        return (centerOfMass - this.transform.position).normalized;
    }

    protected Vector3 ComputeSeperation(IEnumerable<LastSeenAlly> agents, float desiredSeperation = 1f)
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
        float scaleFactor = Mathf.Lerp(1, 0, Mathf.Clamp01(closestGuyDist / desiredSeperation));

        return seperationVector * scaleFactor;
    }

    protected Vector3 ComputeFlockCenter(IEnumerable<LastSeenAlly> agents)
    {
        Vector3 centerOfMass = Vector3.zero;
        foreach (LastSeenAlly agent in agents)
        {
            centerOfMass += agent.Position;
        }

        centerOfMass /= agents.Count();

        return centerOfMass;
    }

    protected void DrawCross(Vector3 point, float timeSeen)
    {
        Vector3 dir1 = new Vector3(1, 0, -1);
        Vector3 dir2 = new Vector3(1, 0, 1);
        float radius = 1f;
        Color c = Color.Lerp(Color.magenta, Color.cyan, (Time.timeSinceLevelLoad - timeSeen) / ForgetTimeInSeconds);

        //Debug.DrawLine(transform.position, point, c);
        Debug.DrawLine(point - dir1 * radius, point + dir1 * radius, c);
        Debug.DrawLine(point - dir2 * radius, point + dir2 * radius, c);
    }

    protected class LastSeenAlly
    {
        public bool IsPlayer;
        public Vector3 Position;
        public Vector3 Velocity;
        public float TimeSeen;
    }
}

