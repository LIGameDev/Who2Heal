using UnityEngine;

public class ResAbilityWidget : BaseAbilityWidget
{
    [SerializeField]
    private PlayerController model;

    protected override bool IsDoable()
    {
        return model.CanRevive();
    }
}
