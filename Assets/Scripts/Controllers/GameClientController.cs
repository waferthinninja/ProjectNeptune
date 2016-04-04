using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;
using System.Text;
using UnityEngine.UI;

public class GameClientController : NetworkBehaviour {

    public GameViewController GameViewController;
    public LobbyController LobbyController;

    private NetworkClient _client;
    private Game _game; // local game position
    private List<PlayerAction> _actions;

    void Start()
    {
        _actions = new List<PlayerAction>();
    }
    
    public override void OnStartClient()
    {
        base.OnStartClient(); // base implementation is currently empty

        _client = NetworkManager.singleton.client;

        Debug.Log("GameClientController _client=" + _client.ToString());

        _client.RegisterHandler((short)MessageTypes.MessageType.DECK_FIRST, OnDeckFirstMessage);
        _client.RegisterHandler((short)MessageTypes.MessageType.DECK_FRAGMENT, OnDeckFragmentMessage);
        _client.RegisterHandler((short)MessageTypes.MessageType.SETUP_GAME, OnSetupGameMessage);
        _client.RegisterHandler((short)MessageTypes.MessageType.DRAWN_CARD, OnDrawnCardMessage);
        _client.RegisterHandler((short)MessageTypes.MessageType.GAME_LOG, OnGameLogMessage);
        _client.RegisterHandler((short)MessageTypes.MessageType.SEND_ACTIONS, OnSendActionsMessage);
    }

    private void OnDeckFirstMessage(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MessageTypes.DeckFirstMessage>();

        // set data
        LobbyController.Opponent.DeckData = msg.deckDataFragment;

    }

    private void OnDeckFragmentMessage(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MessageTypes.DeckFragmentMessage>();

        // append data
        LobbyController.Opponent.DeckData += msg.deckDataFragment;
    }

    private void OnSendActionsMessage(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MessageTypes.SendActionsMessage>();
        ProcessOpponentActions(msg.actionData);
    }

    private void ProcessOpponentActions(string actionData)
    {
        // TODO - process these actions slowly, need to think about how to do this
        string[] data = actionData.Split('#');
        for (int i = 0; i < data.Length; i++)
        {
            ProcessOpponentAction(data[i]);

            // innefficient as sometimes unnecessary, but simple way to ensure this is up to date
            UpdateOpponentStateGUI();
            UpdatePlayerStateGUI();
        }
    }

    private void ProcessOpponentAction(string action)
    {
        Debug.Log(string.Format("Processing action {0}", action));
        string[] actionData = action.Split('|');
        ActionType actionType = (ActionType)Enum.Parse(typeof(ActionType), actionData[0]);
        switch (actionType)
        {
            case ActionType.CLICK_FOR_CARD:
                ProcessOpponentClickForCardAction();
                break;
            case ActionType.CLICK_FOR_CREDIT:
                ProcessOpponentClickForCreditAction();
                break;
            case ActionType.HOST_SHIP:
                string shipId = actionData[1];
                string shipyardId = actionData[2];
                CardCodename cardCodename = (CardCodename)Enum.Parse(typeof(CardCodename), actionData[3]);
                ProcessOpponentHostShipAction(shipId, shipyardId, cardCodename);
                break;
            case ActionType.DEPLOY_SHIP:
                shipId = actionData[1];
                shipyardId = actionData[2];
                ProcessOpponentDeployShipAction(shipId, shipyardId);
                break;
            case ActionType.ADVANCE_CONSTRUCTION:
                shipId = actionData[1];
                shipyardId = actionData[2];
                ProcessOpponentAdvanceConstructionAction(shipId, shipyardId);
                break;
            case ActionType.SHIPYARD:
                shipyardId = actionData[1];
                cardCodename = (CardCodename)Enum.Parse(typeof(CardCodename), actionData[2]);
                ProcessOpponentShipyardAction(shipyardId, cardCodename);
                break;
            default:
                Debug.LogError("Unknown action type [" + actionType + "]");
                break;
        }
    }

    private void ProcessOpponentShipyardAction(string shipyardId, CardCodename cardCodename)
    {
        // card needs to be instantiated
        Shipyard shipyard = (Shipyard)CardFactory.CreateCard(cardCodename, shipyardId);
        GameViewController.AddShipyard(shipyard, false);

        // spend the resources
        _game.Opponent.ChangeClicks(-1);
        _game.Opponent.ChangeCredits(-shipyard.BaseCost);
        _game.Opponent.Hand.RemoveAt(0);
        _game.Opponent.Shipyards.Add(shipyard);

        // log 
        GameViewController.AddGameLogMessage(string.Format("<b>{0}</b> plays {1}", _game.Opponent.Name, shipyard.CardName));
    }

