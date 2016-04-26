using System;

public class Weapon {

    public WeaponType WeaponType { get; private set; }
    public int Damage { get; private set; }

    public IDamageable Target { get; private set; }

    public Weapon (WeaponType weaponType, int damage)
    {
        WeaponType = weaponType;
        Damage = damage;
        Target = null;
    }

    internal void SetTarget(IDamageable target)
    {
        Target = target;
    }

    internal void ClearTarget()
    {
        Target = null;
    }
}
