using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveAbility : UnitAbility
{
    [SerializeField] private int ManaCost = 30;
    [SerializeField] private DetectVolume detectVolume; 
    [SerializeField] private UnitCorpse reviveTarget; 



    public override bool CanUse()
    {
        if (MyUser == null)
            return false;

        return this.State == AbilityState.Ready && MyUser.mana.amount >= ManaCost;

    }

    protected override bool DoUse()
    {
        reviveTarget = null;

        List<GameObject> revivableCorpses = detectVolume.DetectedObjects;
        float closestDist = -1f;

        foreach (GameObject corpse in revivableCorpses)
        {
            UnitCorpse unitCorpse = corpse.GetComponent<UnitCorpse>();
            if (!unitCorpse.CanRevive)
                continue;

            float dist = Vector3.Distance(corpse.transform.position, transform.position);

            if (reviveTarget == null || dist < closestDist)
            {
                reviveTarget = unitCorpse;
                closestDist = dist;
            }
        }

        if (reviveTarget != null)
        {
            return MyUser.TryDrainMana(ManaCost);
        }

        return false;
    }

    protected override bool Execute()
    {
        if (reviveTarget != null && reviveTarget.Revive())
        {
            detectVolume.Remove(reviveTarget.gameObject);
        }

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

    protected override bool StartChanneling()
    {
        return true;
    }
}
