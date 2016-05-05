using System;

public class Missile : IDamageable {

    public int Damage { get; private set; }
    public string Id { get; private set; }

    public IDamageable Target { get; private set; }

    public Missile(int damage): this(damage,"")
    {
        Id = Guid.NewGuid().ToString();
    }

    public Missile(int damage, string id)
    {        
        Damage = damage;
        Target = null;
        Id = id;
    }

    internal void SetTarget(IDamageable target)
    {
        Target = target;
    }

    public void DealDamage(int damage)
    {
        throw new NotImplementedException();
    }
}
