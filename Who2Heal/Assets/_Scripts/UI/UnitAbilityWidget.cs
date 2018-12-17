using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UnitAbilityWidget : MonoBehaviour
{
    private Animator animator;

    [SerializeField]
    private UnitAbility AbilityModel;

    [SerializeField]
    private Image BackImage;

    [SerializeField]
    private Image FrontImage;

    [SerializeField]
    private float activeAlpha = 1;

    [SerializeField]
    private float inactiveAlpha = 0.5f;

    // Use this for initialization
    void Start()
    {
        AbilityModel.StateChanged += AbilityModel_StateChanged;
        animator = GetComponent<Animator>();
        UpdateUI();
    }

    private void OnDestroy()
    {
        AbilityModel.StateChanged -= AbilityModel_StateChanged;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    protected virtual void UpdateUI()
    {
        float alpha = GetAlpha();
        var opacityObjects = this.GetComponentsInChildren<MaskableGraphic>();
        foreach (MaskableGraphic mg in opacityObjects)
        {
            Color c = mg.color;
            c.a = alpha;
            mg.color = c;
        }
    }

    private void AbilityModel_StateChanged(object sender, EventArgs e)
    {
        if(animator != null)
        {
            animator.SetInteger("AbilityState", (int)AbilityModel.State);
        }
    }

    private float GetAlpha()
    {
        if(AbilityModel.CanUse())
        {
            return activeAlpha;
        }
        else
        {
            return inactiveAlpha;
        }
    }
}

