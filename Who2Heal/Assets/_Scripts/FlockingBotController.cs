using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitModel))]
[RequireComponent(typeof(UnitMover))]
public class FlockingBotController : MonoBehaviour 
{
    public GameObject corpsePrefab;

    public LayerMask WallMask;
    public float SightRadius;
    public float SeperationRadius;
    public float DesiredRadiusToPlayer;

    public Transform MinPoint;
    public Transform MaxPoint;

    private UnitModel unitModel;
    private UnitMover unitMover;

    void Start () 
    {
        unitModel = GetComponent<UnitModel>();
        unitMover = GetComponent<UnitMover>();

        unitModel.OnDeath += UnitModel_OnDeath; 
    }
	
	void Update () 
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float distToPlayer = Vector3.Distance(player.transform.position, this.transform.position);
        List<UnitMover> flockingCrew = FindUnits(UnitModel.Team.Ally);

        if (distToPlayer <= DesiredRadiusToPlayer) // we are close enough to the player just spread out
        {
            Vector3 seperation = ComputeSeperation(flockingCrew);
            Vector3 resultingMove = seperation;
            resultingMove.Normalize();
            unitMover.Move(resultingMove);
        }
        else // flock
        {
            Vector3 allignment = ComputeAlignment(flockingCrew);
            Vector3 cohesion = ComputeCohesion(flockingCrew);
            Vector3 seperation = ComputeSeperation(flockingCrew);

            //print(this.gameObject.name + ": " + seperation);
            Vector3 resultingMove = allignment + cohesion + seperation;
            resultingMove.Normalize();

           //resultingMove = AvoidWalls(resultingMove);

            unitMover.Move(resultingMove);
        }
    }

    private Vector3 AvoidWalls(Vector3 resultingMove)
    {
        // NOTE: WE ARE aproximating everything as spheres this will casue issues

        // scan ahead
        Ray forward = new Ray(this.transform.position, unitMover.MoveVector.normalized);
        RaycastHit hit;
        if (Physics.Raycast(forward, out hit, SightRadius, WallMask))
        {
            // am i left or rigbt of the obstacle (if center favor right) TODO: change this to favor where the player is
            // Pos Z means left of center

            float objRad = hit.collider.bounds.extents.magnitude;
            float myRad = this.GetComponent<Collider>().bounds.extents.magnitude;

            Vector3 rightAmount = Vector3.Project(hit.collider.bounds.center - this.transform.position, this.transform.right);

            Vector3 avoidDir;
            if(rightAmount.z > 0)
            {
                // turn left to avoid
                avoidDir = -this.transform.right;
            }
            else
            {
                // turn right to avoid
                avoidDir = this.transform.right;
            }

            Vector3 avoidSpot = hit.transform.position + (avoidDir * (objRad + myRad));
            return (avoidSpot - this.transform.position).normalized;
        }
        else
        {
            return resultingMove;
        }
    }

    private List<UnitMover> FindUnits(UnitModel.Team targetTeam)
    {
        List<UnitMover> unitMovers = new List<UnitMover>();

        // TODO: Can be made more efficent with a layer mask
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, SightRadius);

        foreach(Collider c in hitColliders)
        {
            UnitModel unit = c.gameObject.GetComponent<UnitModel>();
            if(unit != null && unit.gameObject != this.gameObject && unit.team == targetTeam)
            {
                unitMovers.Add(unit.GetComponent<UnitMover>());
            }
        }

        return unitMovers;

    }

    private Vector3 ComputeAlignment(List<UnitMover> agents)
    {
        Vector3 alignment = Vector3.zero;
        foreach (UnitMover agent in agents)
        {
            alignment += agent.MoveVector;
        }
        return (alignment / agents.Count).normalized;
    }

    private Vector3 ComputeCohesion(List<UnitMover> agents)
    {
        Vector3 centerOfMass = Vector3.zero;
        foreach (UnitMover agent in agents)
        {
            centerOfMass += agent.transform.position;
        }

        centerOfMass /= agents.Count;

        return (centerOfMass - this.transform.position).normalized;
    }

    private Vector3 ComputeSeperation(List<UnitMover> agents)
    {
        float closestGuyDist = float.MaxValue;

        Vector3 seperationVector = Vector3.zero;
        foreach (UnitMover agent in agents)
        {
            seperationVector += agent.transform.position - this.transform.position;
            float dist = Vector3.Distance(agent.transform.position, this.transform.position);
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

    private void OnEnable()
    {
        if (unitModel != null)
            unitModel.OnDeath += UnitModel_OnDeath;
    }

    private void OnDisable()
    {
        if (unitModel != null)
            unitModel.OnDeath -= UnitModel_OnDeath; 
    }

    private void UnitModel_OnDeath()
    {
        if (corpsePrefab != null)
        {
            var corpseObj = Instantiate(corpsePrefab, transform.position, transform.rotation);
            corpseObj.GetComponent<UnitCorpse>().unit = gameObject; 

            gameObject.SetActive(false); 
        }
    }
}
