using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour {

    public GameObject manaPotion;
    public List<Vector3> manaPotionSpawnLocations;

    private PlayerController playerController;

    void Start () {
        // setup level
        foreach (Vector3 spawnLoc in manaPotionSpawnLocations) {
            GameObject.Instantiate(manaPotion, spawnLoc, Quaternion.identity);
        }

        playerController = GameObject.FindObjectOfType<PlayerController>().GetComponent<PlayerController>();
	}

    private void Update()
    {
        bool doPause = Input.GetButtonUp("Pause");
        if (doPause)
        {
            GameManager.Instance.GoToScene(GameManager.W2HScene.MainMenu); 
        }
    }

    public void PickupPotion(GameObject potion)
    {
        Destroy(potion);
    }
}
