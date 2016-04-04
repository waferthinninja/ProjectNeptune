
public class DeployAction : PlayerAction {

    public Ship Ship { get; private set; }
    public Shipyard Shipyard { get; private set; }

    public DeployAction(Ship ship, Shipyard shipyard) : base(ActionType.DEPLOY_SHIP)
    {
        Ship = ship;
        Shipyard = shipyard;
    }

    public override string ToString()
    {
        return base.ToString() + "|" + Ship.CardId + "|" + Shipyard.CardId;
    }
}
