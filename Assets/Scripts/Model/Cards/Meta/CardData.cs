using System;
using System.Collections.Generic;

public static class CardData {
    
    // data - will prob eventually load from file
    public static Dictionary<CardCodename, CardType> CardTypes = new Dictionary<CardCodename, CardType>
    {
        { CardCodename.UNKNOWN, CardType.UNKNOWN },
        { CardCodename.DEFAULT_HOMEWORLD, CardType.HOMEWORLD },
        { CardCodename.SHIPYARD, CardType.SHIPYARD },
        { CardCodename.SMALL_SHIPYARD, CardType.SHIPYARD },
        { CardCodename.BATTLESHIP, CardType.SHIP },
        { CardCodename.CRUISER, CardType.SHIP },
        { CardCodename.FRIGATE, CardType.SHIP },
        { CardCodename.SHORT_TERM_INVESTMENT, CardType.OPERATION },
        { CardCodename.LONG_TERM_INVESTMENT, CardType.OPERATION },
        { CardCodename.EFFICIENCY_DRIVE, CardType.OPERATION }
    };


    public static Dictionary<CardCodename, string> CardNames = new Dictionary<CardCodename, string>
    {
        { CardCodename.UNKNOWN, "Unknown" },
        { CardCodename.DEFAULT_HOMEWORLD, "Homeworld" },
        { CardCodename.SHIPYARD, "Shipyard" },
        { CardCodename.SMALL_SHIPYARD, "Small shipyard" },
        { CardCodename.BATTLESHIP, "Beefy battleship" },
        { CardCodename.CRUISER, "Cool cruiser" },
        { CardCodename.FRIGATE, "Funky Frigate" },
        { CardCodename.SHORT_TERM_INVESTMENT, "Short term investment" },
        { CardCodename.LONG_TERM_INVESTMENT, "Long term investment" },
        { CardCodename.EFFICIENCY_DRIVE, "Efficiency drive" }
    };

    public static Dictionary<CardCodename, string> ImageNames = new Dictionary<CardCodename, string>
    {
        { CardCodename.UNKNOWN, "card" },
        { CardCodename.DEFAULT_HOMEWORLD, "card" },
        { CardCodename.SHIPYARD, "shipyard_s" },
        { CardCodename.SMALL_SHIPYARD, "shipyard_s" },
        { CardCodename.BATTLESHIP, "battleship_s" },
        { CardCodename.CRUISER, "cruiser_s" },
        { CardCodename.FRIGATE, "frigate_s" },
        { CardCodename.SHORT_TERM_INVESTMENT, "card" },
        { CardCodename.LONG_TERM_INVESTMENT, "card" },
        { CardCodename.EFFICIENCY_DRIVE, "card" }
    };

    public static Dictionary<CardCodename, string> CardTexts = new Dictionary<CardCodename, string>
    {
        { CardCodename.SHORT_TERM_INVESTMENT, "Gain 9 credits" },
        { CardCodename.LONG_TERM_INVESTMENT, "Gain 1 credit at the start of each turn" },
        { CardCodename.EFFICIENCY_DRIVE, "For the rest of the turn, when you click for a credit, gain an additional credit" }
    };

    //playable only data
    public static Dictionary<CardCodename, int> BaseCosts = new Dictionary<CardCodename, int>
    {
        { CardCodename.UNKNOWN, 99999 },
        { CardCodename.SHIPYARD, 10 },
        { CardCodename.SMALL_SHIPYARD, 6 },
        { CardCodename.BATTLESHIP, 5 },
        { CardCodename.CRUISER, 4 },
        { CardCodename.FRIGATE, 3 },
        { CardCodename.SHORT_TERM_INVESTMENT, 5 },
        { CardCodename.LONG_TERM_INVESTMENT, 5 },
        { CardCodename.EFFICIENCY_DRIVE, 0 }
    };

    public static Dictionary<CardCodename, GamePhase> Phases = new Dictionary<CardCodename, GamePhase>
    {
        { CardCodename.UNKNOWN, GamePhase.SETUP },  
        { CardCodename.SHIPYARD, GamePhase.LOGISTICS_PLANNING },
        { CardCodename.SMALL_SHIPYARD, GamePhase.LOGISTICS_PLANNING },
        { CardCodename.BATTLESHIP, GamePhase.LOGISTICS_PLANNING },
        { CardCodename.CRUISER, GamePhase.LOGISTICS_PLANNING },
        { CardCodename.FRIGATE, GamePhase.LOGISTICS_PLANNING },
        { CardCodename.SHORT_TERM_INVESTMENT, GamePhase.LOGISTICS_PLANNING },
        { CardCodename.LONG_TERM_INVESTMENT, GamePhase.LOGISTICS_PLANNING },
        { CardCodename.EFFICIENCY_DRIVE, GamePhase.LOGISTICS_PLANNING }
    };

    public static Dictionary<CardCodename, Action<Game, Player>> OnPlayActions = new Dictionary<CardCodename, Action<Game, Player>>
    {
        { CardCodename.SHORT_TERM_INVESTMENT, new Action<Game, Player>(OnPlayAction.ShortTermInvestment) }
    };         

    // ship only data 
    public static Dictionary<CardCodename, int> Sizes = new Dictionary<CardCodename, int>
    {
        { CardCodename.BATTLESHIP, 3},
        { CardCodename.CRUISER, 2 },
        { CardCodename.FRIGATE, 1 }
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
        { CardCodename.BATTLESHIP, new List<Weapon> { { new Weapon(WeaponType.LASER, 2)}, { new Weapon(WeaponType.LASER, 3) }, { new Weapon(WeaponType.MISSILE, 3) }  } },
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

    // operation only data
    public static Dictionary<CardCodename, OperationType> OperationTypes = new Dictionary<CardCodename, OperationType>
    {
        { CardCodename.SHORT_TERM_INVESTMENT, OperationType.ONESHOT},
        { CardCodename.LONG_TERM_INVESTMENT, OperationType.ONGOING},
        { CardCodename.EFFICIENCY_DRIVE, OperationType.TURN }
    };
}
