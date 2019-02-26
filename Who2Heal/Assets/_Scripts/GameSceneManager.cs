using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine; 

public class GameSceneManager : MonoBehaviour {

    public GameObject manaPotion;
    public List<Vector3> manaPotionSpawnLocations;

    [SerializeField] UnitModel enemyGeneral;

    [SerializeField] Material environmentMaterial;
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    [SerializeField] float cameraRotationSpeed = 30f;

    public delegate void CameraRotationAction();
    public event CameraRotationAction OnCameraRotationChanged;

    public float CameraYAngle
    {
        get
        {
            return Camera.main.transform.rotation.eulerAngles.y; 
        }
    }

    public bool GameFinished { private set; get; }

    private PlayerController playerController;
    public EndScreenController endScreenController; 

    public enum EndCondition
    {
        WinPlayerEscape, 
        WinGeneralKilled,
        LosePlayerKilled,
    }

    void Start () {
        // setup level
        foreach (Vector3 spawnLoc in manaPotionSpawnLocations) {
            GameObject.Instantiate(manaPotion, spawnLoc, Quaternion.identity);
        }

        playerController = GameObject.FindObjectOfType<PlayerController>().GetComponent<PlayerController>();
        endScreenController = GameObject.FindObjectOfType<EndScreenController>().GetComponent<EndScreenController>(); 

        GameSceneManager_OnCameraRotationChanged(); 
        OnCameraRotationChanged += GameSceneManager_OnCameraRotationChanged;

        GameFinished = false; 

        if (enemyGeneral != null)
        {
            enemyGeneral.OnDeath += EnemyGeneral_OnDeath;
        }
	}
    
    private void Update()
    {
        bool doPause = Input.GetButtonUp("Pause");
        if (doPause)
        {
            GameManager.Instance.GoToScene(GameManager.W2HScene.MainMenu); 
        }
        
        bool rotateCamLeft = Input.GetKey(KeyCode.LeftShift);
        bool rotateCamRight = Input.GetKey(KeyCode.Space);
        if (rotateCamLeft)
        {
            virtualCamera.VirtualCameraGameObject.transform.Rotate(Vector3.up, cameraRotationSpeed * Time.deltaTime, Space.World);
            OnCameraRotationChanged(); 
        }
        if (rotateCamRight)
        {
            virtualCamera.VirtualCameraGameObject.transform.Rotate(Vector3.up, -cameraRotationSpeed * Time.deltaTime, Space.World);
            OnCameraRotationChanged(); 
        }
        
    }

    private void OnApplicationQuit()
    {
        // Reset environment material
        environmentMaterial.SetVector("_ShiftVec", new Vector3(0, 0, 1));
    }

    public void PickupPotion(GameObject potion)
    {
        Destroy(potion);
    }

    public void EndConditionSatisfied(EndCondition condition, float timeDelay = 0f)
    {
        if (GameFinished)
            return;

        GameFinished = true; 

        if (timeDelay > 0f)
            StartCoroutine(ShowEndScreen(condition, timeDelay)); 
        else
            StartCoroutine(ShowEndScreen(condition, 0f));
    }

    private IEnumerator ShowEndScreen(EndCondition condition, float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay); 

        string resultText = "WHAT";
        string conditionText = "?????????";
        bool didWin = true; 

        switch (condition)
        {
            case EndCondition.WinPlayerEscape:
                resultText = "YOU WIN";
                conditionText = "running away is, contrary to popular opinion, a valid solution";
                didWin = true; 
                break;

            case EndCondition.WinGeneralKilled:
                resultText = "YOU WIN";
                conditionText = "i guess finishing the job you were assigned works too"; //report bard for stealing my kill smh
                didWin = true;
                break;

            case EndCondition.LosePlayerKilled:
                resultText = "YOU LOSE";
                conditionText = "how can you eat more cookies if you're dead";
                didWin = false;
                break;
        }

        endScreenController?.Show(resultText, conditionText, didWin);
    }

    private void EnemyGeneral_OnDeath()
    {
        EndConditionSatisfied(EndCondition.WinGeneralKilled, 0.5f); 
    }

    private void GameSceneManager_OnCameraRotationChanged()
    {
        if (environmentMaterial != null)
        {
            var shiftVec = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up) * new Vector3(0, 0, 1);
            environmentMaterial.SetVector("_ShiftVec", shiftVec);
        }
    }
}
