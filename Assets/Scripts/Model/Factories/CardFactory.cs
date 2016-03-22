﻿using System;
using System.Collections.Generic;

public static class CardFactory  {

	public static Card CreateCard(CardCodename cardCodename)
    {
        return CreateCard(cardCodename, Guid.NewGuid().ToString());
    }

    public static Card CreateCard(CardCodename cardCodename, string cardId)
    {
        CardType cardType = CardData.CardTypes[cardCodename];
        switch(cardType)
        {
            case CardType.SHIP:
                return new Ship(cardCodename, cardId);
            case CardType.SHIPYARD:
                return new Shipyard(cardCodename, cardId);
            default:
                throw new Exception("Invalid card type or card type not found for " + cardCodename);
        }
    }
    
}
