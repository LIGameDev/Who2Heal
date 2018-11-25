using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : UnitController {
    
    public Camera gameCamera;
    
    public int manaPotions = 3;
    public int reviveManaCost = 30;
    public int potionMana = 30;

    public NotificationWidget Notification;
    public GameSceneManager GameSceneManager;

    ReviveAbility reviveAbility; 

    protected override void Start ()
    {
        base.Start();

        if (Notification == null)
            Notification = GameObject.FindObjectOfType<NotificationWidget>();

        if (GameSceneManager == null)
            GameSceneManager = GameObject.FindObjectOfType<GameSceneManager>();
 
        reviveAbility = GetAbility<ReviveAbility>();

        if (gameCamera == null)
        {
            gameCamera = GameObject.FindWithTag(Tags.MainCamera).GetComponent<Camera>();
        }
    }

	
	protected override void Update ()
    {
        base.Update(); 

        float h_axis = Input.GetAxisRaw("Horizontal");
        float v_axis = Input.GetAxisRaw("Vertical");
        bool useRevive = Input.GetButtonDown("Revive");
        bool usePotion = Input.GetButtonDown("Potion"); 

        Quaternion camRot = Quaternion.AngleAxis(gameCamera.transform.rotation.eulerAngles.y, Vector3.up);

        unitMover.Move(camRot * new Vector3(h_axis, 0f, v_axis));

        if (useRevive && CanRevive())
        {
            bool abilityUsed = UseAbility<ReviveAbility>(); 

            if (abilityUsed)
            {
                unitModel.TryDrainMana(reviveManaCost);
            }
        }

        if (usePotion && CanUsePotion())
        {
            unitModel.TryUsePotion(potionMana);
            manaPotions--;
        }
        
    }

    public bool CanRevive()
    {
        if (unitModel == null)
            return false;
        
        bool reviveReady = (reviveAbility.State == UnitAbility.AbilityState.Ready);
        bool sufficientMana = (unitModel.mana.amount >= reviveManaCost); 

        return reviveReady && sufficientMana;
    }

    internal bool CanUsePotion()
    {
        if (unitModel == null)
            return false;

        return manaPotions > 0 && unitModel.mana.amount < unitModel.mana.maxAmount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Tags.ManaPotion)
        {
            manaPotions++;

            if (Notification != null)
                Notification.Notify("You picked up a mana potion!");

            if (GameSceneManager != null)
                GameSceneManager.PickupPotion(other.gameObject);
        }
    }

}
