using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;
using System.Text;
using UnityEngine.UI;

public class GameClientController : NetworkBehaviour {

    public NetworkClient _client;

    public CardPrefabFactory CardPrefabFactory;
    public Transform FaceDownCardPrefab;

    public Transform _constructionPanelPrefab;

    public Transform GameLogEntryPrefab;
    public Transform GameLogContent;
    public Transform GameLog;

    public Transform _deckSelectDialog;

    private Queue<Transform> CardPlaceholders;

    // local game state    
    private int _opponentCredits;
    private int _opponentClicks;
    private int _opponentCardsInHand;
    private int _opponentCardsInDeck;
    private int _opponentCardsInDiscard;
    private List<Shipyard> _opponentShipyards;
    private List<Ship> _opponentShips;
    private int _playerCredits;
    private int _playerClicks;
    private List<Card> _playerHand;
    private int _playerCardsInDeck;
    private List<Card> _playerDiscard;
    private List<Shipyard> _playerShipyards;
    private List<Ship> _playerShips;
    private GameState _gameState;

    // these are the GUI elements where these data values are displayed    
    public Transform _opponentCreditsGUI;
    public Transform _opponentClicksGUI;
    public Transform _opponentCardsInHandGUI;
    public Transform _opponentCardsInDeckGUI;
    public Transform _opponentCardsInDiscardGUI;
    public Transform _opponentConstructionAreaGUI;
    public Transform _opponentShipAreaGUI;
    public Transform _playerCreditsGUI;
    public Transform _playerClicksGUI; 
    public Transform _playerCardsInDeckGUI;    
    public Transform _playerCardsInDiscardGUI;
    public Transform _playerHandGUI;
    public Transform _playerConstructionAreaGUI;
    public Transform _playerShipAreaGUI;
    public Transform _gameStateGUI;

    // controls which need to be activated/deactivated depending on state
    public Transform _clickForCreditsButton;
    public Transform _clickForCardsButton;
    public Transform _endPhaseButton;

    private List<Action> _actions;

    private Dictionary<string, Transform> _transformById;

    void Start()
    {
        _playerHand = new List<Card>();
        _playerDiscard = new List<Card>();
        _playerShipyards = new List<Shipyard>();
        _playerShips = new List<Ship>();
        _opponentShipyards = new List<Shipyard>();
        _opponentShips = new List<Ship>();
        _actions = new List<Action>();
        _transformById = new Dictionary<string, Transform>();
        CardPlaceholders = new Queue<Transform>();
    }
    
    public override void OnStartClient()
    {
        base.OnStartClient(); // base implementation is currently empty

        _client = NetworkManager.singleton.client;

        Debug.Log("GameClientController _client=" + _client.ToString());

        _client.RegisterHandler((short)MessageTypes.MessageType.SETUP_GAME, OnSetupGameMessage);
        _client.RegisterHandler((short)MessageTypes.MessageType.DRAWN_CARD, OnDrawnCardMessage);
        _client.RegisterHandler((short)MessageTypes.MessageType.CREDITS, OnCreditsMessage);
        _client.RegisterHandler((short)MessageTypes.MessageType.CLICKS, OnClicksMessage);
        _client.RegisterHandler((short)MessageTypes.MessageType.OPPONENT_STATE, OnOpponentStateMessage);
        _client.RegisterHandler((short)MessageTypes.MessageType.GAME_STATE, OnGameStateMessage);
        _client.RegisterHandler((short)MessageTypes.MessageType.SHIPYARD, OnShipyardMessage);
        _client.RegisterHandler((short)MessageTypes.MessageType.HOST_SHIP, OnHostShipMessage);
        _client.RegisterHandler((short)MessageTypes.MessageType.GAME_LOG, OnGameLogMessage);
    }

