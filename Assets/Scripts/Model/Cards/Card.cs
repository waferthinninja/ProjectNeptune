using System;
using System.Collections.Generic;

[Serializable()]
public abstract class Card {

    public string CardId { get; protected set; }
    public CardCodename CardCodename { get; protected set; }
    public string CardName { get; protected set; }
    public int BaseCost { get; protected set; }
    public GameState Phase { get; protected set; } // which game state the game needs to be in to legally play this
    public CardType CardType { get; protected set; } // not sure if we really need this
    
    public Card(CardCodename cardCodename, string cardId)
    {        
        CardId = cardId;
        CardCodename = cardCodename;
        CardName = DetermineCardName();
        BaseCost = DetermineBaseCost();
    }

    private string DetermineCardName()
    {
        if (CardData.CardNames.ContainsKey(CardCodename) )
        {
            return CardData.CardNames[CardCodename];
        }
        else
        {
            throw new Exception(string.Format("Failed to get card name for {0}", CardCodename));
        }
    }

    private int DetermineBaseCost()
    {
        if (CardData.BaseCosts.ContainsKey(CardCodename))
        {
            return CardData.BaseCosts[CardCodename];
        }
        else
        {
            throw new Exception(string.Format("Failed to get base cost for {0}", CardCodename));
        }
    }

    private GameState DeterminePhase()
    {
        switch (CardCodename)
        {
            case CardCodename.FRIGATE:
            case CardCodename.CRUISER:
            case CardCodename.BATTLESHIP:
                return GameState.LOGISTICS_PLANNING;
            default:
                throw new Exception("Unknown card type");
        }
    }





}
