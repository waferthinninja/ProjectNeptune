public class HostShipAction : PlayerAction {

    public Ship Ship { get; private set; }
    public Shipyard Shipyard { get; private set; }

	public HostShipAction(Ship ship, Shipyard shipyard) : base(ActionType.HOST_SHIP)
    {
        Ship = ship;
        Shipyard = shipyard;
    }

    public override string ToString()
    {
        return base.ToString() + "|" + Ship.CardId + "|" + Shipyard.CardId + "|" + Ship.CardCodename; // card codename wont really be needed by server, but the opponent
                                                                                                      // will need to instantiate it, this way we can just pass on the
                                                                                                      // action unchanged. Kind of inefficient in terms of network traffic
                                                                                                      // but not a big deal
    }
}
