using UnityEngine;

public class PotionAbilityWidget : BaseAbilityWidget
{
    [SerializeField]
    private PlayerController model;

    protected override bool IsDoable()
    {
        return model.CanUsePotion();
    }
}
