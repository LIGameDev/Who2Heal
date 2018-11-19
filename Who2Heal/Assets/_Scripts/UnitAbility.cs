using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitAbility : MonoBehaviour {

    //public GameObject effectVolume;
    public Vector2 range;
    //public bool requireTarget; 
    //public float manaCost; 

    public enum AbilityState { Ready, Channeling, Executing, OnCooldown }; 

    public abstract bool Use(); 
}
