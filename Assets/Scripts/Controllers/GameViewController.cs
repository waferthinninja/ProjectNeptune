using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine.UI;

public class GameViewController : MonoBehaviour {

    private Queue<Transform> _cardPlaceholders;

    public CardPrefabFactory CardPrefabFactory;


    public Transform ConstructionPanelPrefab;
    public Transform GameLogEntryPrefab;
    public Transform GameLogContent;
    public Transform GameLog;
    public Transform DeckSelectDialog;

    // controls which need to be activated/deactivated depending on state
    public Transform ClickForCreditsButton;
    public Transform ClickForCardsButton;
    public Transform EndPhaseButton;

    // gui representation of Game position
    public Transform OpponentNameGUI;
    public Transform OpponentCreditsGUI;
    public Transform OpponentClicksGUI;
    public Transform OpponentCardsInHandGUI;
    public Transform OpponentCardsInDeckGUI;
    public Transform OpponentCardsInDiscardGUI;
    public Transform OpponentConstructionAreaGUI;
    public Transform OpponentShipAreaGUI;
    public Transform PlayerNameGUI;
    public Transform PlayerCreditsGUI;
    public Transform PlayerClicksGUI;
    public Transform PlayerCardsInDeckGUI;
    public Transform PlayerCardsInDiscardGUI;
    public Transform PlayerHandGUI;
    public Transform PlayerConstructionAreaGUI;
    public Transform PlayerShipAreaGUI;
    public Transform GameStateGUI;
    
    private Dictionary<string, Transform> _transformById;

    void Start()
    {
        _transformById = new Dictionary<string, Transform>();
        _cardPlaceholders = new Queue<Transform>();
    }

    public void AddShipyard(Shipyard shipyard, bool belongsToPlayer)
    {        
        // instantiate
        Transform shipyardTransform = InstantiateCardPrefab(shipyard);

        // position 
        Transform constructionArea = (belongsToPlayer ? PlayerConstructionAreaGUI : OpponentConstructionAreaGUI);
        shipyardTransform.SetParent(constructionArea);        
    }

    public void HostShip(Ship ship, Shipyard shipyard, bool belongsToPlayer)
    {
        Transform shipTransform = InstantiateCardPrefab(ship);
        Transform shipyardTransform = FindCardTransformById(shipyard.CardId);
        
        // add "construction remaining" overlay
        var constructionPanelPrefab = Instantiate(ConstructionPanelPrefab);
        constructionPanelPrefab.name = "ConstructionPanel";
        var constructionRemaining = (Text)constructionPanelPrefab.Find("ConstructionRemaining").GetComponent(typeof(Text));
        constructionRemaining.text = ship.ConstructionRemaining.ToString();

        if (belongsToPlayer == false)
        {
            // remove the "advance" button
            Button button = constructionPanelPrefab.Find("AdvanceConstructionButton").GetComponent<Button>();
            button.transform.localScale = new Vector3();
            button.interactable = false;
        }

        constructionPanelPrefab.SetParent(shipTransform);
        constructionPanelPrefab.localPosition = new Vector3(0, 0, 0);
        
        // position the ship on the shipyard
        shipTransform.SetParent(shipyardTransform);
        shipTransform.localPosition = new Vector3(15, 15, 0);
    }

    public void UpdateGameState(GameState gameState)
    {
        SetText(GameStateGUI, gameState.ToString().Replace('_', ' '));
    }

    private void SetText(Transform guiElement, string value)
    {
        // TODO - check this is actually a Text element?
        guiElement.GetComponent<Text>().text = value;
    }

    public void SetPlayerName(string name)
    {
        SetText(PlayerNameGUI, name);
    }

    public void SetOpponentName(string name)
    {
        SetText(OpponentNameGUI, name);
    }

    public void HideDeckSelectDialog()
    {
        DeckSelectDialog.gameObject.SetActive(false);
    }

