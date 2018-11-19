using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveAbility : UnitAbility {

    public float channelTime = 1f;
    public float executeTime = 0.2f;
    public float cooldownTime = 5f; 

    public DetectVolume detectVolume; 

    [SerializeField] AbilityState state = AbilityState.Ready;
    public AbilityState State
    {
        get { return state; }
    }
    
    [SerializeField] float timer = -1f; 
    [SerializeField] UnitCorpse reviveTarget; 
    
    private void Update()
    {
        if (timer > 0f)
        {
            timer = Mathf.MoveTowards(timer, 0f, Time.deltaTime); 
        }

        if (timer == 0f)
        {
            switch (state)
            {
                case AbilityState.Channeling:
                    Execute(); 
                    break;

                case AbilityState.Executing:
                    PutOnCooldown(); 
                    break;

                case AbilityState.OnCooldown:
                    SetReady();
                    break;
            }
        }
    }

    public override bool Use()
    {
        reviveTarget = null;

        List<GameObject> revivableCorpses = detectVolume.DetectedObjects;
        float closestDist = -1f;

        foreach (GameObject corpse in revivableCorpses)
        {
            UnitCorpse unitCorpse = corpse.GetComponent<UnitCorpse>();
            if (!unitCorpse.CanRevive)
                continue;

            float dist = (corpse.transform.position - transform.position).magnitude;

            if (reviveTarget == null || dist < closestDist)
            {
                reviveTarget = unitCorpse;
                closestDist = dist;
            }
        }

        if (reviveTarget != null)
        {
            state = AbilityState.Channeling;
            timer = channelTime;

            return true; 
        }

        return false;
    }

    private void Execute()
    {
        state = AbilityState.Executing;
        timer = executeTime;

        if (reviveTarget != null && reviveTarget.Revive())
        {
            detectVolume.Remove(reviveTarget.gameObject);
        }
    }

    private void PutOnCooldown()
    {
        state = AbilityState.OnCooldown;
        timer = cooldownTime; 
    }

    private void SetReady()
    {
        state = AbilityState.Ready;
        timer = -1f; 
    }
}
