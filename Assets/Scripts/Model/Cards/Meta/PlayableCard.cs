using System;

public abstract class PlayableCard : Card {

    public int BaseCost { get; protected set; }
    public GamePhase Phase { get; protected set; } // which game state the game needs to be in to legally play this
                                                   // Use this for initialization

    public Action<Game, Player> OnPlay;

    public PlayableCard(CardCodename codename, string cardId) : base(codename, cardId)
    {
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
            throw new Exception(string.Format("Failed to get base cost for {0}", CardCodename));
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
            throw new Exception(string.Format("Failed to get phase for {0}", CardCodename));
        }
    }

}