    public void EnableDisableControls(GameState gameState, bool clicksRemain)
    {
        //Debug.Log("EnableDisableControls");
        ((Button)ClickForCreditsButton.GetComponent(typeof(Button))).interactable = clicksRemain;
        ((Button)ClickForCardsButton.GetComponent(typeof(Button))).interactable = clicksRemain;

        // some controls only available when in logistics planning and clicks remain
        bool planningClicksRemain = gameState == GameState.LOGISTICS_PLANNING && clicksRemain;

        ((Button)EndPhaseButton.GetComponent(typeof(Button))).interactable = gameState == GameState.COMBAT_PLANNING
            || (gameState == GameState.LOGISTICS_PLANNING && clicksRemain == false);

        // handle "advance construction" buttons
        var constructionHandlers = FindObjectsOfType<AdvanceConstructionHandler>();
        foreach (AdvanceConstructionHandler handler in constructionHandlers)
        {
            (handler.GetComponent<Button>()).interactable = planningClicksRemain ||
                (gameState == GameState.LOGISTICS_PLANNING && handler.IsComplete());
        }
    }

    public void UpdateConstructionRemaining(Ship ship)
    {
        Transform shipTransform = FindCardTransformById(ship.CardId);
        Transform constructionRemainingT = shipTransform.Find("ConstructionPanel/ConstructionRemaining");
        Text constructionRemaining = (Text)constructionRemainingT.GetComponent(typeof(Text));
        constructionRemaining.text = ship.ConstructionRemaining.ToString();
    }

    public void AddCardToHand(PlayableCard card, bool replacesUnknown)
    {
        // instantiate the card prefab
        Transform cardPrefab = InstantiateCardPrefab(card);

        // add it to the players hand area
        cardPrefab.SetParent(PlayerHandGUI);

        if (replacesUnknown)
        {
            // remove the placeholder face down card
            Destroy(_cardPlaceholders.Dequeue().gameObject);
        }
    }

    public void AddUnknownCardToHand()
    {
        var card = CardPrefabFactory.CreateCardPrefab(new UnknownCard(CardCodename.UNKNOWN));
        card.SetParent(PlayerHandGUI);
        _cardPlaceholders.Enqueue(card);
    }

    public void DeployShip(Ship ship, bool belongsToPlayer)
    {
        Transform shipArea = (belongsToPlayer ? PlayerShipAreaGUI : OpponentShipAreaGUI);

        Transform shipTransform = FindCardTransformById(ship.CardId);        
        shipTransform.SetParent(shipArea);

        RemoveConstructionPanel(ship);
    }

    public void RemoveConstructionPanel(Ship ship)
    {
        Transform shipTransform = FindCardTransformById(ship.CardId);
        Destroy(shipTransform.Find("ConstructionPanel").gameObject);
    }

    public void AddGameLogMessage(string message)
    {
        var entry = Instantiate(GameLogEntryPrefab);
        Text t = (Text)entry.GetComponent(typeof(Text));
        t.text = message;
        entry.SetParent(GameLogContent);

        var scrollRect = (ScrollRect)GameLog.GetComponent(typeof(ScrollRect));
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private Transform InstantiateCardPrefab(Card card)
    {
        Transform cardPrefab = CardPrefabFactory.CreateCardPrefab(card);

        // store in lookup so we can find it by its id
        _transformById[card.CardId] = cardPrefab;

        return cardPrefab;
    }

    public void UpdateOpponentStateGUI(Player opponent)
    {
        SetText(OpponentClicksGUI, opponent.Clicks.ToString());
        SetText(OpponentCreditsGUI, opponent.Credits.ToString());
        SetText(OpponentCardsInHandGUI, opponent.Hand.Count.ToString());
        SetText(OpponentCardsInDeckGUI, opponent.Deck.GetCount().ToString());
        SetText(OpponentCardsInDiscardGUI, opponent.Discard.Count.ToString());
    }

    public void UpdatePlayerStateGUI(Player player)
    {
        SetText(PlayerClicksGUI, player.Clicks.ToString());
        SetText(PlayerCreditsGUI, player.Credits.ToString());
        SetText(PlayerCardsInDeckGUI, player.Deck.GetCount().ToString());
        SetText(PlayerCardsInDiscardGUI, player.Discard.Count.ToString());
    }

    private Transform FindCardTransformById(string cardId)
    {
        return _transformById[cardId];
    }
}
