using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitResource
{
    public int amount = 100;
    public int maxAmount = 100;

    public int regenAmount = 1;
    public float regenRate = 0.2f;

    UnitModel model; // for calling coroutines

    public void Init(UnitModel newModel = null)
    {
        if (newModel != null)
        {
            model = newModel; 
        }
        
        StopUpdating(); 
        
        if (regenAmount != 0 && regenRate > 0f)
        {
            model.StartCoroutine(Update()); 
        }
    }

    private IEnumerator Update()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenRate);

            var prevAmount = amount; 

            amount += regenAmount;
            amount = amount.Clamp(0, maxAmount);

            if (prevAmount != amount)
            {
                model.CheckIfDead();
            }
        }
    }

    public void StopUpdating()
    {
        model.StopCoroutine(Update());
    }
}

[System.Serializable]
public class UnitAttribute
{
    public int baseValue = 0;
    public int buffAmount = 0; 
}


public class UnitModel : MonoBehaviour {

    public Team team; 
    public UnitResource health; 
    public UnitResource mana;

    //public UnitAttribute physAttack;
    //public UnitAttribute magicAttack;
    //public UnitAttribute physDefense;
    //public UnitAttribute magicDefense; 

    public bool IsDead
    {
        get
        {
            return health.amount == 0; 
        }
    }

    public delegate void DeathAction();
    public event DeathAction OnDeath;

    public enum Team { Ally, Enemy };
    

    void Start ()
    {
        health.Init(this);
        mana.Init(this);
    }
	
    public bool TryDamage(int amount)
    {
        if (IsDead)
            return false;

        health.amount = (health.amount - amount).Clamp(0, health.maxAmount); 
        CheckIfDead(); 

        return true; 
    }

    public bool TryDrainMana(int amount)
    {
        if (IsDead || mana.amount < amount)
            return false;

        mana.amount = (mana.amount - amount).Clamp(0, mana.maxAmount);
        return true; 
    }

    public bool TryUsePotion(int amount)
    {
        if (IsDead || mana.amount == mana.maxAmount)
            return false;
        mana.amount = (mana.amount + amount).Clamp(0, mana.maxAmount);
        return true;
    }

    public bool TryRevive()
    {
        if (!IsDead)
            return false;

        health.amount = health.maxAmount;
        mana.amount = mana.maxAmount;

        health.Init();
        mana.Init(); 

        return true; 
    }

    public void CheckIfDead()
    {
        if (IsDead)
        {
            if (OnDeath != null)
                OnDeath();

            health.StopUpdating();
            mana.StopUpdating(); 
        }
    }

    public static bool CheckTeamsAllied(Team team1, Team team2)
    {
        if ((team1 == Team.Ally && team2 == Team.Enemy) ||
            (team2 == Team.Ally && team1 == Team.Enemy))
        {
            return false; 
        }
        
        // Map all other relationships to "allied" 
        return true;
    }
}