    private void ProcessOpponentAdvanceConstructionAction(string shipId, string shipyardId)
    {
        Shipyard shipyard = _game.Opponent.Shipyards.Find(x => x.CardId == shipyardId);
        Ship ship = shipyard.HostedShip;
        ship.AdvanceConstruction(1);
        _game.Opponent.ChangeClicks(-1);

        // update gui panel
        GameViewController.UpdateConstructionRemaining(ship);

        GameViewController.AddGameLogMessage(string.Format("<b>{0}</b> advances construction of {1}", _game.Opponent.Name, ship.CardName));
    }

    private void ProcessOpponentDeployShipAction(string shipId, string shipyardId)
    {
        // move the ship to the deployed area
        Shipyard shipyard = _game.Opponent.Shipyards.Find(x => x.CardId == shipyardId);
        Ship ship = shipyard.HostedShip;
        _game.Opponent.Ships.Add(ship);
        shipyard.ClearHostedCard();

        GameViewController.DeployShip(ship, false);       
        
        GameViewController.AddGameLogMessage(string.Format("<b>{0}</b> deploys {1}", _game.Opponent.Name, ship.CardName));
    }

    private void ProcessOpponentHostShipAction(string shipId, string shipyardId, CardCodename cardCodename)
    {
        // ship will be an unknown card at this point so needs to be instantiated
        Ship ship = (Ship)CardFactory.CreateCard(cardCodename, shipId);
        Shipyard shipyard = _game.Opponent.Shipyards.Find(x => x.CardId == shipyardId);
        shipyard.HostCard(ship);
        ship.StartConstruction();
        ship.OnPlay(_game, _game.Opponent);

        _game.Opponent.ChangeClicks(-1);
        _game.Opponent.ChangeCredits(-ship.BaseCost);
        _game.Opponent.Hand.RemoveAt(0);        

        GameViewController.HostShip(ship, shipyard, false);                
        
        GameViewController.AddGameLogMessage(string.Format("<b>{0}</b> hosts {1} on {2}", _game.Opponent.Name, ship.CardName, shipyard.CardName));
    }

    private void ProcessOpponentClickForCreditAction()
    {
        _game.Opponent.ChangeClicks(-1);
        _game.Opponent.ChangeCredits(1);
        GameViewController.AddGameLogMessage(string.Format("<b>{0}</b> clicks for a credit", _game.Opponent.Name));
    }

    private void ProcessOpponentClickForCardAction()
    {
        _game.Opponent.ChangeClicks(-1);
        _game.Opponent.DrawFromDeck();        
        GameViewController.AddGameLogMessage(string.Format("<b>{0}</b> clicks for a card", _game.Opponent.Name));
    }

    private void OnGameLogMessage(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MessageTypes.GameLogMessage>();

        GameViewController.AddGameLogMessage(msg.message);
    }   
       
    private void EnableDisableControls()
    {
        GameViewController.EnableDisableControls(_game.GameState, _game.Player.Clicks > 0);     
    }

    private void OnSetupGameMessage(NetworkMessage netMsg)
    {
        Debug.Log("Game starting");
        //var msg = netMsg.ReadMessage<MessageTypes.SetupGameMessage>();
        Deck deck = new Deck(LobbyController.Opponent.DeckData);
        LobbyController.Opponent.SetDeck(deck);
        _game = new Game(0, LobbyController.LocalPlayer);
        _game.AddOpponent(LobbyController.Opponent);
        _game.Setup();

        SetupInitialGameView();

        GameViewController.HideDeckSelectDialog();
        WriteGameTurnToLog();
    }

    private void SetupInitialGameView()
    {
        GameViewController.SetPlayerName(_game.Player.Name);
        GameViewController.SetOpponentName(_game.Opponent.Name);
        UpdatePlayerStateGUI();
        UpdateOpponentStateGUI();

        // TODO Homeworld

        foreach(Shipyard shipyard in _game.Player.Shipyards)
        {
            GameViewController.AddShipyard(shipyard, true);
        }

        foreach (Shipyard shipyard in _game.Opponent.Shipyards)
        {
            GameViewController.AddShipyard(shipyard, false);
        }
    }

