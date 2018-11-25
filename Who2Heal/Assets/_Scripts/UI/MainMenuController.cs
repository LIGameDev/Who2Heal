using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class MainMenuController : MonoBehaviour {

    [SerializeField] Button exitButton;

    private void Start()
    {
#if UNITY_EDITOR
        Debug.Log("[MainMenuController] Non-standalone platform; disabling Exit button.");
        exitButton.gameObject.SetActive(false);
#else
        Debug.Log("[MainMenuController] Standalone platform; leaving Exit button enabled.");
#endif
    }

    public void StartGame()
    {
        GameManager.Instance.GoToScene(GameManager.W2HScene.Game); 
    }
    
    public void ExitGame()
    {
        Application.Quit(); 
    }

}