    private void OnGameLogMessage(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MessageTypes.GameLogMessage>();
        
        var entry = Instantiate(GameLogEntryPrefab);        
        Text t = (Text)entry.GetComponent(typeof(Text));
        t.text = msg.message;
        entry.SetParent(GameLogContent);

        var scrollRect = (ScrollRect)GameLog.GetComponent(typeof(ScrollRect));
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private void OnHostShipMessage(NetworkMessage netMsg) // note that this will currently always be an opponents action, as the player version is handled in the client
    {
        // TODO - consolidate this and the player hosting as mostly are doing the same thing
        var msg = netMsg.ReadMessage<MessageTypes.HostShipMessage>();
        var CardCodenameData = msg.CardCodename;
        var cardId = msg.cardId;
        var shipyardId = msg.shipyardId;
        //Transform card = FindCardTransformById(cardId);
        //instantiate card
        var CardCodename = (CardCodename)Enum.Parse(typeof(CardCodename), CardCodenameData); // TODO - verify this is actually a ship card?
        var shipCard = new Ship(CardCodename, cardId);

        // instantiate the card prefab 
        var cardPrefab = InstantiateCardPrefab(shipCard);

        var shipyard = FindShipyardTransformById(shipyardId);

        // attach the ship to the shipyard
        cardPrefab.SetParent(shipyard);
        cardPrefab.localPosition = new Vector3(15, -15, 0);
        
        // set the construction remaining
        shipCard.StartConstruction();

        // add "construction remaining" overlay
        var constructionPanelPrefab = Instantiate(_constructionPanelPrefab);
        var constructionRemaining = (Text)constructionPanelPrefab.Find("ConstructionRemaining").GetComponent(typeof(Text));// TODO - find by child name?
        constructionRemaining.text = shipCard.ConstructionRemaining.ToString();
        constructionPanelPrefab.SetParent(cardPrefab);
        constructionPanelPrefab.localPosition = new Vector3(0, 0, 0);

        Debug.Log(String.Format("Hosting ship {0} on shipyard {1}", cardId, shipyardId));
    }

    private Transform InstantiateCardPrefab(Card card)
    {        
        Transform cardPrefab = CardPrefabFactory.CreateCardPrefab(card);

        // store in lookup so we can find it by its id
        _transformById[card.CardId] = cardPrefab;
        
        return cardPrefab;
    }

    private Transform FindCardTransformById(string cardId)
    {
        return _transformById[cardId];
    }

    private Transform FindShipyardTransformById(string shipyardId)
    {
        return _transformById[shipyardId];
    }

    private void OnGameStateMessage(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MessageTypes.GameStateMessage>();
        Debug.Log("Game state: " + msg.state);
        _gameState = (GameState)Enum.Parse(typeof(GameState), msg.state);
        SetText(_gameStateGUI, _gameState.ToString().Replace('_',' '));

        // if gamestate is now logistics resolution - 
        // at the moment the turn resolves instantly, so instantly
        // advance to the next phase
        // TODO - remove this once we are slowly playing out resolutions
        if (_gameState == GameState.LOGISTICS_RESOLUTION)
        {
            _gameState = GameState.COMBAT_PLANNING;
        }

        // activate controls based on new game state
        EnableDisableControls();
    }

    private void EnableDisableControls()
    {
        //Debug.Log("EnableDisableControls");
        ((Button)_clickForCreditsButton.GetComponent(typeof(Button))).interactable = (_playerClicks > 0);
        ((Button)_clickForCardsButton.GetComponent(typeof(Button))).interactable = (_playerClicks > 0);

        // some controls only available when in logistics planning and clicks remain
        bool planningClicksRemain = _gameState == GameState.LOGISTICS_PLANNING && _playerClicks > 0;

        ((Button)_endPhaseButton.GetComponent(typeof(Button))).interactable = _gameState == GameState.COMBAT_PLANNING
            || (_gameState == GameState.LOGISTICS_PLANNING && _playerClicks < 1);

        // handle "advance construction" buttons
        var constructionHandlers = FindObjectsOfType<AdvanceConstructionHandler>();
        foreach(AdvanceConstructionHandler handler in constructionHandlers)
        {
            (handler.GetComponent<Button>()).interactable = planningClicksRemain;
        }        
    }

    private void OnSetupGameMessage(NetworkMessage netMsg)
    {
        Debug.Log("Game starting");
        // close the deck select dialog
        _deckSelectDialog.gameObject.SetActive(false);
    }

    private void OnCreditsMessage(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MessageTypes.CreditsMessage>();
        Debug.Log("Credits: " + msg.credits);
        _playerCredits = msg.credits;
        UpdatePlayerStateGUI();
    }

    private void OnClicksMessage(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MessageTypes.ClicksMessage>();
        Debug.Log("Clicks: " + msg.clicks);
        _playerClicks = msg.clicks;
        UpdatePlayerStateGUI();
    }

    private void OnDrawnCardMessage(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MessageTypes.DrawnCardMessage>();
        var cardCodenameData = msg.CardCodename;
        string cardId = msg.cardId;
        _playerCardsInDeck = msg.cardsInDeck;
        Debug.Log(String.Format("Card drawn: {0}({1})", cardCodenameData, cardId));

        // add to local version of game state 
        CardCodename cardCodename = (CardCodename)Enum.Parse(typeof(CardCodename), cardCodenameData);
        Card card = CardFactory.CreateCard(cardCodename, cardId);
        _playerHand.Add(card);

        // instantiate the card prefab
        Transform cardPrefab = InstantiateCardPrefab(card);

        // add it to the players hand area
        cardPrefab.SetParent(_playerHandGUI);

        // remove the placeholder face down card
        Destroy(CardPlaceholders.Dequeue().gameObject);

        // store in dictionary
        _transformById[card.CardId] = cardPrefab.transform;

        // update player state gui
        UpdatePlayerStateGUI();
    }

    private void OnShipyardMessage(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MessageTypes.ShipyardMessage>();        
        var shipyardTypeData = msg.shipyardType;
        string shipyardId = msg.shipyardId;
        bool belongsToPlayer = msg.player;
        Debug.Log(string.Format("Shipyard message. Type {0}({1}) {2}", msg.shipyardType, msg.shipyardId, (belongsToPlayer ? " belonging to player" : " belonging to opponent")));

        CardCodename shipyardCodename = (CardCodename)Enum.Parse(typeof(CardCodename), shipyardTypeData);
        Shipyard shipyard = (Shipyard)CardFactory.CreateCard(shipyardCodename, shipyardId);

        // add to the appropriate player's list
        List<Shipyard> shipYardList = (belongsToPlayer ? _playerShipyards : _opponentShipyards);
        shipYardList.Add(shipyard);

        // instantiate a prefab 
        var shipyardPrefab = CardPrefabFactory.CreateCardPrefab(shipyard);


        // link the shipyard prefab to the underlying object
        var link = shipyardPrefab.GetComponent<CardLink>();
        link.card = shipyard;

        // set the approriate dropzone type
        var dropzone = shipyardPrefab.GetComponent<DropHandler>();
        dropzone.DropZoneType = (belongsToPlayer ? DropZoneType.PLAYER_SHIPYARD : DropZoneType.ENEMY_SHIPYARD);

        Transform constructionArea = (belongsToPlayer ? _playerConstructionAreaGUI : _opponentConstructionAreaGUI);
        shipyardPrefab.SetParent(constructionArea);
        
        // store in dictionary
        _transformById[shipyard.CardId] = shipyardPrefab.transform;
    }

    private void OnOpponentStateMessage(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MessageTypes.OpponentStateMessage>();
        _opponentCredits = msg.credits;
        _opponentClicks = msg.clicks;
        _opponentCardsInHand = msg.cardsInHand;
        _opponentCardsInDeck = msg.cardsInDeck;
        _opponentCardsInDiscard = msg.cardsInDiscard;

        UpdateOpponentStateGUI();
    }

    private void UpdateOpponentStateGUI()
    {
        SetText(_opponentClicksGUI, _opponentClicks.ToString());
        SetText(_opponentCreditsGUI, _opponentCredits.ToString());
        SetText(_opponentCardsInHandGUI, _opponentCardsInHand.ToString());
        SetText(_opponentCardsInDeckGUI, _opponentCardsInDeck.ToString());
        SetText(_opponentCardsInDiscardGUI, _opponentCardsInDiscard.ToString());
    }

    private void UpdatePlayerStateGUI()
    {
        SetText(_playerClicksGUI, _playerClicks.ToString());
        SetText(_playerCreditsGUI, _playerCredits.ToString());
        SetText(_playerCardsInDeckGUI, _playerCardsInDeck.ToString());
        SetText(_playerCardsInDiscardGUI, _playerDiscard.Count.ToString());
    }

    private void SetText(Transform guiElement, string value)
    {
        // TODO - check this is actually a Text element?
        guiElement.GetComponent<Text>().text = value;

    }

    public void PlayerReady()
    {
        Debug.Log("Sending ready message");
        MessageTypes.PlayerReadyMessage msg = new MessageTypes.PlayerReadyMessage();
        NetworkManager.singleton.client.Send((short)MessageTypes.MessageType.PLAYER_READY, msg);

        // disable the button
        var createGameButton = (Button)GameObject.Find("ReadyButton").GetComponent(typeof(Button));
        createGameButton.interactable = false;

        // create placeholder card objects
        // TODO - this should be based on the faction starting cards, for now we will hard code until we have done the deck selector thing properly
        for (int i = 0; i < 5; i++)
        {
            var card = Instantiate(FaceDownCardPrefab);
            card.SetParent(_playerHandGUI);
            CardPlaceholders.Enqueue(card);
        }
    }

    private void UpdateClicks(int change)
    {
        _playerClicks += change;

        EnableDisableControls();
        UpdatePlayerStateGUI();
    }

    public void ClickToDraw() // not that we don't draw the cards until the end of the phase
    {
        Debug.Log("Clicking for card");
        _actions.Add(new Action(ActionType.CLICK_FOR_CARD));
        UpdateClicks(-1);

        // create a face down card as a placeholder
        var card = Instantiate(FaceDownCardPrefab);
        card.SetParent(_playerHandGUI);
        CardPlaceholders.Enqueue(card);
        
    }

    public void ClickForCredit()
    {
        Debug.Log("Clicking for credit");
        _actions.Add(new Action(ActionType.CLICK_FOR_CREDIT));
        _playerCredits++;
        UpdateClicks(-1);
    }
    
    public bool TryAdvanceConstruction(Ship ship)
    {
        // cant advance if already complete
        if (ship.ConstructionRemaining < 1)
            return false;

        // if we get here, go ahead and perform the advancement
        ship.AdvanceConstruction(1);
        _actions.Add(new AdvanceConstructionAction(ship));
        UpdateClicks(-1);
        return true;
    }

    public bool TryDeploy(Ship ship, Shipyard shipyard)
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
        _playerShips.Add(ship);

        return true;
    }

