using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine; 

public class GameSceneManager : MonoBehaviour {

    public GameObject manaPotion;
    public List<Vector3> manaPotionSpawnLocations;

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

    private PlayerController playerController;

    void Start () {
        // setup level
        foreach (Vector3 spawnLoc in manaPotionSpawnLocations) {
            GameObject.Instantiate(manaPotion, spawnLoc, Quaternion.identity);
        }

        playerController = GameObject.FindObjectOfType<PlayerController>().GetComponent<PlayerController>();

        GameSceneManager_OnCameraRotationChanged(); 
        OnCameraRotationChanged += GameSceneManager_OnCameraRotationChanged;
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

    private void GameSceneManager_OnCameraRotationChanged()
    {
        if (environmentMaterial != null)
        {
            var shiftVec = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up) * new Vector3(0, 0, 1);
            environmentMaterial.SetVector("_ShiftVec", shiftVec);
        }
    }
}
