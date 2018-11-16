using UnityEngine;
using UnityEngine.UI;

public abstract class BaseAbilityWidget : MonoBehaviour 
{
    [SerializeField]
    private float activeAlpha = 1;

    [SerializeField]
    private float inactiveAlpha = 0.5f;

    protected abstract bool IsDoable();

	// Use this for initialization
	void Start () 
    {
        UpdateUI();	
	}
	
	// Update is called once per frame
	void Update () 
    {
        UpdateUI();	
	}

    private void UpdateUI()
    {
        float alpha = 1;

        if (IsDoable())
        {
            alpha = activeAlpha;
        }
        else
        {
            alpha = inactiveAlpha;
        }

        var opacityObjects = this.GetComponentsInChildren<MaskableGraphic>();
        foreach (MaskableGraphic mg in opacityObjects)
        {
            Color c = mg.color;
            c.a = alpha;
            mg.color = c;
        }
    }
}
