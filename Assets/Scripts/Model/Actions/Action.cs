public class Action {

	public ActionType Type { get; private set; }

    public Action(ActionType type)
    {
        Type = type;
    }

    public override string ToString()
    {
        return Type.ToString();
    }


}
