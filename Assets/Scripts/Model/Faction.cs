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
    public Homeworld Homeworld { get; private set; }

    public Faction(string factionName, int startingHandSize, int startingCredits, int clicksPerTurn, List<Shipyard> shipyards, Homeworld homeworld)
    {
        FactionName = factionName;
        StartingHandSize = startingHandSize;
        StartingCredits = startingCredits;
        ClicksPerTurn = clicksPerTurn;
        Shipyards = shipyards;
        Homeworld = homeworld;
    }

    public Faction(string dataStr)
    {
        string[] data = dataStr.Split('|');

        FactionName = data[0];
        StartingHandSize = int.Parse(data[1]);
        StartingCredits = int.Parse(data[2]);
        ClicksPerTurn = int.Parse(data[3]);
        Homeworld = (Homeworld)CardFactory.CreateCard((CardCodename)Enum.Parse(typeof(CardCodename), data[4]), data[5]);
        Shipyards = new List<Shipyard>();
        int numShipyards = int.Parse(data[6]);
        for (int i = 0; i < numShipyards; i++)
        {
            Shipyard shipyard = (Shipyard)CardFactory.CreateCard((CardCodename)Enum.Parse(typeof(CardCodename), data[7 + (i * 2)]), data[8 + (i * 2)]);
            Shipyards.Add(shipyard);
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(FactionName);
        sb.Append("|");
        sb.Append(StartingHandSize);
        sb.Append("|");
        sb.Append(StartingCredits);
        sb.Append("|");
        sb.Append(ClicksPerTurn);
        sb.Append("|");
        sb.Append(Homeworld.CardCodename);
        sb.Append("|");
        sb.Append(Homeworld.CardId);
        sb.Append("|");
        sb.Append(Shipyards.Count);
        sb.Append("|");

        foreach (var shipyard in Shipyards)
        {
            sb.Append(shipyard.CardCodename);
            sb.Append("|");
            sb.Append(shipyard.CardId);
            sb.Append("|"); 
        }

        return sb.ToString();
    }
}
