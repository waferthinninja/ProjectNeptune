using System;

public class Strategy : Card
{

    public Strategy(CardCodename codename) : this(codename, Guid.NewGuid().ToString())
    {
    }

    public Strategy(CardCodename codename, string cardId) : base(codename, cardId)
    {
        CardType = CardType.STRATEGY;
    }
}