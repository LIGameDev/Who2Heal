using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitVitalsWidget: MonoBehaviour 
{
    public Image HealthIndicator;
    public Image ManaIndicator;

    [SerializeField]
    private UnitModel _model;
    public UnitModel Model
    {
        get
        {
            return _model;
        }

        set
        {
            _model = value;
            RefreshUI();
        }
    }

	// Update is called once per frame
	void Update () 
    {
        RefreshUI();
	}

    private void RefreshUI()
    {
        if(Model == null)
        {
            HealthIndicator.enabled = false;
            ManaIndicator.enabled = false;
        }
        else
        {
            HealthIndicator.enabled = Model.health.maxAmount > 0;
            ManaIndicator.enabled = Model.mana.maxAmount > 0;

            if(HealthIndicator.enabled)
                HealthIndicator.fillAmount = Mathf.Lerp(0f, 0.66f, (float)Model.health.amount / (float)Model.health.maxAmount);

            if(ManaIndicator.enabled)
                ManaIndicator.fillAmount = Mathf.Lerp(0f, 0.66f, (float)Model.mana.amount / (float)Model.mana.maxAmount);
        }
    }
}
