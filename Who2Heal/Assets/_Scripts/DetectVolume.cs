using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectVolume : MonoBehaviour {

    public string detectTag;

    [SerializeField] List<GameObject> detectedObjects = new List<GameObject>();
    public List<GameObject> DetectedObjects
    {
        get { return detectedObjects; }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("[DetectVolume] Trigger entered"); 
        if (other.tag == detectTag && !detectedObjects.Contains(other.gameObject))
        {
            detectedObjects.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("[DetectVolume] Trigger exited");
        if (other.tag == detectTag && detectedObjects.Contains(other.gameObject))
        {
            detectedObjects.Remove(other.gameObject);
        }
    }
    
    public GameObject GetClosest(Vector3 position)
    {
        GameObject closest = null;
        float closestDist = -1f; 

        for (int i = 0; i < detectedObjects.Count; i++)
        {
            float checkDist = (position - detectedObjects[i].transform.position).magnitude;

            if (i == 0 || checkDist < closestDist)
            {
                closest = detectedObjects[i];
                closestDist = checkDist;
            }
        }

        return closest; 
    }

    public void Remove(GameObject obj)
    {
        if (detectedObjects.Contains(obj))
        {
            detectedObjects.Remove(obj);
        }
    }
}
