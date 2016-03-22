using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;

public class GameServerController : NetworkBehaviour {

    public GameObject _lobbyController;

    public override void OnStartServer()
    {
        Debug.Log("GameServerController OnStartServer");
        base.OnStartServer(); //base is empty
        
        NetworkServer.RegisterHandler((short)MessageTypes.MessageType.PLAYER_READY, OnPlayerReadyMessage);
        NetworkServer.RegisterHandler((short)MessageTypes.MessageType.SUBMIT_ACTIONS, OnSubmitActionsMessage);

    }

    void OnSubmitActionsMessage(NetworkMessage netMsg)
    {
        // determine player and game from connection
        int connectionId = netMsg.conn.connectionId;
        Player player = GetPlayerFromConnection(connectionId);
        Game game = GetGameFromConnection(connectionId);

        // read the message
        var msg = netMsg.ReadMessage<MessageTypes.SubmitActionsMessage>();
        
        // store in the game object
        if (game.Host == player)
        {
            game.SubmitHostActions(msg.actionData);
            Debug.Log(string.Format("Game {0} - host {1} has submitted actions", game.GameNumber, game.Host.Name));
        }

        if (game.Challenger == player)
        {
            game.SubmitChallengerActions(msg.actionData);
            Debug.Log(string.Format("Game {0} - challenger {1} has submitted actions", game.GameNumber, game.Challenger.Name));
        }
        
        if (game.BothActionsSubmitted())
        {
            // if both players now submitted, process actions, advance the game state
            ProcessActions(game);
            game.StartLogisticsResolutionPhase();
            SendGameStateToPlayers(game);

            // advance the game to the combat planning stage 
            // dont send this to clients, they will advance themselves after 
            // replaying resolution
            game.StartCombatPlanningPhase();
        }
        else
        {
            // if this is the first player to submit, send them a "wait for opponent message"
            PutPlayerInWaitMode(player);
        }
    }    

    private void ProcessActions(Game game)
    {
        ProcessActionsForPlayer(game, game.HostActionData, game.Host, game.Challenger);
        ProcessActionsForPlayer(game, game.ChallengerActionData, game.Challenger, game.Host);
    }

    private void ProcessActionsForPlayer(Game game, string actionData, Player player, Player opponent)
    {
        string[] data = actionData.Split('#');
        for (int i = 0; i < data.Length; i++)
        {
            ProcessAction(data[i], game, player, opponent);
        }

    }

