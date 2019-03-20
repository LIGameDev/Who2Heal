using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeleeAbility : UnitAbility
{
    private float range; // set to UnitChaseBehaviour's DesiredRadiusToTarget
    private DetectVolume TargetDetection; // set to UnitChaseBehaviour's TargetDetection
    public List<string> DesiredTags;
    private UnitModel targetUnitModel;
    public int damage;

    private void Start()
    {
        UnitChaseBehaviour chaseBehaviourComp = null;
        if ((chaseBehaviourComp = GetComponent<UnitChaseBehaviour>()) != null)
        {
            range = chaseBehaviourComp.DesiredRadiusToTarget;
            TargetDetection = chaseBehaviourComp.TargetDetection;
        }
    }

    public override bool CanUse()
    {
        if (MyUser == null)
            return false;
        return this.State == AbilityState.Ready;
    }

    protected override bool DoUse()
    {
        IEnumerable<UnitMover> targetObjs = TargetDetection.DetectedObjects.Where(go => DesiredTags.Contains(go.tag)).Select(go => go.GetComponent<UnitMover>());
        if (targetObjs != null)
        {
            foreach (UnitMover targetObj in targetObjs)
            {
                Vector3 targetPoint = targetObj.transform.position;
                if (Vector3.Distance(transform.position, targetPoint) <= range)
                {
                    targetUnitModel = targetObj.GetComponent<UnitModel>();
                    return true;
                }
            }
        }
        return false;
    }

    protected override bool StartChanneling()
    {
        return true;
    }

    protected override bool Execute()
    {
        if (targetUnitModel == null)
        {
            Debug.LogError("no melee target");
            return false;
        }
        // We're going to assume a channeling time of 0 for now so the target will not have moved out of range
        targetUnitModel.TryDamage(damage);
        return true;
    }

    protected override bool StartCooldown()
    {
        return true;
    }

    protected override bool SetReady()
    {
        return true;
    }

}