    private void WriteGameTurnToLog()
    {
        GameViewController.AddGameLogMessage(string.Format("<b>Turn {0}</b>", _game.GameTurn));
    }

    private void OnDrawnCardMessage(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MessageTypes.DrawnCardMessage>();
        var cardCodenameData = msg.CardCodename;
        string cardId = msg.cardId;
        Debug.Log(String.Format("Card drawn: {0}({1})", cardCodenameData, cardId));

        // add to local version of game state 
        CardCodename cardCodename = (CardCodename)Enum.Parse(typeof(CardCodename), cardCodenameData);
        PlayableCard card = (PlayableCard)CardFactory.CreateCard(cardCodename, cardId);
        _game.Player.Hand.Add(card);

        GameViewController.AddCardToHand(card, true);

        // update player state gui
        UpdatePlayerStateGUI();
    }

    private void UpdatePlayerStateGUI()
    {
        GameViewController.UpdatePlayerStateGUI(_game.Player);
    }

    private void UpdateOpponentStateGUI()
    {
        GameViewController.UpdateOpponentStateGUI(_game.Opponent);
    }

    //private void OnShipyardMessage(NetworkMessage netMsg)
    //{
    //    var msg = netMsg.ReadMessage<MessageTypes.ShipyardMessage>();        
    //    var shipyardTypeData = msg.shipyardType;
    //    string shipyardId = msg.shipyardId;
    //    bool belongsToPlayer = msg.player;
    //    Debug.Log(string.Format("Shipyard message. Type {0}({1}) {2}", msg.shipyardType, msg.shipyardId, (belongsToPlayer ? " belonging to player" : " belonging to opponent")));

    //    CardCodename shipyardCodename = (CardCodename)Enum.Parse(typeof(CardCodename), shipyardTypeData);
    //    Shipyard shipyard = (Shipyard)CardFactory.CreateCard(shipyardCodename, shipyardId);

    //    // add to the appropriate player's list
    //    List<Shipyard> shipYardList = (belongsToPlayer ? _playerShipyards : _opponentShipyards);
    //    shipYardList.Add(shipyard);

    //    // instantiate a prefab 
    //    var shipyardPrefab = InstantiateCardPrefab(shipyard);

    //    // link the shipyard prefab to the underlying object
    //    var link = shipyardPrefab.GetComponent<CardLink>();
    //    link.Card = shipyard;

    //    // set the approriate dropzone type
    //    var dropzone = shipyardPrefab.GetComponent<DropHandler>();
    //    dropzone.DropZoneType = (belongsToPlayer ? DropZoneType.PLAYER_SHIPYARD : DropZoneType.ENEMY_SHIPYARD);

    //    Transform constructionArea = (belongsToPlayer ? PlayerConstructionAreaGUI : OpponentConstructionAreaGUI);
    //    shipyardPrefab.SetParent(constructionArea);
    //}   

    public void PlayerReady()
    {
        // this is where we would set the deck to be the one selected in the deck selection dialog
        LobbyController.LocalPlayer.SetDeck(new Deck());

        Debug.Log("Sending deck fragment");
        string deckData = LobbyController.LocalPlayer.Deck.ToString(false);
        MessageTypes.DeckFirstMessage msgFirst = new MessageTypes.DeckFirstMessage();
        msgFirst.deckDataFragment = deckData.Substring(0, Math.Min(deckData.Length, 1024));
        NetworkManager.singleton.client.Send((short)MessageTypes.MessageType.DECK_FIRST, msgFirst);

        for (int i = 1; i * 1024 < deckData.Length; i++)
        {
            Debug.Log("Sending deck fragment");
            MessageTypes.DeckFragmentMessage msgFrag = new MessageTypes.DeckFragmentMessage();
            msgFrag.deckDataFragment = deckData.Substring(i * 1024, Math.Min((deckData.Length - i * 1024), 1024));
            NetworkManager.singleton.client.Send((short)MessageTypes.MessageType.DECK_FRAGMENT, msgFrag);
        }

        Debug.Log("Sending ready message");
        MessageTypes.PlayerReadyMessage msg = new MessageTypes.PlayerReadyMessage();
        NetworkManager.singleton.client.Send((short)MessageTypes.MessageType.PLAYER_READY, msg);
        
        // disable the button
        var createGameButton = (Button)GameObject.Find("ReadyButton").GetComponent(typeof(Button));
        createGameButton.interactable = false;

        // create placeholder card objects        
        for (int i = 0; i < LobbyController.LocalPlayer.Deck.Faction.StartingHandSize; i++)
        {
            Debug.Log("Adding fake card to hand");
            GameViewController.AddUnknownCardToHand();
        }
    }

