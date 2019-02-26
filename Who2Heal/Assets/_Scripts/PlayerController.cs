using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : UnitController {
    
    public Camera gameCamera;
    
    public int manaPotions = 3;

    public NotificationWidget Notification;
    public GameSceneManager GameSceneManager;

    protected override void Start ()
    {
        base.Start();

        if (Notification == null)
            Notification = GameObject.FindObjectOfType<NotificationWidget>();

        if (GameSceneManager == null)
            GameSceneManager = GameObject.FindObjectOfType<GameSceneManager>();

        GetAbility<ReviveAbility>().MyUser = this.unitModel;
        GetAbility<UseManaPotionAbility>().MyUser = this.unitModel;

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

        if (useRevive && CanUseAbility<ReviveAbility>())
        {
            UseAbility<ReviveAbility>(); 
        }

        if (usePotion && CanUseAbility<UseManaPotionAbility>())
        {
            UseAbility<UseManaPotionAbility>();
        }
        
    }

    protected override void UnitModel_OnDeath()
    {
        base.UnitModel_OnDeath();

        GameSceneManager.EndConditionSatisfied(GameSceneManager.EndCondition.LosePlayerKilled, 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case Tags.ManaPotion: 
                manaPotions++;

                if (Notification != null)
                    Notification.Notify("You picked up a mana potion!");

                if (GameSceneManager != null)
                    GameSceneManager.PickupPotion(other.gameObject);

                break;

            case Tags.ExitZone:
                GameSceneManager.EndConditionSatisfied(GameSceneManager.EndCondition.WinPlayerEscape); 
                break;
        }
    }

}
