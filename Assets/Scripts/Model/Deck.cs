using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;

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
        shipyards.Add((Shipyard)CardFactory.CreateCard(CardCodename.SMALL_SHIPYARD));
        Faction = new Faction("DefaultFaction", 8, 100, 10, shipyards, homeworld);
        AddCard(CardCodename.FRIGATE, 4);
        AddCard(CardCodename.CRUISER, 4);
        AddCard(CardCodename.BATTLESHIP, 4);
        //AddCard(CardCodename.SHIPYARD, 4);
        AddCard(CardCodename.SMALL_SHIPYARD, 4);
        AddCard(CardCodename.SHORT_TERM_INVESTMENT, 4);
        //AddCard(CardCodename.LONG_TERM_INVESTMENT, 4);
        //AddCard(CardCodename.EFFICIENCY_DRIVE, 4);
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

    public Deck(string dataStr)
    {
        string[] data = dataStr.Split('#');
        Faction = new Faction(data[0]);

        _cards = new List<PlayableCard>();
        string[] cardData = data[1].Split('|');
        int numCards = int.Parse(cardData[0]);

        for (int i = 0; i < numCards; i++)
        {
            PlayableCard card = (PlayableCard)CardFactory.CreateCard((CardCodename)Enum.Parse(typeof(CardCodename), cardData[1 + (i * 2)]), cardData[2 + (i * 2)]);
            _cards.Add(card);
        }
    }

    public override string ToString()
    {
        return ToString(false);
    }

    public string ToString(bool anonymize)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(Faction.ToString());
        sb.Append("#");

        sb.Append(_cards.Count);
        sb.Append("|");

        foreach (Card card in _cards)
        {
            sb.Append(anonymize ? CardCodename.UNKNOWN : card.CardCodename);
            sb.Append("|");
            sb.Append(card.CardId);
            sb.Append("|");
        }

        return sb.ToString();
    }
}
