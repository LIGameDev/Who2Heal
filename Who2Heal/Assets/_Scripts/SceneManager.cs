using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour {

    public GameObject manaPotion;
    public List<Vector3> manaPotionSpawnLocations;

    public Text HUD;
    public Text uiMessage;
    public float uiMessageTimeShown;

    private List<string> uiMessages = new List<string>();
    private List<float> uiMessageLifespans = new List<float>();

    private PlayerController playerController;

    void Start () {
        // setup level
        foreach (Vector3 spawnLoc in manaPotionSpawnLocations) {
            GameObject.Instantiate(manaPotion, spawnLoc, Quaternion.identity);
        }
        HUD.text = "";
        playerController = GameObject.FindObjectOfType<PlayerController>().GetComponent<PlayerController>();
        NotifyHUD();
	}

    void Update()
    {
        uiMessage.text = "";
        for (int i = uiMessageLifespans.Count - 1; i >= 0; i--)
        {
            uiMessageLifespans[i] -= Time.deltaTime;
            if (uiMessageLifespans[i] <= 0f)
            {
                uiMessageLifespans.RemoveAt(i);
                uiMessages.RemoveAt(i);
            }
        }
        foreach (string uiMessageString in uiMessages) {
            uiMessage.text += uiMessageString + "\n";
        }
    }

    public void NotifyHUD()
    {
        HUD.text = "Mana Potions: " + playerController.manaPotions;
    }

	public void PickupPotion(GameObject potion)
    {
        ShowUIMessage("You got\na mana potion!");
        Destroy(potion);
    }

    private void ShowUIMessage(string message)
    {
        uiMessages.Add(message);
        uiMessageLifespans.Add(uiMessageTimeShown);
    }
}
