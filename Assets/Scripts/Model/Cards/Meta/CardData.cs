using System.Collections.Generic;

public static class CardData {
    
    // data - will prob eventually load from file
    public static Dictionary<CardCodename, CardType> CardTypes = new Dictionary<CardCodename, CardType>
    {
        { CardCodename.DEFAULT_HOMEWORLD, CardType.HOMEWORLD },
        { CardCodename.SHIPYARD, CardType.SHIPYARD },
        { CardCodename.SMALL_SHIPYARD, CardType.SHIPYARD },
        { CardCodename.BATTLESHIP, CardType.SHIP },
        { CardCodename.CRUISER, CardType.SHIP },
        { CardCodename.FRIGATE, CardType.SHIP }
    };

    public static Dictionary<CardCodename, int> BaseCosts = new Dictionary<CardCodename, int>
    {
        { CardCodename.DEFAULT_HOMEWORLD, 0 }, // doesn't really have a cost, special case, but easier to keep cost as a base Card property
        { CardCodename.SHIPYARD, 10 },
        { CardCodename.SMALL_SHIPYARD, 6 },
        { CardCodename.BATTLESHIP, 9 },
        { CardCodename.CRUISER, 6 },
        { CardCodename.FRIGATE, 3 }
    };

    public static Dictionary<CardCodename, string> CardNames = new Dictionary<CardCodename, string>
    {
        { CardCodename.DEFAULT_HOMEWORLD, "Homeworld" },
        { CardCodename.SHIPYARD, "Shipyard" },
        { CardCodename.SMALL_SHIPYARD, "Small shipyard" },
        { CardCodename.BATTLESHIP, "Beefy battleship" },
        { CardCodename.CRUISER, "Cool cruiser" },
        { CardCodename.FRIGATE, "Funky Frigate" }
    };

    public static Dictionary<CardCodename, string> ImageNames = new Dictionary<CardCodename, string>
    {
        { CardCodename.DEFAULT_HOMEWORLD, "shipyard_s" },
        { CardCodename.SHIPYARD, "shipyard_s" },
        { CardCodename.SMALL_SHIPYARD, "shipyard_s" },
        { CardCodename.BATTLESHIP, "battleship_s" },
        { CardCodename.CRUISER, "cruiser_s" },
        { CardCodename.FRIGATE, "frigate_s" }
    };

    //playable only data

    public static Dictionary<CardCodename, GameState> Phases = new Dictionary<CardCodename, GameState>
    {        
        { CardCodename.SHIPYARD, GameState.LOGISTICS_PLANNING },
        { CardCodename.SMALL_SHIPYARD, GameState.LOGISTICS_PLANNING },
        { CardCodename.BATTLESHIP, GameState.LOGISTICS_PLANNING },
        { CardCodename.CRUISER, GameState.LOGISTICS_PLANNING },
        { CardCodename.FRIGATE, GameState.LOGISTICS_PLANNING }
    };

    // ship only data 
    public static Dictionary<CardCodename, int> Sizes = new Dictionary<CardCodename, int>
    {
        { CardCodename.BATTLESHIP, 10},
        { CardCodename.CRUISER, 5 },
        { CardCodename.FRIGATE, 3 }
    };

    public static Dictionary<CardCodename, int> MaxHealths = new Dictionary<CardCodename, int>
    {
        { CardCodename.DEFAULT_HOMEWORLD, 50 },
        { CardCodename.SHIPYARD, 30 },
        { CardCodename.SMALL_SHIPYARD, 20 },
        { CardCodename.BATTLESHIP, 12 },
        { CardCodename.CRUISER, 7 },
        { CardCodename.FRIGATE, 2 }
    };

    public static Dictionary<CardCodename, List<Weapon>> Weapons = new Dictionary<CardCodename, List<Weapon>>
    {
        { CardCodename.BATTLESHIP, new List<Weapon> { { new Weapon(WeaponType.LASER, 2)}, { new Weapon(WeaponType.LASER, 2) }, { new Weapon(WeaponType.MISSILE, 3) }  } },
        { CardCodename.CRUISER, new List<Weapon>() { new Weapon(WeaponType.MISSILE, 3) } },
        { CardCodename.FRIGATE, new List<Weapon>() { new Weapon(WeaponType.LASER, 1) } }
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