    private void PutPlayerInWaitMode(Player player)
    {
        var stateMsg = new MessageTypes.GameStateMessage();
        stateMsg.state = GameState.WAITING_FOR_OPPONENT.ToString();
        NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.GAME_STATE, stateMsg);
    }

    private void SendGameStateToPlayers(Game game)
    {
        SendGameStateToPlayer(game, game.Host, game.Challenger);
        SendGameStateToPlayer(game, game.Challenger, game.Host);
    }

    private void SendGameStateToPlayer(Game game, Player player, Player opponent)
    {
        // send basic opponent state
        var opponentMsg = new MessageTypes.OpponentStateMessage();
        opponentMsg.credits = opponent.Credits;
        opponentMsg.clicks = opponent.Clicks;
        opponentMsg.cardsInHand = opponent.Hand.Count;
        opponentMsg.cardsInDeck = opponent.Deck.GetCount();
        opponentMsg.cardsInDiscard = opponent.Discard.Count;
        NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.OPPONENT_STATE, opponentMsg);

        var stateMsg = new MessageTypes.GameStateMessage();
        stateMsg.state = game.GameState.ToString();
        NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.GAME_STATE, stateMsg);
    }

    private void ProcessAction(string action, Game game, Player player, Player opponent)
    {
        string[] actionData = action.Split('|');
        ActionType actionType = (ActionType)Enum.Parse(typeof(ActionType), actionData[0]);
        switch (actionType)
        {
            case ActionType.CLICK_FOR_CARD:
                ProcessClickForCardAction(player, game);
                break;
            case ActionType.CLICK_FOR_CREDIT:
                ProcessClickForCreditAction(player, game);
                break;
            case ActionType.HOST_SHIP:
                string cardId = actionData[1];
                string shipyardId = actionData[2];
                ProcessHostShipAction(player, opponent, game, cardId, shipyardId);
                break;
            case ActionType.DEPLOY_SHIP:
                cardId = actionData[1];
                shipyardId = actionData[2];
                ProcessDeployShipAction(player, opponent, game, cardId, shipyardId);
                break;
            default:
                Debug.LogError("Unknown action type [" + actionType + "]");
                break;
        }
    }

    private void ProcessDeployShipAction(Player player, Player opponent, Game game, string shipId, string shipyardId)
    {        
        Shipyard shipyard = (Shipyard)FindCardByGuid(shipyardId, player.Shipyards);//player.Shipyards.Find(x => x.ShipyardId == shipyardId);

        // TODO - error if shipyard not found

        Ship ship = shipyard.HostedShip;

        // TODO - error if this is not the correct ship

        // TODO - error if not complete

        shipyard.ClearHostedCard();

        player.Ships.Add(ship);
    }

    void ProcessHostShipAction(Player player, Player opponent, Game game, string shipId, string shipyardId)
    {
        // find card and shipyard by their id
        Ship ship = (Ship)FindCardByGuid(shipId, player.Hand);//player.Hand.Find(x => x.CardId == cardId);

        // TODO - error if card not in hand
        
        Shipyard shipyard = (Shipyard)FindCardByGuid(shipyardId, player.Shipyards);//player.Shipyards.Find(x => x.ShipyardId == shipyardId);

        // TODO - error if shipyard not found

        Debug.Log(string.Format("Trying to host {0}({1}) on {2}({3}) for {4}", ship.CardName, ship.CardId, shipyard.CardName, shipyard.CardId, player.Name));        
        if (TryHost(player, ship, shipyard) == true)
        {
            // inform the opponent to update their GUI
            var msg = new MessageTypes.HostShipMessage();
            msg.CardCodename = ship.CardCodename.ToString();
            msg.cardId = shipId;
            msg.shipyardId = shipyardId;
            NetworkServer.SendToClient(opponent.ConnectionId, (short)MessageTypes.MessageType.HOST_SHIP, msg);

            // and game log
            SendGameLogToPlayer(opponent, string.Format("{0} hosts {1} on {2}", player.Name, ship.CardName, shipyard.CardName));
        }
        else
        {
            Debug.Log(string.Format("Failed trying to host {0}({1}) on {2}({3}) for {4}", ship.CardName, ship.CardId, shipyard.CardName, shipyard.CardId, player.Name));
        }
    }

    private void ChangeCredits(Player player, int change)
    {
        // update the credits
        player.ChangeCredits(change);

        // inform the player of their new total
        var creditsMsg = new MessageTypes.CreditsMessage();
        creditsMsg.credits = player.Credits;
        NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.CREDITS, creditsMsg);
    }

    private void ChangeClicks(Player player, int change)
    {
        // update the clicks
        player.ChangeClicks(change);

        // inform the player of the new total
        var clicksMsg = new MessageTypes.ClicksMessage();
        clicksMsg.clicks = player.Clicks;
        NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.CLICKS, clicksMsg);
    }

    private Card FindCardByGuid(string cardId, List<Card> findIn)
    {
        Card card = null;
        for (int i = 0; i < findIn.Count; i++)
        {
            //Debug.Log(string.Format("Comparing {0} to {1}", findIn[i].CardId, cardId));
            if (findIn[i].CardId == cardId)
            {
                card = findIn[i];
            }
        }

        return card;
    }

    public bool TryHost(Player player, Ship shipCard, Shipyard shipyard)
    {
        Debug.Log(string.Format("TryHost player{0}, shipCard{1}, shipyard{2}", player.Name, shipCard.CardId, shipyard.CardId));
        // can only host one card
        if (shipyard.HostedShip != null)
            return false;

        // can only host ships up to a certain size
        if (shipCard.Size > shipyard.MaxSize)
            return false;

        // need enough money
        if (shipCard.BaseCost > player.Credits)
            return false;

        // hosting costs a click 
        if (player.Clicks < 1)
            return false;

        // if we get here is all good, host away
        shipyard.HostCard(shipCard);
        shipCard.StartConstruction();
        ChangeCredits(player, -shipCard.BaseCost);
        ChangeClicks(player, -1);
        shipCard.StartConstruction();
        return true;
    }

    void ProcessClickForCardAction(Player player, Game game)
    {

        // if the game is not in the right phase log error and return
        if (game.GameState != GameState.LOGISTICS_PLANNING)
        {
            Debug.LogError("Received a click for card message in the wrong game phase Player=" + player.Name);
            return;
        }

        // does the player have a click remaining
        if (player.Clicks < 1)
        {
            Debug.LogError("Received a click for card message from a player with no clicks remaining Player=" + player.Name);
            return;
        }

        // spend the click
        ChangeClicks(player, -1);

        // draw card for player
        Card card = player.Deck.Draw();
        player.Hand.Add(card);

        // send it to the client
        var cardMsg = new MessageTypes.DrawnCardMessage();
        cardMsg.CardCodename = card.CardCodename.ToString();
        cardMsg.cardId = card.CardId;
        cardMsg.cardsInDeck = player.Deck.GetCount();
        NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.DRAWN_CARD, cardMsg);

        // send the action to the opponent for their game log
        SendGameLogToPlayer(game.GetOpponent(player), string.Format("{0} clicks to draw", player.Name));
    }

    void ProcessClickForCreditAction(Player player, Game game)
    {
        // if the game is not in the right phase log error and return
        if (game.GameState != GameState.LOGISTICS_PLANNING)
        {
            Debug.LogError("Received a click for credit message in the wrong game phase Player=" + player.Name);
            return;
        }

        // does the player have a click remaining
        if (player.Clicks < 1)
        {
            Debug.LogError("Received a click for credit message from a player with no clicks remaining Player=" + player.Name);
            return;
        }

        // spend the click
        ChangeClicks(player, -1);

        // add the credit
        ChangeCredits(player, 1);

        // send the action to the opponent for their game log
        SendGameLogToPlayer(game.GetOpponent(player), string.Format("{0} clicks for a credit", player.Name));
    }

    void OnPlayerReadyMessage(NetworkMessage netMsg)
    {
        // determine player and game from connection
        int connectionId = netMsg.conn.connectionId;
        Player player = GetPlayerFromConnection(connectionId);
        Game game = GetGameFromConnection(connectionId);

        if (game.Host == player)
        {
            game.HostReady();
            Debug.Log(string.Format("Game {0} - host {1} is ready", game.GameNumber, game.Host.Name));
        }

        if (game.Challenger == player)
        {
            game.ChallengerReady();
            Debug.Log(string.Format("Game {0} - challenger {1} is ready", game.GameNumber, game.Challenger.Name));
        }
        
        // if both players now ready, start the game proper
        if (game.BothPlayersReady())
        {
            Debug.Log(string.Format("Game {0} - both players ready, setting up game", game.GameNumber));

            // set up the game
            game.Setup();

            // communicate starting state to the players
            SendStartingState(game);

            // send turn message for log
            SendTurnNumber(game);
        }
    }

    private void SendTurnNumber(Game game)
    {
        string message = string.Format("<b>Turn {0}</b>", game.GameTurn);
        SendGameLogToPlayer(game.Host, message);
        SendGameLogToPlayer(game.Challenger, message);
    }

    private void SendStartingState(Game game)
    {
        SendStartingStateToPlayer(game.Host, game.Challenger);
        SendStartingStateToPlayer(game.Challenger, game.Host);
    }

    private void SendStartingStateToPlayer(Player player, Player opponent)
    {
        var logMsg = new MessageTypes.GameLogMessage();
        logMsg.message = "Turn 1";

        var creditsMsg = new MessageTypes.CreditsMessage();
        creditsMsg.credits = player.Credits;
        NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.CREDITS, creditsMsg);

        var clicksMsg = new MessageTypes.ClicksMessage();
        clicksMsg.clicks = player.Clicks;
        NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.CLICKS, clicksMsg);

        for (int i = 0; i < player.Shipyards.Count; i++)
        {
            var shipyardMsg = new MessageTypes.ShipyardMessage();
            shipyardMsg.shipyardType = player.Shipyards[i].CardCodename.ToString();
            shipyardMsg.shipyardId = player.Shipyards[i].CardId;
            shipyardMsg.player = true;
            NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.SHIPYARD, shipyardMsg);
        }

        for (int i = 0; i < player.Hand.Count; i++)
        {
            var cardMsg = new MessageTypes.DrawnCardMessage();            
            cardMsg.CardCodename = player.Hand[i].CardCodename.ToString();
            cardMsg.cardId = player.Hand[i].CardId;
            Debug.Log(String.Format("Sending card {0}({1}) to {2}", cardMsg.CardCodename, cardMsg.cardId, player.Name));
            cardMsg.cardsInDeck = player.Deck.GetCount();
            NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.DRAWN_CARD, cardMsg);
        }

        var opponentMsg = new MessageTypes.OpponentStateMessage();
        opponentMsg.credits = opponent.Credits;
        opponentMsg.clicks = opponent.Clicks;
        opponentMsg.cardsInHand = opponent.Hand.Count;
        opponentMsg.cardsInDeck = opponent.Deck.GetCount();
        opponentMsg.cardsInDiscard = opponent.Discard.Count;
        NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.OPPONENT_STATE, opponentMsg);

        for (int i = 0; i < opponent.Shipyards.Count; i++)
        {
            var shipyardMsg = new MessageTypes.ShipyardMessage();
            shipyardMsg.shipyardType = opponent.Shipyards[i].CardCodename.ToString();
            shipyardMsg.shipyardId = opponent.Shipyards[i].CardId;
            shipyardMsg.player = false;
            NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.SHIPYARD, shipyardMsg);
        }

        var msg = new MessageTypes.SetupGameMessage();
        NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.SETUP_GAME, msg);

        var stateMsg = new MessageTypes.GameStateMessage();
        stateMsg.state = GameState.LOGISTICS_PLANNING.ToString();
        NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.GAME_STATE, stateMsg);
    }

    private void SendGameLogToPlayer(Player player, string message)
    {
        Debug.Log(string.Format("Sending game log message '{0}' to {1}", message, player.Name));
        var msg = new MessageTypes.GameLogMessage();
        msg.message = message;
        NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.GAME_LOG, msg);
    }

    Player GetPlayerFromConnection(int connectionId)
    {
        return ((LobbyController)_lobbyController
            .GetComponent(typeof(LobbyController)))
            ._playerConnections[connectionId];
    }

    Game GetGameFromConnection(int connectionId)
    {
        LobbyController lc = (LobbyController)_lobbyController
            .GetComponent(typeof(LobbyController));
        int gameNumber = lc._gameConnections[connectionId];

        return lc._games[gameNumber];
    }
}
