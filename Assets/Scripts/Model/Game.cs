using UnityEngine;
using System.Collections.Generic;

public class Game  {

    // NOTE - only the server sees this comprehansive version of the game state - the players view of the game is held in GameClientController

    public int GameNumber { get; private set; }
    public Player Player { get; private set; } 
    public Player Opponent { get; private set; }
    public GameState GameState { get; private set; }
    public int GameTurn { get; private set; }
    public string HostActionData { get; private set; }
    public string ChallengerActionData { get; private set; }

    private bool _hostReady;
    private bool _challengerReady;
    
    public Game(int gameNumber, Player player)
    {
        GameNumber = gameNumber;
        Player = player;
        GameState = GameState.AWAITING_CHALLENGER;
    }

    public bool AddOpponent(Player opponent)
    {
        if (Opponent != null || GameState != GameState.AWAITING_CHALLENGER)
        {
            return false;
        }
        Opponent = opponent;
        GameState = GameState.SETUP;

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
        GameState = GameState.WAITING_FOR_OPPONENT;
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
