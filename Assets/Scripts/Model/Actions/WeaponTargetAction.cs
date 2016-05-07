public class WeaponTargetAction : PlayerAction
{
    public Ship Firer { get; private set; }
    public int WeaponIndex { get; private set; }
    public DamageableCard Target { get; private set; }

    public WeaponTargetAction(Ship firer, int weaponIndex, DamageableCard target) : base(ActionType.WEAPON_TARGET)
    {
        Firer = firer;
        WeaponIndex = weaponIndex;
        Target = target;
    }

    public override string ToString()
    {
        return base.ToString() + "|" + Firer.CardId + "|" + WeaponIndex.ToString() + "|" + Target.CardId;

    }
}