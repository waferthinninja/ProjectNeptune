using System;

public class Operation : PlayableCard {


    public Operation(CardCodename codename) : this (codename, Guid.NewGuid().ToString())
    {
    }

    public Operation(CardCodename codename, string cardId) : base (codename, cardId)
    {
        CardType = CardType.OPERATION;
    }
}
