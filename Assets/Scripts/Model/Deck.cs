using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable()]
public class Deck {

    public Faction Faction { get; private set; }
    private List<PlayableCard> _cards;

    public Deck()
    {
        _cards = new List<PlayableCard>();

        // temp hard coded deck
        Homeworld homeworld = (Homeworld)CardFactory.CreateCard(CardCodename.DEFAULT_HOMEWORLD);
        List<Shipyard> shipyards = new List<Shipyard>();
        shipyards.Add((Shipyard)CardFactory.CreateCard(CardCodename.SHIPYARD));
        shipyards.Add((Shipyard)CardFactory.CreateCard(CardCodename.SHIPYARD));
        shipyards.Add((Shipyard)CardFactory.CreateCard(CardCodename.SMALL_SHIPYARD));
        Faction = new Faction("DefaultFaction", 5, 10, 5, shipyards, homeworld);
        AddCard(CardCodename.FRIGATE, 10);
        AddCard(CardCodename.CRUISER, 10);
        AddCard(CardCodename.BATTLESHIP, 10);
        AddCard(CardCodename.SHIPYARD, 5);
        AddCard(CardCodename.SMALL_SHIPYARD, 5);
    }

    public int GetCount()
    {
        return _cards.Count;
    }
    
    public void AddCard(CardCodename CardCodename, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            _cards.Add((PlayableCard)CardFactory.CreateCard(CardCodename));            
        }
    }    

    public PlayableCard Draw()
    {
        if(_cards.Count < 1)
        {
            Debug.Log("Tried to draw card from an empty deck");
            return null;
        }

        PlayableCard c = _cards[0];
        _cards.RemoveAt(0);
        return c;
    }

    public void Shuffle()
    {
        _cards.Shuffle();
    }
}
