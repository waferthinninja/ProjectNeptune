using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;

public class GameServerController : NetworkBehaviour {

    public GameObject _lobbyController;
    public Transform _serverMessagePrefab;
    public Transform _serverPanelContent;

    public override void OnStartServer()
    {
        Debug.Log("GameServerController OnStartServer");
        base.OnStartServer(); //base is empty
        
        NetworkServer.RegisterHandler((short)MessageTypes.MessageType.PLAYER_READY, OnPlayerReadyMessage);
        NetworkServer.RegisterHandler((short)MessageTypes.MessageType.SEND_ACTIONS, OnSendActionsMessage);
    }

    private void ServerMessage(string message)
    {
        var serverMessage = Instantiate(_serverMessagePrefab);
        Text t = (Text)serverMessage.GetComponent(typeof(Text));
        t.text = message;
        serverMessage.SetParent(_serverPanelContent);
    }

    private void ServerLog(string message)
    {
        Debug.Log(message);
        ServerMessage(message);        
    }

    private void ServerLog(string message, Game game)
    {
        ServerLog(string.Format("Game {0} - {1}", game.GameNumber, message));
    }

    private void ServerLogError(string message)
    {
        Debug.LogError(message);
        ServerMessage(string.Format("<color=red>{0}</color>", message));        
    }

    private void ServerLogError(string message, Game game)
    {
        ServerLogError(string.Format("Game {0} - {1}", game.GameNumber, message));
    }

