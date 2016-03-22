using UnityEngine;
using System.Collections;
using System;

[Serializable()]
public class Shipyard : Card {
 
    public int MaxSize { get; private set; }
    public int Efficiency { get; private set; } // number of construction tokens removed per turn


    public Ship HostedShip { get; private set; }

    public Shipyard(CardCodename type) : base (type, Guid.NewGuid().ToString())
    { }

    public Shipyard(CardCodename type, string cardId) : base (type, cardId)
    {
        MaxSize = GetMaxSize();
        Efficiency = GetEfficiency();
        CardType = CardType.SHIPYARD;  
    }    

    private int GetEfficiency()
    {
        if (CardData.Efficiencies.ContainsKey(CardCodename))
        {
            return CardData.Efficiencies[CardCodename];
        }
        else
        {
            throw new Exception(string.Format("Failed to get efficiency for {0}", CardCodename));
        }
    }

    private int GetMaxSize()
    {
        if (CardData.Efficiencies.ContainsKey(CardCodename))
        {
            return CardData.MaxSizes[CardCodename];
        }
        else
        {
            throw new Exception(string.Format("Failed to get efficiency for {0}", CardCodename));
        }
    }

    public void HostCard(Ship ship)
    {
        HostedShip = ship;
    }

    public void ClearHostedCard()
    {
        HostedShip = null;
    }

   

}
