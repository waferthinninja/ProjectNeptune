
public class OperationAction : PlayerAction {

    public Operation Operation { get; private set;  }

	public OperationAction(Operation operation) : base(ActionType.OPERATION)
    {
        Operation = operation;
    }

    public override string ToString()
    {
        return base.ToString() + "|" + Operation.CardId + "|" + Operation.CardCodename ;

    }
}
