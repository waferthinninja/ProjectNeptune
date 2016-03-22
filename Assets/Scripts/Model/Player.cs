﻿using UnityEngine;
using System.Collections.Generic;

public class Player {

    public string Name { get; protected set; }
    //private Deck _deck; // this is the "clean" version of the deck     
    public int ConnectionId { get; protected set; }

    // in game 
    public Deck Deck { get; private set; } // this is the current deck used during the game itself 
    public int Credits { get; private set;  }
    public int Clicks { get; private set; }
    public List<Card> Hand { get; private set; }
    public List<Card> Discard { get; private set; }
    public List<Card> Shipyards { get; private set; }
    public List<Card> Ships { get; private set; }

    public Player(string name, int connectionId)
    {
        Name = name;
        ConnectionId = connectionId;
        // temp hard coded deck
        Deck = new Deck();        
    }

    public void SetDeck(Deck deck)
    {
        Deck = deck;
    }

    public void ChangeClicks(int change)
    {
        Clicks += change;
        // cap at 0
        if (Clicks < 0)
        {
            Clicks = 0;
        }
    }

    public void ChangeCredits(int change)
    {
        Credits += change;
        // cap at 0
        if (Credits < 0)
        {
            Credits = 0;
        }
    } 

    public void Setup()
    {
        //Deck = _deck.Clone();
        Deck.Shuffle();

        // set starting credits
        Credits = Deck.Faction.StartingCredits;

        // set starting clicks
        Clicks = Deck.Faction.ClicksPerTurn;

        // create starting shipyards
        Shipyards = new List<Card>();
        for (int i = 0; i < Deck.Faction.Shipyards.Count; i++)
        {
            Shipyard shipyard = Deck.Faction.Shipyards[i];
            Shipyards.Add(shipyard);
        }

        // initialize lists
        Ships = new List<Card>();

        // draw starting hand
        Hand = new List<Card>();
        Discard = new List<Card>();
        for (int i = 0; i < Deck.Faction.StartingHandSize; i++)
        {
            Hand.Add(Deck.Draw());
        }        
    }
}
