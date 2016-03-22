public class AdvanceConstructionAction : Action {

    public Ship Ship { get; private set; }

    public AdvanceConstructionAction(Ship ship) : base(ActionType.ADVANCE_CONSTRUCTION)
    {
        Ship = ship;
    }

    public override string ToString()
    {
        return base.ToString() + "|" + Ship.CardId;
    }
}
