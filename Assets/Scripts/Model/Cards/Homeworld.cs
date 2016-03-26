using System;

public class Homeworld : Card, IDamageable {

    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }

    public Homeworld(CardCodename codename) : this(codename, Guid.NewGuid().ToString())
    {
    }

    public Homeworld(CardCodename codename, string cardId) : base(codename, cardId)
    {
        this.CardType = CardType.HOMEWORLD;
        MaxHealth = DetermineMaxHealth();
        CurrentHealth = MaxHealth;
    }

    public void DealDamage(int damage)
    {
        CurrentHealth -= damage;
    }

    private int DetermineMaxHealth()
    {
        if (CardData.MaxHealths.ContainsKey(CardCodename))
        {
            return CardData.MaxHealths[CardCodename];
        }
        else
        {
            throw new Exception(string.Format("Failed to get health for {0}", CardCodename));
        }
    }
}
