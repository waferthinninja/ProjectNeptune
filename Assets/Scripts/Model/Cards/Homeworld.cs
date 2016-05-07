using System;

public class Homeworld : DamageableCard
{ 
    public Homeworld(CardCodename codename) : this(codename, Guid.NewGuid().ToString())
    {
    }

    public Homeworld(CardCodename codename, string cardId) : base(codename, cardId)
    {
        this.CardType = CardType.HOMEWORLD;
    }
    
}
