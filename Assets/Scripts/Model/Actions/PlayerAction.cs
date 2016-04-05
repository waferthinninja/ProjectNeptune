// base class for player actions
// not abstract as can have simple actions where all we want is the type e.g. click for credit
public class PlayerAction {

	public ActionType Type { get; private set; }

    public PlayerAction(ActionType type)
    {
        Type = type;
    }

    public override string ToString()
    {
        return Type.ToString();
    }
}
