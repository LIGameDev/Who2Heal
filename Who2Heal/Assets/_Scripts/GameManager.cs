using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    private static GameManager _instance = null; 

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("[GameManager] Instance get called but is null, generating GameObject"); 
                var gameManagerObj = new GameObject("GameManager");
                _instance = gameManagerObj.AddComponent<GameManager>(); 
            }

            return _instance;
        }
    }

    [SerializeField] W2HScene currentScene = W2HScene.MainMenu;

    public W2HScene CurrentScene
    {
        get
        {
            return currentScene; 
        }
    }

    public enum W2HScene
    {
        MainMenu = 0, 
        Game = 1, 
    }

    private void Awake()
    {
        // Check if another instance of GameManager exists already; set instance to this if not
        if (_instance == null)
        {
            Debug.Log("[GameManager] Awake: Instance null, setting to this"); 
            _instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else if (_instance != this)
        {
            Debug.Log("[GameManager] Awake: Instance is non-null and not this, destroying this");
            Destroy(gameObject); 
        }
    }

    public void GoToScene(W2HScene scene)
    {
        GoToScene((int)scene); 
    }

    private void GoToScene(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
            return; 

        var loadOp = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
        if (loadOp != null)
        {
            StartCoroutine(LoadScene(loadOp));
        }
    }

    private IEnumerator LoadScene(AsyncOperation loadOp)
    {
        while (!loadOp.isDone)
        {
            yield return loadOp.isDone;

            //int progressPercent = (int)(loadOp.progress * 100f); 
            //Debug.Log(string.Format("[GameManager] Loading game scene ({0}%)", progressPercent)); 
        }
    }

    public void ReloadScene()
    {
        GoToScene(SceneManager.GetActiveScene().buildIndex); 
    }
}
