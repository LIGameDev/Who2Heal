using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitModel))]
[RequireComponent(typeof(UnitMover))]
public class PlayerController : MonoBehaviour {
    
    public Camera gameCamera;
    
    public int manaPotions = 3;
    public int reviveManaCost = 30;
    public int potionMana = 30;

    public NotificationWidget Notification;
    public SceneManager SceneManager;

    UnitModel unitModel; 
    UnitMover unitMover; 
    

    [SerializeField] List<GameObject> revivableCorpses; 
    
	void Start ()
    {
        if (Notification == null)
            Notification = GameObject.FindObjectOfType<NotificationWidget>();

        if (SceneManager == null)
            SceneManager = GameObject.FindObjectOfType<SceneManager>();

        unitModel = GetComponent<UnitModel>();
        unitMover = GetComponent<UnitMover>();

        revivableCorpses = new List<GameObject>(); 

        if (gameCamera == null)
        {
            gameCamera = GameObject.FindWithTag(Tags.MainCamera).GetComponent<Camera>();
        }
    }

    void Update ()
    {
        if (unitModel.IsDead)
            return; 

        float h_axis = Input.GetAxisRaw("Horizontal");
        float v_axis = Input.GetAxisRaw("Vertical");
        bool useRevive = Input.GetButtonDown("Revive");
        bool usePotion = Input.GetButtonDown("Potion"); 

        Quaternion camRot = Quaternion.AngleAxis(gameCamera.transform.rotation.eulerAngles.y, Vector3.up);

        unitMover.Move(camRot * new Vector3(h_axis, 0f, v_axis));

        if (useRevive && CanRevive())
        {
            UnitCorpse closestCorpse = null;
            float closestDist = -1f;

            foreach (GameObject corpse in revivableCorpses)
            {
                UnitCorpse unitCorpse = corpse.GetComponent<UnitCorpse>();
                if (!unitCorpse.CanRevive)
                    continue;

                float dist = (corpse.transform.position - transform.position).magnitude;

                if (closestCorpse == null || dist < closestDist)
                {
                    closestCorpse = unitCorpse;
                    closestDist = dist;
                }
            }

            if (closestCorpse != null && closestCorpse.Revive())
            {
                unitModel.TryDrainMana(reviveManaCost);
                revivableCorpses.Remove(closestCorpse.gameObject);
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

        return revivableCorpses.Count > 0 && unitModel.mana.amount >= reviveManaCost;
    }

    internal bool CanUsePotion()
    {
        if (unitModel == null)
            return false;

        return manaPotions > 0 && unitModel.mana.amount < unitModel.mana.maxAmount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Tags.Revivable && !revivableCorpses.Contains(other.gameObject))
        {
            revivableCorpses.Add(other.gameObject); 
        }

        if (other.tag == Tags.ManaPotion)
        {
            manaPotions++;

            if (Notification != null)
                Notification.Notify("You picked up a mana potion!");

            if (SceneManager != null)
                SceneManager.PickupPotion(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == Tags.Revivable && revivableCorpses.Contains(other.gameObject))
        {
            revivableCorpses.Remove(other.gameObject); 
        }
    }
}
