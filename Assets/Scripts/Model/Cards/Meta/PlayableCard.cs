using System;

public abstract class PlayableCard : Card {

    public int BaseCost { get; protected set; }
    public GameState Phase { get; protected set; } // which game state the game needs to be in to legally play this
                                                   // Use this for initialization

    public PlayableCard(CardCodename codename, string cardId) : base(codename, cardId)
    {
        BaseCost = DetermineBaseCost();
        Phase = DeterminePhase();
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
