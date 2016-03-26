using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Ship : PlayableCard, IDamageable {

    public int Size { get; private set; }
    public int ConstructionRemaining { get; private set; }
    public List<Weapon> Weapons { get; private set; }
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }

    public Ship(CardCodename codename) : this (codename, Guid.NewGuid().ToString())
    {
        
    }

    public Ship(CardCodename codename, string cardId) : base (codename, cardId)
    {
        CardType = CardType.SHIP;
        Size = DetermineSize();
        Weapons = DetermineWeapons();
        MaxHealth = DetermineMaxHealth();
        CurrentHealth = MaxHealth;
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

    private List<Weapon> DetermineWeapons()
    {
        if (CardData.Weapons.ContainsKey(CardCodename))
        {
            return CardData.Weapons[CardCodename];
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

    public void DealDamage(int damage)
    {
        CurrentHealth -= damage;
    }
}
