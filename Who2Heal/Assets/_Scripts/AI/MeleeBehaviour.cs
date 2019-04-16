using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeleeBehaviour : ABotBehaviour
{
    public float range;

    private UnitChaseBehaviour chaseBehaviourComp; // need to access the DesiredRadiusToTarget of this
    private MeleeAbility meleeAbility; // need to access the DesiredTags of this

    void Start()
    {
        chaseBehaviourComp = GetComponent<UnitChaseBehaviour>();
        meleeAbility = GetComponent<MeleeAbility>();
    }

    public override bool ShouldProcessUpdate()
    {
        if (chaseBehaviourComp == null)
        {
            Debug.LogError("unassigned UnitChaseBehaviour");
            return false;
        }
        if (meleeAbility == null)
        {
            Debug.LogError("unassigned MeleeAbility");
            return false;
        }

        IEnumerable<UnitMover> targetObjs = chaseBehaviourComp.TargetDetection.DetectedObjects.Where(go => go != null && meleeAbility.DesiredTags.Contains(go.tag)).Select(go => go.GetComponent<UnitMover>());
        if (targetObjs != null)
        {
            foreach (UnitMover targetObj in targetObjs)
            {
                Vector3 targetPoint = targetObj.transform.position;
                if (Vector3.Distance(transform.position, targetPoint) <= chaseBehaviourComp.DesiredRadiusToTarget)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public override void ProcessUpdate(UnitMover unitMover)
    {
        if (meleeAbility == null)
        {
            Debug.LogError("unassigned MeleeAbility");
            return;
        }
        meleeAbility.Use();
    }
}
