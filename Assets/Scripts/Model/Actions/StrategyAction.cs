public class StrategyAction : PlayerAction
{
    public Strategy Strategy { get; private set; }

    public StrategyAction(Strategy strategy) : base(ActionType.STRATEGY)
    {
        Strategy = strategy;
    }

    public override string ToString()
    {
        return base.ToString() + "|" + Strategy.CardId + "|" + Strategy.CardCodename;

    }
}