    public bool TryHost(Ship shipCard, Shipyard shipyard)
    {        
        // can only host one card
        if (shipyard.HostedShip != null)
            return false;

        // can only host ships up to a certain size
        if (shipCard.Size > shipyard.MaxSize)
            return false;

        // need enough money
        if (shipCard.BaseCost > _playerCredits)
            return false;

        // hosting costs a click
        if (_playerClicks < 1)
            return false;

        // TODO - we could gather the reasons for failure here and do something e.g display a popup? 

        // if we get here is all good, host away
        UpdateClicks(-1);
        _playerCredits -= shipCard.BaseCost;
        _actions.Add(new HostShipAction(shipCard, shipyard));
        HostCard(shipCard, shipyard);

        UpdatePlayerStateGUI();
        
        return true;
    }

    private void HostCard(Ship shipCard, Shipyard shipyard)
    {
        Debug.Log(string.Format("Hosting card {0} on shipyard {1}", shipCard.CardId, shipyard.CardId));
        shipyard.HostCard(shipCard);

        Transform cardTransform = FindCardTransformById(shipCard.CardId);
        Transform shipyardTransform = FindShipyardTransformById(shipyard.CardId);

        // set the parent and offset
        cardTransform.SetParent(shipyardTransform);
        cardTransform.localPosition = new Vector3(15, -15, 0);

        // set the construction remaining
        shipCard.StartConstruction();

        // add "construction remaining" overlay
        var constructionPanelPrefab = Instantiate(_constructionPanelPrefab);
        var constructionRemaining = (Text)constructionPanelPrefab.GetChild(1).GetComponent(typeof(Text));// TODO - find by child name?
        constructionRemaining.text = shipCard.ConstructionRemaining.ToString();
        constructionPanelPrefab.SetParent(cardTransform);
        constructionPanelPrefab.localPosition = new Vector3(0, 0, 0);

    }

    public void SubmitActions()
    {
        // TODO - warn player if they have clicks remaining

        MessageTypes.SubmitActionsMessage msg = new MessageTypes.SubmitActionsMessage();

        // serialize actions
        StringBuilder data = new StringBuilder();
        foreach(Action action in _actions)
        {
            data.Append(action.ToString() + '#');
        }
        msg.actionData = data.ToString().TrimEnd('#');

        NetworkManager.singleton.client.Send((short)MessageTypes.MessageType.SUBMIT_ACTIONS, msg);
    }
}
            

