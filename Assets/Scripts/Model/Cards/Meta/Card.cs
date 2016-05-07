using System;

[Serializable()]
public abstract class Card {

    public string CardId { get; protected set; }
    public CardCodename CardCodename { get; protected set; }
    public string CardName { get; protected set; }

    public CardType CardType { get; protected set; } // not sure if we really need this
    public string ImageName { get; protected set; }
    public string CardText { get; protected set; }
    public int BaseCost { get; protected set; }
    public GamePhase Phase { get; protected set; } // which game state the game needs to be in to legally play this
                                                   // Use this for initialization

    public Action<Game, Player> OnPlay;
    
    public Card(CardCodename cardCodename, string cardId)
    {
        CardId = cardId;
        CardCodename = cardCodename;
        CardName = DetermineCardName();
        ImageName = DetermineImageName();
        CardText = DetermineCardText();

        BaseCost = DetermineBaseCost();
        Phase = DeterminePhase();
        OnPlay += DetermineOnPlayAction();
    }

    private Action<Game, Player> DetermineOnPlayAction()
    {
        if (CardData.OnPlayActions.ContainsKey(CardCodename))
        {
            return CardData.OnPlayActions[CardCodename];
        }
        else
        {
            return null;
            //throw new Exception(string.Format("Failed to get on play action for {0}", CardCodename));
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
            return -1; // not sure I like this 
            //throw new Exception(string.Format("Failed to get base cost for {0}", CardCodename));
        }
    }

    private GamePhase DeterminePhase()
    {
        if (CardData.Phases.ContainsKey(CardCodename))
        {
            return CardData.Phases[CardCodename];
        }
        else
        {
            return GamePhase.NONE;
            //throw new Exception(string.Format("Failed to get phase for {0}", CardCodename));
        }
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
