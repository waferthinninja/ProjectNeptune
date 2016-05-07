using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Ship : DamageableCard {

    public int Size { get; private set; }
    public int ConstructionRemaining { get; private set; }
    public List<Weapon> Weapons { get; private set; }
    public bool IsDeployed { get; private set; } 

    private Action<Ship> OnDeploy;
    private Action<Ship> OnConstructionChanged;

    public Ship(CardCodename codename) : this (codename, Guid.NewGuid().ToString())
    {        
    }

    public Ship(CardCodename codename, string cardId) : base (codename, cardId)
    {
        CardType = CardType.SHIP;
        Size = DetermineSize();
        Weapons = DetermineWeapons();
        IsDeployed = false;
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
        if (OnConstructionChanged != null)
        {
            OnConstructionChanged(this);
        }
    }

    public void AdvanceConstruction(int workDone)
    {
        ConstructionRemaining -= workDone;
        if (ConstructionRemaining < 0)
            ConstructionRemaining = 0;

        if (OnConstructionChanged != null)
        {

        }
    }

    public void Deploy()
    {
        IsDeployed = true;
        if (OnDeploy != null)
        {
            OnDeploy(this);
        }
    }

    public void RegisterOnDeployCallback(Action<Ship> onDeploy)
    {
        OnDeploy += onDeploy;
    }

    public void RegisterOnConstructionChangedCallback(Action<Ship> onConstructionChanged)
    {
        OnConstructionChanged += onConstructionChanged;
    }
}
