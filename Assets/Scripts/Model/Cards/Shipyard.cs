using UnityEngine;
using System.Collections;
using System;

[Serializable()]
public class Shipyard : PlayableCard, IDamageable {
 
    public int MaxSize { get; private set; }
    public int Efficiency { get; private set; } // number of construction tokens removed per turn

    public Ship HostedShip { get; private set; }

    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }

    public Shipyard(CardCodename codename) : this (codename, Guid.NewGuid().ToString())
    { }

    public Shipyard(CardCodename codename, string cardId) : base (codename, cardId)
    {
        CardType = CardType.SHIPYARD;
        MaxSize = GetMaxSize();
        Efficiency = GetEfficiency();        
        MaxHealth = DetermineMaxHealth();
        CurrentHealth = MaxHealth;
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

    public void DealDamage(int damage)
    {
        throw new NotImplementedException();
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
