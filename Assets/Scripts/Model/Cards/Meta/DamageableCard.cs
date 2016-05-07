using System;

public class DamageableCard : Card {

    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }

    public Action<DamageableCard> OnHealthChanged;

    public DamageableCard(CardCodename cardCodename, string cardId) : base(cardCodename, cardId)
    {

        MaxHealth = DetermineMaxHealth();
        CurrentHealth = MaxHealth;
    }

    public void DealDamage(int damage)
    {
        CurrentHealth -= damage;
        if (OnHealthChanged != null)
        {
            OnHealthChanged(this);
        }
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

    public void RegisterOnHealthChangedCallback(Action<DamageableCard> onHealthChanged)
    {
        OnHealthChanged += onHealthChanged;
    }
}
