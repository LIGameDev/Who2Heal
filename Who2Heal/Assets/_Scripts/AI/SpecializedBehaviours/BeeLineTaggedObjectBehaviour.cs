using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BeeLineTaggedObjectBehaviour : ABotBehaviour
{
    [SerializeField]
    private DetectVolume DetectVolume;

    [SerializeField]
    private string DesiredTag;

    private GameObject beelineTarget = null;

    private void Update()
    {
        // look for my target obj    
        if (beelineTarget == null)
        {
            beelineTarget = DetectVolume.DetectedObjects.Where(go => go != null && go.tag.Equals(DesiredTag) && HasLineOfSight(go.GetComponent<Collider>()))
                                                        .OrderBy(go => Vector3.Distance(this.transform.position, go.transform.position))
                                                        .FirstOrDefault();
        }
        else
        {
            // did we loose sight of our target
            if (!HasLineOfSight(beelineTarget.GetComponent<Collider>()))
                beelineTarget = null;
        }
    }

    public override bool ShouldProcessUpdate()
    {
        return beelineTarget != null;
    }

    public override void ProcessUpdate(UnitMover unitMover)
    {
        Vector3 dir = beelineTarget.transform.position - this.transform.position;
        dir.Normalize();
        unitMover.Move(dir);
    }

    private bool HasLineOfSight(Collider targetCollider)
    {
        if (targetCollider == null)
            return false;

        Vector3 startPoint = this.GetComponent<Collider>().bounds.center;
        Vector3 targetPoint = targetCollider.bounds.center;
        RaycastHit hit;

        float dist = Vector3.Distance(startPoint, targetPoint);
        Vector3 dir = (targetPoint - startPoint).normalized;
        LayerMask enviLayer = LayerMask.GetMask(new string[] { "Environment" });

        if (Physics.Raycast(startPoint, dir, out hit, dist, enviLayer))
        {
            Debug.DrawRay(startPoint, dir * hit.distance, Color.red);
            return false;
        }

        return true;
    }
}
