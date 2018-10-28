using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCorpse : MonoBehaviour {
    
    public GameObject unit; 

    public bool CanRevive
    {
        get
        {
            return unit != null;
        }
    }

	public bool Revive()
    {
        if (!CanRevive)
            return false;

        unit.SetActive(true);
        unit.GetComponent<UnitModel>().TryRevive();

        Destroy(gameObject); 

        return true; 
    }
}
