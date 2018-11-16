using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionWidget : MonoBehaviour 
{
    [SerializeField]
    private PlayerController model;

    [SerializeField]
    private GameObject PotionIconTemplate;

    [SerializeField]
    private GameObject potionIconParent;

    private int lastPotionCount;


	// Use this for initialization
	void Start () 
    {
        // ASSUMES THAT OLL MY CHILDREN ARE POTION OBJECTS ONLY
        lastPotionCount = this.transform.childCount;

        if (model == null)
        {
            Debug.LogWarning("Potion Widget doesnt have a Player Controler. UI element is disabling.");
            this.gameObject.SetActive(false);
            return;
        }
            
        RefreshUI();
	}
	
	// Update is called once per frame
	void Update () 
    {
        RefreshUI();
	}

    private void RefreshUI()
    {
        if (model == null || lastPotionCount == model.manaPotions)
            return;

        if(model.manaPotions > lastPotionCount)
        {
            // add potions
            for (int i = lastPotionCount; i < model.manaPotions; i++)
            {
                GameObject newPotion = Instantiate(PotionIconTemplate);
                newPotion.transform.SetParent(potionIconParent.transform);
            }
        }
        else
        {
            // remove potions
            for (int i = potionIconParent.transform.childCount - 1; i >= model.manaPotions; i--)
            {
                Transform toDestory = potionIconParent.transform.GetChild(i);
                Destroy(toDestory.gameObject);
            }
        }

        lastPotionCount = model.manaPotions;
    }
}
