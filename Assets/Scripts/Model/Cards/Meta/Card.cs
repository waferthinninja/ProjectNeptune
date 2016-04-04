using System;
using System.Collections.Generic;

[Serializable()]
public abstract class Card {

    public string CardId { get; protected set; }
    public CardCodename CardCodename { get; protected set; }
    public string CardName { get; protected set; }

    public CardType CardType { get; protected set; } // not sure if we really need this
    public string ImageName { get; protected set; }
    public string CardText { get; protected set; }

    public Card(CardCodename cardCodename, string cardId)
    {        
        CardId = cardId;
        CardCodename = cardCodename;
        CardName = DetermineCardName();
        ImageName = DetermineImageName();
        CardText = DetermineCardText();
    }

    private string DetermineCardText()
    {
        if (CardData.CardTexts.ContainsKey(CardCodename))
        {
            return CardData.CardTexts[CardCodename];
        }
        else
        {
            return "";
            //throw new Exception(string.Format("Failed to get card text for {0}", CardCodename));
        }
    }

    private string DetermineImageName()
    {
        if (CardData.ImageNames.ContainsKey(CardCodename))
        {
            return CardData.ImageNames[CardCodename];
        }
        else
        {
            throw new Exception(string.Format("Failed to get image name for {0}", CardCodename));
        }
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

}
