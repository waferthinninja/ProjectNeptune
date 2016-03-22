using UnityEngine;
using System.Collections.Generic;

public class Game  {

    // NOTE - only the server sees this comprehansive version of the game state - the players view of the game is held in GameClientController

    public int GameNumber { get; private set; }
    public Player Host { get; private set; } // player who created the game - they aren't really "hosting" in a network sense, just a way to distinguish the players
    public Player Challenger { get; private set; }
    public GameState GameState { get; private set; }
    public int GameTurn { get; private set; }
    public string HostActionData { get; private set; }
    public string ChallengerActionData { get; private set; }

    private bool _hostReady;
    private bool _challengerReady;
    
    public Game(int gameNumber, Player host)
    {
        GameNumber = gameNumber;
        Host = host;
        GameState = GameState.AWAITING_CHALLENGER;
    }

    public bool AddChallenger(Player challenger)
    {
        if (Challenger != null || GameState != GameState.AWAITING_CHALLENGER)
        {
            return false;
        }
        Challenger = challenger;
        GameState = GameState.SETUP;

        return true;
    }

    public void HostReady()
    {
        _hostReady = true;
    }

    public void ChallengerReady()
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
        Host.Setup();
        Challenger.Setup();

        // start the first turn
        StartNewTurn();
    }

    public void StartNewTurn()
    {
        GameTurn++;
        StartLogisticsPlanningPhase();
    }

    public void StartLogisticsPlanningPhase()
    {
        GameState = GameState.LOGISTICS_PLANNING;
        HostActionData = "NONE";
        ChallengerActionData = "NONE";
    }

    public void StartLogisticsResolutionPhase()
    {
        GameState = GameState.LOGISTICS_RESOLUTION;
    }

    public void StartCombatPlanningPhase()
    {
        GameState = GameState.COMBAT_PLANNING;
        HostActionData = "NONE";
        ChallengerActionData = "NONE";
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
        if (player == Host)
        {
            return Challenger;
        }
        else if (player == Challenger)
        {
            return Host;
        }
        else
        {
            throw new System.Exception("Invalid player for this game");
        }
    }
}
