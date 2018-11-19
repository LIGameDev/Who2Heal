using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitModel))]
[RequireComponent(typeof(UnitMover))]
public class UnitController : MonoBehaviour {

    public GameObject corpsePrefab;

    protected UnitModel unitModel;
    protected UnitMover unitMover;
    protected List<UnitAbility> unitAbilities; 
    
    protected virtual void Start () {
        unitModel = GetComponent<UnitModel>();
        unitMover = GetComponent<UnitMover>();

        RefreshUnitAbilities(); 

        unitModel.OnDeath += UnitModel_OnDeath;
    }
	
	protected virtual void Update () {
        if (unitModel.IsDead)
            return;
    }

    protected virtual void OnEnable()
    {
        if (unitModel != null)
            unitModel.OnDeath += UnitModel_OnDeath;
    }

    protected virtual void OnDisable()
    {
        if (unitModel != null)
            unitModel.OnDeath -= UnitModel_OnDeath;
    }

    protected virtual void UnitModel_OnDeath()
    {
        if (corpsePrefab != null)
        {
            var corpseObj = Instantiate(corpsePrefab, transform.position, transform.rotation);
            corpseObj.GetComponent<UnitCorpse>().unit = gameObject;

            gameObject.SetActive(false);
        }
    }

    protected void RefreshUnitAbilities()
    {
        if (unitAbilities == null)
            unitAbilities = new List<UnitAbility>(); 
        else
            unitAbilities.Clear();

        GetComponents<UnitAbility>(unitAbilities); 
    }

    protected bool UseAbility<T>() where T : UnitAbility
    {
        var ability = GetAbility<T>();
        if (ability != null)
        {
            return ability.Use();
        }

        Debug.Log(string.Format("[{0}] UseAbility(): No ability of type in list", name)); 
        return false; 
    }

    protected T GetAbility<T>() where T : UnitAbility
    {
        foreach (UnitAbility ability in unitAbilities)
        {
            if (ability is T)
            {
                return ability as T; 
            }
        }

        return null; 
    }
}