    private void ChangeClicks(int change)
    {
        _game.Player.ChangeClicks(change);

        EnableDisableControls();
        UpdatePlayerStateGUI();
    }

    public void ClickToDraw() // not that we don't draw the cards until the end of the phase
    {
        Debug.Log("Clicking for card");
        _actions.Add(new PlayerAction(ActionType.CLICK_FOR_CARD));
        ChangeClicks(-1);

        GameViewController.AddUnknownCardToHand();        
    }

    public void ClickForCredit()
    {
        Debug.Log("Clicking for credit");
        _actions.Add(new PlayerAction(ActionType.CLICK_FOR_CREDIT));
        _game.Player.ChangeCredits(1);
        ChangeClicks(-1);
    }
    
    public bool TryAdvanceConstruction(Ship ship, Shipyard shipyard)
    {
        // cant advance if already complete
        if (ship.ConstructionRemaining < 1)
            return false;

        // if we get here, go ahead and perform the advancement
        ship.AdvanceConstruction(1);
        _actions.Add(new AdvanceConstructionAction(ship, shipyard));
        ChangeClicks(-1);
        return true;
    }

    public bool TryPlayShipyard(Shipyard shipyard)
    {
        // card must be in the players hand
        if (!_game.Player.Hand.Contains(shipyard))
            return false;

        // need enough money
        if (shipyard.BaseCost > _game.Player.Credits)
            return false;

        // playing costs a click
        if (_game.Player.Clicks < 1)
            return false;

        // if we get here, go ahead and play it
        _actions.Add(new ShipyardAction(shipyard));
        _game.Player.Shipyards.Add(shipyard);
        ChangeClicks(-1);
        _game.Player.ChangeCredits(-shipyard.BaseCost);

        GameViewController.AddShipyard(shipyard, true);   

        return true;
    }

    public bool TryDeployShip(Ship ship, Shipyard shipyard)
    {
        if (shipyard.HostedShip != ship)
        {
            Debug.LogError("Trying to deploy from wrong shipyard");
            return false;
        }

        if (ship.ConstructionRemaining > 0)
        {
            Debug.LogError("Trying to deploy incomplete ship");
            return false;
        }

        // if we get here, all good
        shipyard.ClearHostedCard();
        _game.Player.Ships.Add(ship);
        _actions.Add(new DeployAction(ship, shipyard));

        GameViewController.DeployShip(ship, true);

        return true;
    }

    public bool TryHost(Ship ship, Shipyard shipyard)
    {
        // ship card must be in the players hand
        if (!_game.Player.Hand.Contains(ship))
            return false;

        // can only host one card
        if (shipyard.HostedShip != null)
            return false;

        // can only host ships up to a certain size
        if (ship.Size > shipyard.MaxSize)
            return false;

        // need enough money
        if (ship.BaseCost > _game.Player.Credits)
            return false;

        // hosting costs a click
        if (_game.Player.Clicks < 1)
            return false;

        // TODO - we could gather the reasons for failure here and do something e.g display a popup? 

        // if we get here is all good, host away
        _actions.Add(new HostShipAction(ship, shipyard));
        ChangeClicks(-1);
        _game.Player.ChangeCredits(-ship.BaseCost);
        shipyard.HostCard(ship);
        ship.StartConstruction();
        _game.Player.Hand.Remove(ship);

        GameViewController.HostShip(ship, shipyard, true);

        UpdatePlayerStateGUI();
        EnableDisableControls();

        Debug.Log(string.Format("Hosting card {0} on shipyard {1}", ship.CardId, shipyard.CardId));

        return true;
    }
    

    public void SubmitActions()
    {
        // TODO - warn player if they have clicks remaining
        MessageTypes.SendActionsMessage msg = new MessageTypes.SendActionsMessage();

        // serialize actions
        StringBuilder data = new StringBuilder();
        foreach(PlayerAction action in _actions)
        {
            data.Append(action.ToString() + '#');
        }
        msg.actionData = data.ToString().TrimEnd('#');

        NetworkManager.singleton.client.Send((short)MessageTypes.MessageType.SEND_ACTIONS, msg);

        _game.AwaitOpponent();
        EnableDisableControls();
    }
}
            

