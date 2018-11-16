using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour {

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


	public void PickupPotion(GameObject potion)
    {
        Destroy(potion);
    }
}
