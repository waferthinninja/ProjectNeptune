public class Weapon {

    public WeaponType WeaponType { get; private set; }
    public int Damage { get; private set; }

    public Weapon (WeaponType weaponType, int damage)
    {
        WeaponType = weaponType;
        Damage = damage;
    }

}
