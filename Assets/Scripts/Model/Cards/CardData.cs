using System.Collections.Generic;

public static class CardData  {

    // data - will prob eventually load from file
    public static Dictionary<CardCodename, CardType> CardTypes = new Dictionary<CardCodename, CardType>
    {
        { CardCodename.SHIPYARD, CardType.SHIPYARD },
        { CardCodename.SMALL_SHIPYARD, CardType.SHIPYARD },
        { CardCodename.BATTLESHIP, CardType.SHIP },
        { CardCodename.CRUISER, CardType.SHIP },
        { CardCodename.FRIGATE, CardType.SHIP }
    };

    public static Dictionary<CardCodename, int> BaseCosts = new Dictionary<CardCodename, int>
    {
        { CardCodename.SHIPYARD, 10 },
        { CardCodename.SMALL_SHIPYARD, 6 },
        { CardCodename.BATTLESHIP, 9 },
        { CardCodename.CRUISER, 6 },
        { CardCodename.FRIGATE, 3 }
    };

    public static Dictionary<CardCodename, string> CardNames = new Dictionary<CardCodename, string>
    {
        { CardCodename.SHIPYARD, "Shipyard" },
        { CardCodename.SMALL_SHIPYARD, "Small shipyard" },
        { CardCodename.BATTLESHIP, "Beefy battleship" },
        { CardCodename.CRUISER, "Cool cruiser" },
        { CardCodename.FRIGATE, "Funky Frigate" }
    };

    // ship only data 
    public static Dictionary<CardCodename, int> Sizes = new Dictionary<CardCodename, int>
    {
        { CardCodename.BATTLESHIP, 10},
        { CardCodename.CRUISER, 5 },
        { CardCodename.FRIGATE, 3 }
    };

    // shipyard only data
    public static Dictionary<CardCodename, int> Efficiencies = new Dictionary<CardCodename, int>
    {
        { CardCodename.SHIPYARD, 3},
        { CardCodename.SMALL_SHIPYARD, 3 }
    };

    public static Dictionary<CardCodename, int> MaxSizes = new Dictionary<CardCodename, int>
    {
        { CardCodename.SHIPYARD, 10},
        { CardCodename.SMALL_SHIPYARD, 5 }
    };
}
