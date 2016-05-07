﻿using System;

public class UnknownCard : Card {
    public UnknownCard(CardCodename codename) : this (codename, Guid.NewGuid().ToString())
    {
    }

    public UnknownCard(CardCodename codename, string cardId) : base (codename, cardId)
    {
        CardType = CardType.UNKNOWN;
    }
}