    void OnSendActionsMessage(NetworkMessage netMsg)
    {
        // determine player and game from connection
        int connectionId = netMsg.conn.connectionId;
        Player player = GetPlayerFromConnection(connectionId);
        Game game = GetGameFromConnection(connectionId);

        // read the message
        var msg = netMsg.ReadMessage<MessageTypes.SendActionsMessage>();
        
        // store in the game object
        if (game.Host == player)
        {
            game.SubmitHostActions(msg.actionData);
            ServerLog(string.Format("Host {0} has submitted actions", game.Host.Name), game);
        }

        if (game.Challenger == player)
        {
            game.SubmitChallengerActions(msg.actionData);
            ServerLog(string.Format("Challenger {0} has submitted actions", game.Challenger.Name), game);
        }
        
        if (game.BothActionsSubmitted())
        {
            // if both players now submitted, process actions, advance the game state
            ProcessActions(game);
            game.StartLogisticsResolutionPhase();
            //SendGameStateToPlayers(game);

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

        // send actions to opponent - should probably have filtered out bad actions by this point
        SendActionsToPlayer(game.HostActionData, game.Challenger);
        SendActionsToPlayer(game.ChallengerActionData, game.Host);

    }

    private void SendActionsToPlayer(string actionData, Player player)
    {
        var actionMsg = new MessageTypes.SendActionsMessage();
        actionMsg.actionData = actionData;
        NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.SEND_ACTIONS, actionMsg);
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
                string shipId = actionData[1];
                string shipyardId = actionData[2];
                CardCodename cardCodename = (CardCodename)Enum.Parse(typeof(CardCodename), actionData[3]);
                ProcessHostShipAction(player, opponent, game, shipId, shipyardId, cardCodename);
                break;
            case ActionType.DEPLOY_SHIP:
                shipId = actionData[1];
                shipyardId = actionData[2];                
                ProcessDeployShipAction(player, opponent, game, shipId, shipyardId);
                break;
            case ActionType.ADVANCE_CONSTRUCTION:
                shipId = actionData[1];
                shipyardId = actionData[2];
                ProcessAdvanceConstructionAction(player, opponent, game, shipId, shipyardId);
                break;
            default:
                ServerLogError("Unknown action type [" + actionType + "]", game);
                break;
        }
    }

    private void ProcessAdvanceConstructionAction(Player player, Player opponent, Game game, string shipId, string shipyardId)
    {
        Shipyard shipyard = FindCardIn(shipyardId, player.Shipyards);
        Ship ship = shipyard.HostedShip;

        // TODO - check that the shipId matches hosted card

        ServerLog(string.Format("Advancing construction of {0}({1}) on {2}({3})", ship.CardName, ship.CardId, shipyard.CardName, shipyard.CardId ), game);
        ship.AdvanceConstruction(1);
        ChangeClicks(player, -1);
    }

    private void ProcessDeployShipAction(Player player, Player opponent, Game game, string shipId, string shipyardId)
    {        
        Shipyard shipyard = FindCardIn(shipyardId, player.Shipyards);

        // TODO - error if shipyard not found

        Ship ship = shipyard.HostedShip;

        // TODO - error if this is not the correct ship

        // TODO - error if not complete

        ServerLog(string.Format("Deploying {0}({1}) from {2}({3})", ship.CardName, ship.CardId, shipyard.CardName, shipyard.CardId), game);
        shipyard.ClearHostedCard();
        player.Ships.Add(ship);
    }
    
    private T FindCardIn<T>(string cardId, List<T> findIn) where T : Card
    {
        return findIn.Find(t => t.CardId == cardId);        
    }

    void ProcessHostShipAction(Player player, Player opponent, Game game, string shipId, string shipyardId, CardCodename cardCodename)
    {
        // find card and shipyard by their id
        PlayableCard card = FindCardIn(shipId, player.Hand);//player.Hand.Find(x => x.CardId == cardId);
        Ship ship = (Ship)card;
        // TODO - error if card not in hand - at the moment we just trust that it is
        // TODO - error if cardCodename is not the same - need to verify this as we pass to opponent on trust otherwise
        
        Shipyard shipyard = FindCardIn(shipyardId, player.Shipyards);//player.Shipyards.Find(x => x.ShipyardId == shipyardId);

        // TODO - error if shipyard not found

        ServerLog(string.Format("Trying to host {0}({1}) on {2}({3}) for {4}", ship.CardName, ship.CardId, shipyard.CardName, shipyard.CardId, player.Name), game);        
        if (TryHost(game, player, ship, shipyard) == false)
        {
            ServerLogError(string.Format("Failed trying to host {0}({1}) on {2}({3}) for {4}", ship.CardName, ship.CardId, shipyard.CardName, shipyard.CardId, player.Name), game);
        }
    }

    private void ChangeCredits(Player player, int change)
    {
        // update the credits
        player.ChangeCredits(change);

        //// inform the player of their new total
        //var creditsMsg = new MessageTypes.CreditsMessage();
        //creditsMsg.credits = player.Credits;
        //NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.CREDITS, creditsMsg);
    }

    private void ChangeClicks(Player player, int change)
    {
        // update the clicks
        player.ChangeClicks(change);

        //// inform the player of the new total
        //var clicksMsg = new MessageTypes.ClicksMessage();
        //clicksMsg.clicks = player.Clicks;
        //NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.CLICKS, clicksMsg);
    }
    
    public bool TryHost(Game game, Player player, Ship ship, Shipyard shipyard)
    {
        ServerLog(string.Format("TryHost player{0}, shipCard{1}, shipyard{2}", player.Name, ship.CardId, shipyard.CardId), game);
        // can only host one card
        if (shipyard.HostedShip != null)
            return false;

        // can only host ships up to a certain size
        if (ship.Size > shipyard.MaxSize)
            return false;

        // need enough money
        if (ship.BaseCost > player.Credits)
            return false;

        // hosting costs a click 
        if (player.Clicks < 1)
            return false;

        // if we get here is all good, host away
        shipyard.HostCard(ship);
        ship.StartConstruction();
        ChangeCredits(player, -ship.BaseCost);
        ChangeClicks(player, -1);        
        return true;
    }

    void ProcessClickForCardAction(Player player, Game game)
    {
        // if the game is not in the right phase log error and return
        if (game.GameState != GameState.LOGISTICS_PLANNING)
        {
            ServerLogError("Received a click for card message in the wrong game phase Player=" + player.Name);
            return;
        }

        // does the player have a click remaining
        if (player.Clicks < 1)
        {
            ServerLogError("Received a click for card message from a player with no clicks remaining Player=" + player.Name);
            return;
        }

        // spend the click
        ChangeClicks(player, -1);

        // draw card for player
        PlayableCard card = player.Deck.Draw();
        player.Hand.Add(card);

        // send it to the client
        var cardMsg = new MessageTypes.DrawnCardMessage();
        cardMsg.CardCodename = card.CardCodename.ToString();
        cardMsg.cardId = card.CardId;
        cardMsg.cardsInDeck = player.Deck.GetCount();
        NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.DRAWN_CARD, cardMsg);

        // send the action to the opponent for their game log
        //SendGameLogToPlayer(game.GetOpponent(player), string.Format("{0} clicks to draw", player.Name));
    }

    void ProcessClickForCreditAction(Player player, Game game)
    {
        // if the game is not in the right phase log error and return
        if (game.GameState != GameState.LOGISTICS_PLANNING)
        {
            ServerLogError("Received a click for credit message in the wrong game phase Player=" + player.Name);
            return;
        }

        // does the player have a click remaining
        if (player.Clicks < 1)
        {
            ServerLogError("Received a click for credit message from a player with no clicks remaining Player=" + player.Name);
            return;
        }

        // spend the click
        ChangeClicks(player, -1);

        // add the credit
        ChangeCredits(player, 1);
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
            ServerLog(string.Format("Host {0} is ready", game.Host.Name), game);
        }

        if (game.Challenger == player)
        {
            game.ChallengerReady();
            ServerLog(string.Format("Challenger {0} is ready", game.Challenger.Name), game);
        }
        
        // if both players now ready, start the game proper
        if (game.BothPlayersReady())
        {
            ServerLog("Both players ready, setting up game", game);

            // set up the game
            game.Setup();

            // communicate starting state to the players
            SendStartingState(game);
        }
    }
    
    private void SendStartingState(Game game)
    {
        SendStartingStateToPlayer(game, game.Host, game.Challenger);
        SendStartingStateToPlayer(game, game.Challenger, game.Host);
    }

    private void SendStartingStateToPlayer(Game game, Player player, Player opponent)
    {
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
            ServerLog(String.Format("Sending shipyard (own) {0}({1}) to {2}", shipyardMsg.shipyardType, shipyardMsg.shipyardId, player.Name), game);
            NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.SHIPYARD, shipyardMsg);
        }

        for (int i = 0; i < player.Hand.Count; i++)
        {
            var cardMsg = new MessageTypes.DrawnCardMessage();            
            cardMsg.CardCodename = player.Hand[i].CardCodename.ToString();
            cardMsg.cardId = player.Hand[i].CardId;
            cardMsg.cardsInDeck = player.Deck.GetCount();
            ServerLog(String.Format("Sending card {0}({1}) to {2}", cardMsg.CardCodename, cardMsg.cardId, player.Name), game);            
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
            ServerLog(String.Format("Sending shipyard (opponent) {0}({1}) to {2}", shipyardMsg.shipyardType, shipyardMsg.shipyardId, player.Name), game);
            NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.SHIPYARD, shipyardMsg);
        }

        var msg = new MessageTypes.SetupGameMessage();
        msg.playerName = player.Name;
        msg.opponentName = opponent.Name;
        NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.SETUP_GAME, msg);

        var stateMsg = new MessageTypes.GameStateMessage();
        stateMsg.state = GameState.LOGISTICS_PLANNING.ToString();
        NetworkServer.SendToClient(player.ConnectionId, (short)MessageTypes.MessageType.GAME_STATE, stateMsg);
    }

    private void SendGameLogToPlayer(Player player, string message)
    {
        ServerLog(string.Format("Sending game log message '{0}' to {1}", message, player.Name));
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
