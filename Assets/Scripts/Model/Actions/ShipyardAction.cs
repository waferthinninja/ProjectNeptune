
public class ShipyardAction : PlayerAction {

    public Shipyard Shipyard { get; private set;  }

    public ShipyardAction(Shipyard shipyard) : base(ActionType.SHIPYARD)
    {
        Shipyard = shipyard;
    }

    public override string ToString()
    {
        return base.ToString() + "|" + Shipyard.CardId + "|" + Shipyard.CardCodename;
    }
}
