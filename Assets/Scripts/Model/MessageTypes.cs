using UnityEngine.Networking;
using System;

public class MessageTypes
{
    public enum MessageType
    {       
        JOIN_LOBBY = 1000,      // C -> S join the lobby
        CREATE_GAME = 1001,     // C -> S request a game be created with the client as "host"
        CANCEL_GAME = 1002,     // C -> S request that the game he is "hosting" be stopped
        CHAT_MESSAGE = 1003,    // C <-> S simple string messages
        GAME_NUMBER = 1004,     // C <- S inform them of the game they are hosting/joining
        GAME_LIST = 1005,       // C <- S update the list of games
        JOIN_GAME = 1006,       // C -> S request to join an existing game 
        START_GAME = 1007,      // C <- S tell the client to load the gameclient scene
        PLAYER_LIST = 1008,     // C <- S update the list of players
        PLAYER_NAME = 1009,     // C <- S inform them of their name (might have been made unique by adding a number)
        PLAYER_READY = 2000,    // C -> S say they are ready to start the game
        SETUP_GAME = 2001,      // S -> C both players are ready 
        DRAWN_CARD = 2002,      // S -> C when a player draws a card from their deck
        CREDITS = 2003,         // C <- S update their current credits
        CLICKS = 2004,          // C <- S update their current clicks (i.e action points/logistics points)
        OPPONENT_STATE = 2005,  // C <- S update the basic state of their opponent (credits, clicks, cards in hand) 
        GAME_STATE = 2006,      // C <- S update the game state (i.e. phase)
        CLICK_FOR_CARD = 2007,  // C -> S spend a click to draw a card  
        CLICK_FOR_CREDIT = 2008, // C -> S spend a click to gain a credit
        SHIPYARD = 2009,        // S -> C when the player gets a new shipyard
        HOST_SHIP = 2010,       // S -> C when opponent hosts a card (usually in logistics resolution)
        SEND_ACTIONS = 2011,   // C <-> S to submit actions for the turn (then server transmits to opponent to update local state)
        GAME_LOG = 2012          // S -> C to send details of game actions to display 
    }

    public class ChatMessage : MessageBase          { public string message; }
    public class JoinLobbyMessage : MessageBase     { public string playerName; }
    public class CreateGameMessage : MessageBase    { }
    public class CancelGameMessage : MessageBase    { public int gameNumber; }
    public class GameNumberMessage : MessageBase    { public int gameNumber; }
    public class GameListMessage : MessageBase      { public string gameListData; }
    public class PlayerListMessage : MessageBase    { public string playerListData; }
    public class PlayerNameMessage : MessageBase    { public string playerName; }
    public class JoinGameMessage : MessageBase      { public int gameNumber; }
    public class StartGameMessage : MessageBase     { public string opponentName; }
    public class PlayerReadyMessage : MessageBase   { }
    public class SetupGameMessage : MessageBase     { public string playerName; public string opponentName; }
    public class DrawnCardMessage : MessageBase     { public string CardCodename; public string cardId; public int cardsInDeck; } // possibly not the best place to pass cards in deck?
    public class ShipyardMessage : MessageBase      { public string shipyardType; public string shipyardId; public bool player; } // player = belongs to player, false = belongs to opponent. Might not stay this way
    public class CreditsMessage : MessageBase       { public int credits; }
    public class ClicksMessage : MessageBase        { public int clicks; }
    public class OpponentStateMessage : MessageBase { public int credits; public int clicks; public int cardsInHand; public int cardsInDeck; public int cardsInDiscard; }
    public class GameStateMessage : MessageBase     { public string state; }
    public class ClickForCardMessage : MessageBase  { }
    public class ClickForCreditMessage : MessageBase { }
    public class HostShipMessage : MessageBase      { public string CardCodename; public string cardId; public string shipyardId; }
    public class SendActionsMessage : MessageBase   { public string actionData; }
    public class GameLogMessage : MessageBase       { public string message; }
}
