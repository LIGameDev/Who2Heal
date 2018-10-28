using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitModel))]
[RequireComponent(typeof(UnitMover))]
public class BotController : MonoBehaviour {

    public GameObject corpsePrefab; 
    
    UnitModel unitModel;
    UnitMover unitMover;

    Vector3 targetMovePoint;
    float targetMoveTimer;

    void Start () {
        unitModel = GetComponent<UnitModel>();
        unitMover = GetComponent<UnitMover>();

        targetMovePoint = transform.position; 
        targetMoveTimer = Random.Range(1f, 3f);

        unitModel.OnDeath += UnitModel_OnDeath; 
    }
	
	void Update () {
        if (unitModel.IsDead)
            return;

        // Temp behavior: wander near origin
        targetMoveTimer = Mathf.MoveTowards(targetMoveTimer, 0f, Time.deltaTime); 
        if (targetMoveTimer == 0f)
        {
            // Pick new target position
            targetMovePoint = new Vector3(Random.Range(-8f, 8f), 0f, Random.Range(-8f, 8f)); 

            targetMoveTimer = Random.Range(1.5f, 5f); 
        }

        Vector3 toTarget = targetMovePoint - transform.position;
        if (toTarget.magnitude > 1f)
        {
            unitMover.Move(toTarget.normalized);
        }
    }

    private void OnEnable()
    {
        if (unitModel != null)
            unitModel.OnDeath += UnitModel_OnDeath;
    }

    private void OnDisable()
    {
        if (unitModel != null)
            unitModel.OnDeath -= UnitModel_OnDeath; 
    }

    private void UnitModel_OnDeath()
    {
        if (corpsePrefab != null)
        {
            var corpseObj = Instantiate(corpsePrefab, transform.position, transform.rotation);
            corpseObj.GetComponent<UnitCorpse>().unit = gameObject; 

            gameObject.SetActive(false); 
        }
    }
}
