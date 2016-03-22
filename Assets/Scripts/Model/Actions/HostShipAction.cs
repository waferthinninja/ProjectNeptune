public class HostShipAction : Action {

    public Ship Ship { get; private set; }
    public Shipyard Shipyard { get; private set; }

	public HostShipAction(Ship ship, Shipyard shipyard) : base(ActionType.HOST_SHIP)
    {
        Ship = ship;
        Shipyard = shipyard;
    }

    public override string ToString()
    {
        return base.ToString() + "|" + Ship.CardId + "|" + Shipyard.CardId;
    }
}
