using UnityEngine;
using System.Collections.Generic;

public class Game  {

    // NOTE - only the server sees this comprehansive version of the game state - the players view of the game is held in GameClientController

    public int GameNumber { get; private set; }
    public Player Player { get; private set; } 
    public Player Opponent { get; private set; }
    public GamePhase GamePhase { get; private set; }
    public int GameTurn { get; private set; }
    public string HostActionData { get; private set; }
    public string ChallengerActionData { get; private set; }

    private bool _waitingForOpponent; // this replaces GamePhase.WAITING_FOR_OPPONENT, we instead want to keep the correct game phase in GameState

    private bool _hostReady;
    private bool _challengerReady;
    
    public Game(int gameNumber, Player player)
    {
        GameNumber = gameNumber;
        Player = player;
        GamePhase = GamePhase.AWAITING_CHALLENGER;
    }

    public bool AddOpponent(Player opponent)
    {
        if (Opponent != null || GamePhase != GamePhase.AWAITING_CHALLENGER)
        {
            return false;
        }
        Opponent = opponent;
        GamePhase = GamePhase.SETUP;

        return true;
    }

    public void PlayerReady()
    {
        _hostReady = true;
    }

    public void OpponentReady()
    {
        _challengerReady = true;
    }
    
    public bool BothPlayersReady()
    {
        return _hostReady && _challengerReady;
    }

    public void Setup()
    {
        // setup the deck of each player
        Player.Setup();
        Opponent.Setup();

        // start the first turn
        StartNewTurn();
    }

    public void StartNewTurn()
    {
        GameTurn++;
        StartLogisticsPlanningPhase();
    }

    public void AwaitOpponent()
    {
        _waitingForOpponent = true;
    }

    public bool IsAwaitingOpponent()
    {
        return _waitingForOpponent;
    }

    public void StartLogisticsPlanningPhase()
    {
        ChangeGamePhase(GamePhase.LOGISTICS_PLANNING);
        HostActionData = "NONE";
        ChallengerActionData = "NONE";
    }

    public void StartLogisticsResolutionPhase()
    {
        ChangeGamePhase(GamePhase.LOGISTICS_RESOLUTION);
    }

    public void StartCombatPlanningPhase()
    {
        ChangeGamePhase(GamePhase.COMBAT_PLANNING);
        HostActionData = "NONE";
        ChallengerActionData = "NONE";
    }

    public void StartLaserWeaponResolutionPhase()
    {
        ChangeGamePhase(GamePhase.LASER_WEAPON_RESOLUTION);
    }

    private void ChangeGamePhase(GamePhase gamePhase)
    {
        GamePhase = gamePhase;
        _waitingForOpponent = false;
    }

    public void SubmitHostActions(string data)
    {
        HostActionData = data;
    }

    public void SubmitChallengerActions(string data)
    {
        ChallengerActionData = data;
    }

    public bool BothActionsSubmitted()
    {
        return HostActionData != "NONE" && ChallengerActionData != "NONE";
    }

    public Player GetOpponent(Player player)
    {
        if (player == Player)
        {
            return Opponent;
        }
        else if (player == Opponent)
        {
            return Player;
        }
        else
        {
            throw new System.Exception("Invalid player for this game");
        }
    }

    
}
