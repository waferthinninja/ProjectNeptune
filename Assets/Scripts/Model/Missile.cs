using System;

public class Missile : DamageableCard {

    public int Damage { get; private set; }

    public DamageableCard Target { get; private set; }


    public Missile(CardCodename codename) : this (codename, Guid.NewGuid().ToString())
    { }

    public Missile(CardCodename codename, string cardId) : base (codename, cardId)
    {
        CardType = CardType.MISSILE;        
    }

    internal void SetDamage(int damage)
    {
        Damage = damage;
    }

    internal void SetTarget(DamageableCard target)
    {
        Target = target;
    }
    
}
