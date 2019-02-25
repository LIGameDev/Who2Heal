using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class EndScreenController : MonoBehaviour {
    
    [SerializeField] Text endResultText;
    [SerializeField] Text endConditionText;

	void Start () {
        gameObject.SetActive(false); 
	}
	
    public void Show(string resultText, string conditionText)
    {
        gameObject.SetActive(true);
        endResultText.text = resultText;
        endConditionText.text = conditionText;
    }

    public void ReturnToMainMenu()
    {
        GameManager.Instance.GoToScene(GameManager.W2HScene.MainMenu); 
    }
}
