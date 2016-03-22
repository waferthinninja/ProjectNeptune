using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Ship : Card {

    public int Size { get; private set; }
    public int ConstructionRemaining { get; private set; }

    public Ship(CardCodename type) : base (type, Guid.NewGuid().ToString())
    { }

    public Ship(CardCodename type, string cardId) : base (type, cardId)
    {
        CardType = CardType.SHIP;
        Size = DetermineSize();
    }

    private int DetermineSize()
    {
        if (CardData.Sizes.ContainsKey(CardCodename))
        {
            return CardData.Sizes[CardCodename];
        }
        else
        {
            throw new Exception(string.Format("Failed to get size for {0}", CardCodename));
        }
    }

    public void StartConstruction()
    {
        ConstructionRemaining = Size;
    }

    public void AdvanceConstruction(int workDone)
    {
        ConstructionRemaining -= workDone;
        if (ConstructionRemaining < 0)
            ConstructionRemaining = 0;
    }


}
