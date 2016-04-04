public class AdvanceConstructionAction : PlayerAction {

    public Ship Ship { get; private set; }
    public Shipyard Shipyard { get; private set; }

    public AdvanceConstructionAction(Ship ship, Shipyard shipyard) : base(ActionType.ADVANCE_CONSTRUCTION)
    {
        Ship = ship;
        Shipyard = shipyard;
    }

    public override string ToString()
    {
        return base.ToString() + "|" + Ship.CardId + "|" + Shipyard.CardId;
    }
}
