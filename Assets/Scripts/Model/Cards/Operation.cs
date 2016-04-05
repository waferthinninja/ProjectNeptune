using System;

public class Operation : PlayableCard {

    public OperationType OperationType { get; private set;  }

    public Operation(CardCodename codename) : this (codename, Guid.NewGuid().ToString())
    {
    }

    public Operation(CardCodename codename, string cardId) : base (codename, cardId)
    {
        CardType = CardType.OPERATION;
        OperationType = DetermineOperationType();
    }
    
    private OperationType DetermineOperationType()
    {
        if (CardData.OperationTypes.ContainsKey(CardCodename))
        {
            return CardData.OperationTypes[CardCodename];
        }
        else
        {
            throw new Exception(string.Format("Failed to get OperationType for {0}", CardCodename));
        }
    }
}
