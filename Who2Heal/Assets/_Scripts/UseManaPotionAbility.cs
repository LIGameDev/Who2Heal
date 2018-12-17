using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseManaPotionAbility : UnitAbility
{
    [SerializeField] PlayerController player;
    [SerializeField] int ManaToAdd = 30;

    public override bool CanUse()
    {
        if (MyUser == null)
            return false;

        return this.State == AbilityState.Ready && MyUser.mana.amount < MyUser.mana.maxAmount && player.manaPotions > 0;

    }

    protected override bool DoUse()
    {
        return true;
    }

    protected override bool Execute()
    {
        MyUser.TryUsePotion(ManaToAdd);
        player.manaPotions -= 1;
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
