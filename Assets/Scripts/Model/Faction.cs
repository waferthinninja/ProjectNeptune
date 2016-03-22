using System.Collections.Generic;
using System.Text;
using System;

[Serializable()]
public class Faction {

    public string FactionName { get; private set; }
    public int StartingHandSize { get; private set; }
    public int StartingCredits { get; private set; }
    public int ClicksPerTurn { get; private set; }
    public List<Shipyard> Shipyards { get; private set; }

    public Faction(string factionName, int startingHandSize, int startingCredits, int clicksPerTurn, List<Shipyard> shipyards)
    {
        FactionName = factionName;
        StartingHandSize = startingHandSize;
        StartingCredits = startingCredits;
        ClicksPerTurn = clicksPerTurn;
        Shipyards = shipyards;
    }
}
