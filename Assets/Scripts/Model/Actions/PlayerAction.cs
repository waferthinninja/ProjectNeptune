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
