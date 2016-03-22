using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable()]
public class Deck {

    public Faction Faction { get; private set; }
    private List<Card> _cards;

    public Deck()
    {
        _cards = new List<Card>();

        // temp hard coded deck
        List<Shipyard> shipyards = new List<Shipyard>();
        shipyards.Add(new Shipyard(CardCodename.SHIPYARD));
        shipyards.Add(new Shipyard(CardCodename.SHIPYARD));
        shipyards.Add(new Shipyard(CardCodename.SMALL_SHIPYARD));
        Faction = new Faction("DefaultFaction", 5, 10, 5, shipyards);
        AddCard(CardType.SHIP, CardCodename.FRIGATE, 10);
        AddCard(CardType.SHIP, CardCodename.CRUISER, 10);
        AddCard(CardType.SHIP, CardCodename.BATTLESHIP, 10);
        AddCard(CardType.SHIPYARD, CardCodename.SHIPYARD, 5);
        AddCard(CardType.SHIPYARD, CardCodename.SMALL_SHIPYARD, 5);
    }

    public int GetCount()
    {
        return _cards.Count;
    }
    
    public void AddCard(CardType subtype, CardCodename CardCodename, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            switch(subtype)
            {
                case CardType.SHIP:
                    _cards.Add(new Ship(CardCodename));
                    break;
                case CardType.SHIPYARD:
                    _cards.Add(new Shipyard(CardCodename));
                    break;
                default:
                    throw new Exception("unknown card subtype");
            }
            
        }
    }    

    public Card Draw()
    {
        if(_cards.Count < 1)
        {
            Debug.Log("Tried to draw card from an empty deck");
            return null;
        }

        Card c = _cards[0];
        _cards.RemoveAt(0);
        return c;
    }

    public void Shuffle()
    {
        _cards.Shuffle();
    }
}
