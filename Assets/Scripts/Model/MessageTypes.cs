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
        PLAYER_READY = 2000,    // C -> S say they are ready to start the game - and send the deck they will be playing with
        SETUP_GAME = 2001,      // S -> C both players are ready  - and send the opponents "public" deck details
        DRAWN_CARD = 2002,      // S -> C when a player draws a card from their deck
        ACTIONS = 2003,         // C <-> S to submit actions for the turn (then server transmits to opponent to update local state)
        GAME_LOG = 2004,        // S -> C to send details of game actions to display 
        DECK_FIRST = 2005,      // C <-> S probably a clumsy way to handle this, but deck too large to send in one go so we send these chunks
        DECK_FRAGMENT = 2006    //          first one tells to start storing it, the rest are appended, then some other message will use the full data
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
    public class PlayerReadyMessage : MessageBase   {  }
    public class SetupGameMessage : MessageBase     {  }
    public class DrawnCardMessage : MessageBase     { public string CardCodename; public string cardId; } 
    public class ActionsMessage : MessageBase       { public string actionData; }
    public class GameLogMessage : MessageBase       { public string message; }
    public class DeckFirstMessage : MessageBase     { public string deckDataFragment; }
    public class DeckFragmentMessage : MessageBase  { public string deckDataFragment; }
}
