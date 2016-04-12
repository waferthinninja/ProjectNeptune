using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Text;

public class LobbyController : NetworkBehaviour {

    public Dictionary<int, Player> _playerConnections; // connectionId -> player
    public Dictionary<int, Game> _games;               // gameId -> game
    public Dictionary<int, int> _gameConnections;      // connectionId -> gameId
    private List<Player> _playersInLobby; // NOTE at the moment we remove players from the lobby when they create a game, though they should still get lobby messgages?
    private NetworkClient _client;
    private int _gameNumber;

    public Player LocalPlayer;// for the client to store their own player object
    public Player Opponent;

    public Transform GameListPanelContent;
    public Transform PlayerListPanelContent;
    public Transform ChatPanelContent;
    public Transform ChatPanel;
    public Transform DeckSelectDialog;
    public Transform ChatMessagePrefab;
    public Transform PlayerListEntry;
    public Transform JoinableGameListEntry;
    public Transform UnjoinableGameListEntry;

    void Start()
    {
        //_chatLog = new List<string>();        
        _playersInLobby = new List<Player>();
        _playerConnections = new Dictionary<int, Player>();
        _gameConnections = new Dictionary<int, int>();
        _games = new Dictionary<int, Game>();
        _gameNumber = -1;

        // enable the lobby gui 
        EnableDisableGameClientGUI(false);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            DoSendMessage();
        }
    }

    // hook into NetworkManager client setup process
    public override void OnStartClient()
    {
        base.OnStartClient(); // base implementation is currently empty

        _client = NetworkManager.singleton.client;

        //Debug.Log("LobbyController _client=" + _client.ToString());

        _client.RegisterHandler((short)MessageTypes.MessageType.CHAT_MESSAGE, OnClientChatMessage);
        _client.RegisterHandler((short)MessageTypes.MessageType.GAME_NUMBER, OnGameNumberMessage);
        _client.RegisterHandler((short)MessageTypes.MessageType.GAME_LIST, OnGameListMessage);
        _client.RegisterHandler((short)MessageTypes.MessageType.PLAYER_LIST, OnPlayerListMessage);
        _client.RegisterHandler((short)MessageTypes.MessageType.START_GAME, OnStartGameMessage);
        _client.RegisterHandler((short)MessageTypes.MessageType.PLAYER_NAME, OnPlayerNameMessage);
    }

    // hook into NetManagers server setup process
    public override void OnStartServer()
    {
        //Debug.Log("LobbyController OnStartServer");
        base.OnStartServer(); //base is empty

        NetworkServer.RegisterHandler((short)MessageTypes.MessageType.CHAT_MESSAGE, OnServerChatMessage);
        NetworkServer.RegisterHandler((short)MessageTypes.MessageType.JOIN_LOBBY, OnJoinLobbyMessage);
        NetworkServer.RegisterHandler((short)MessageTypes.MessageType.CREATE_GAME, OnCreateGameMessage);
        NetworkServer.RegisterHandler((short)MessageTypes.MessageType.CANCEL_GAME, OnCancelGameMessage);
        NetworkServer.RegisterHandler((short)MessageTypes.MessageType.JOIN_GAME, OnJoinGameMessage);

        // move camera to the ServerGUI
        Camera.main.transform.position = new Vector3(-2000, 0, -10);
    }

    public void DoSendMessage()
    {
        //Debug.Log("DoSendMessage");
        var chatInputField = GameObject.Find("ChatInputField");
        var t = (InputField)chatInputField.GetComponent(typeof(InputField));
        t.Select();
        t.ActivateInputField();
        if (!string.IsNullOrEmpty(t.text))
        {
            MessageTypes.ChatMessage msg = new MessageTypes.ChatMessage();
            msg.message = t.text;
            NetworkManager.singleton.client.Send((short)MessageTypes.MessageType.CHAT_MESSAGE, msg);
            t.text = String.Empty;
        }
    }

    public void CreateGame()
    {
        MessageTypes.CreateGameMessage msg = new MessageTypes.CreateGameMessage();
        NetworkManager.singleton.client.Send((short)MessageTypes.MessageType.CREATE_GAME, msg);

        var createGameButton = (Button)GameObject.Find("CreateGameButton").GetComponent(typeof(Button));
        createGameButton.interactable = false;

        var cancelGameButton = (Button)GameObject.Find("CancelGameButton").GetComponent(typeof(Button));
        cancelGameButton.interactable = true;
    }

    public void CancelGame()
    {
        MessageTypes.CancelGameMessage msg = new MessageTypes.CancelGameMessage();
        msg.gameNumber = _gameNumber;
        NetworkManager.singleton.client.Send((short)MessageTypes.MessageType.CANCEL_GAME, msg);

        var createGameButton = (Button)GameObject.Find("CreateGameButton").GetComponent(typeof(Button));
        createGameButton.interactable = true;

        var cancelGameButton = (Button)GameObject.Find("CancelGameButton").GetComponent(typeof(Button));
        cancelGameButton.interactable = false;
    }

    private void ResendLists()
    {
        SendGameListToClients();
        SendPlayerListToClients();
    }

    private void OnServerChatMessage(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MessageTypes.ChatMessage>();
        //Debug.Log("New chat message on server: " + msg.message);

        MessageTypes.ChatMessage chat = new MessageTypes.ChatMessage();
        chat.message = _playerConnections[netMsg.conn.connectionId].Name + ": " + msg.message;

        NetworkServer.SendToAll((short)MessageTypes.MessageType.CHAT_MESSAGE, chat);
    }

    private void OnClientChatMessage(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MessageTypes.ChatMessage>();
        //Debug.Log("New chat message on client: " + msg.message);
        
        var chatMessage = Instantiate(ChatMessagePrefab);
        Text t = (Text)chatMessage.GetComponent(typeof(Text));
        t.text = msg.message;
        chatMessage.SetParent(ChatPanelContent);

        var scrollRect = (ScrollRect)ChatPanel.GetComponent(typeof(ScrollRect));
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private void OnGameNumberMessage(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MessageTypes.GameNumberMessage>();
        Debug.Log("Received game number " + msg.gameNumber);
        _gameNumber = msg.gameNumber;
    }
    private void OnPlayerNameMessage(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage <MessageTypes.PlayerNameMessage>();

        LocalPlayer = new Player(msg.playerName, 0);

        Debug.Log("Received player name " + msg.playerName);        
    }

    private void OnGameListMessage(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MessageTypes.GameListMessage>();
        string[] data = msg.gameListData.Split('|');
        //Debug.Log("Received game list with " + data[0] + " entries: " + msg.gameListData);

        // clear the game list gui
        var children = new List<GameObject>();
        foreach (Transform child in GameListPanelContent) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
        
        // populate the list with the new data
        int gameCount = int.Parse(data[0]);
        for (int i = 0; i < gameCount; i++)
        {
            AddGameListEntry(int.Parse(data[3 * i + 1]), 
                data[3 * i + 2] == "1", 
                data[3 * i + 3]);
        }
    }

    private void OnPlayerListMessage(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MessageTypes.PlayerListMessage>();
        string[] data = msg.playerListData.Split('|');
        //Debug.Log(string.Format("Received player list with {0} entries: {1}", data.Length - 1, msg.playerListData));

        // clear the player list gui
        var children = new List<GameObject>();
        foreach (Transform child in PlayerListPanelContent) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));

        // populate the list with the new data
        foreach (string playerName in data)
        {
            if (playerName != "")
                AddPlayerListEntry(playerName);
        }
    }

    private void AddPlayerListEntry(string playerName)
    {
        Transform entry = Instantiate(PlayerListEntry);

        // set the players text
        var textElement = (Text)entry.GetComponentInChildren(typeof(Text));
        textElement.text = playerName;

        // set parent to be the player list content panel
        entry.SetParent(PlayerListPanelContent);
    }

    private void AddGameListEntry(int gameNumber, bool joinable, string players)
    {
        // override joinable for own game (can't join own game)
        joinable = joinable && gameNumber != _gameNumber;

        // Instantiate the appropriate prefab
        Transform entry = (joinable ? Instantiate(JoinableGameListEntry) : Instantiate(UnjoinableGameListEntry));
        
        // set the players text
        var textElement = (Text)entry.GetComponentInChildren(typeof(Text));
        textElement.text = players;

        // if joinable, set the join button to join the gameNumber
        if (joinable) 
        {
            var buttonElement = (Button)entry.GetComponentInChildren(typeof(Button));
            buttonElement.onClick.AddListener(() => JoinGame(gameNumber));
        }

        // set parent to be the gamelistcontent panel
        entry.SetParent(GameListPanelContent);
    }

    private void JoinGame(int gameNumber)
    {
        //Debug.Log("Attempting to join game number " + gameNumber.ToString());
        MessageTypes.JoinGameMessage msg = new MessageTypes.JoinGameMessage();
        msg.gameNumber = gameNumber;
        _client.Send((short)MessageTypes.MessageType.JOIN_GAME, msg);
    }

    private void OnJoinLobbyMessage(NetworkMessage netMsg)
    {
        var playerName = netMsg.ReadMessage<MessageTypes.JoinLobbyMessage>().playerName;
        playerName = MakeUnique(playerName);

        int connectionId = netMsg.conn.connectionId;
        //Debug.Log(String.Format("Adding player {0} with connectionId {1} to lobby",  playerName, connectionId));
        
        // create player object, add to list of players in lobby
        Player player = new Player(playerName, connectionId);
        _playersInLobby.Add(player);
        _playerConnections[connectionId] = player;

        // send the player his unique name
        var msg = new MessageTypes.PlayerNameMessage();
        msg.playerName = playerName;
        NetworkServer.SendToClient(connectionId, (short)MessageTypes.MessageType.PLAYER_NAME, msg);

        // send game list to the player
        ResendLists();
    }

    private string MakeUnique(string playerName)
    {
        string originalPlayerName = playerName;        
        bool unique = false;
        int i = 0;
        while (!unique)
        {
            bool found = false;
            foreach(Player player in _playerConnections.Values)
            {
                if (player.Name == playerName)
                {
                    found = true;
                }
            }
            if (found)
            {
                i++;
                playerName = originalPlayerName + i.ToString();
            }
            else
            {
                unique = true;
            }
        }
        return playerName;  
    }

    private void OnCreateGameMessage(NetworkMessage netMsg)
    {
        // determine player from the connectionId
        int connectionId = netMsg.conn.connectionId;
        var player = _playerConnections[connectionId];

        // check player is in the lobby
        if (_playersInLobby.Contains(player))
        {
            // create a game object
            _gameNumber++;
            Game game = new Game(_gameNumber, player);
            _games[_gameNumber] = game;
            _gameConnections[connectionId] = _gameNumber;

            // send the game number to the client
            var msg = new MessageTypes.GameNumberMessage();
            msg.gameNumber = _gameNumber;
            NetworkServer.SendToClient(connectionId, (short)MessageTypes.MessageType.GAME_NUMBER, msg);

            // remove player from lobby
            _playersInLobby.Remove(player);

            // resend game list to all clients
            ResendLists();

            //Debug.Log(String.Format("Created game {0} for player {1}", _gameNumber, player.Name));
        }
        else
        {
            Debug.LogError("Trying to create game for player not in the lobby");
        }
    }

    private void SendGameListToClients()
    {
        MessageTypes.GameListMessage gameList = new MessageTypes.GameListMessage();
        gameList.gameListData = SerializeGameList();
        NetworkServer.SendToAll((short)MessageTypes.MessageType.GAME_LIST, gameList);
    }

    private void SendPlayerListToClients()
    {
        MessageTypes.PlayerListMessage playerList = new MessageTypes.PlayerListMessage();
        playerList.playerListData = SerializePlayerList();
        NetworkServer.SendToAll((short)MessageTypes.MessageType.PLAYER_LIST, playerList);
    }

    private string SerializeGameList()
    {
        StringBuilder sb = new StringBuilder();
        // header is {number of entries}
        sb.Append(_games.Keys.Count);        

        // then |GameNum|{Joinable}|Playername(s) for each 
        foreach (int key in _games.Keys)
        {
            sb.Append("|");
            sb.Append(key);
            sb.Append("|");
            sb.Append(_games[key].GamePhase == GamePhase.AWAITING_CHALLENGER ? "1" : "0");
            sb.Append("|");
            sb.Append(_games[key].Player.Name);
            sb.Append(_games[key].GamePhase == GamePhase.AWAITING_CHALLENGER ? "" : " vs " + _games[key].Opponent.Name );
        }

        return sb.ToString();
    }

    private string SerializePlayerList()
    {
        StringBuilder sb = new StringBuilder();

        foreach (Player player in _playersInLobby)
        {
            sb.Append(player.Name);
            sb.Append("|");
        }

        foreach (int key in _games.Keys)
        {            
            if (_games[key].GamePhase == GamePhase.AWAITING_CHALLENGER)
            {
                sb.Append(_games[key].Player.Name + " (Waiting)");
                sb.Append("|");
            }
            else
            {
                sb.Append(_games[key].Player.Name + " (Playing)");
                sb.Append("|");
                sb.Append(_games[key].Opponent.Name + " (Playing)");
                sb.Append("|");
            }
        }

        return sb.ToString();
    }

    private void OnCancelGameMessage(NetworkMessage netMsg)
    {
        // determine player from the connectionId
        int connectionId = netMsg.conn.connectionId;
        var player = _playerConnections[connectionId];

        int gameNumber = netMsg.ReadMessage<MessageTypes.CancelGameMessage>().gameNumber;
        // check player created the game
        if(_games[gameNumber].Player == player)
        {
            // Remove the game
            _games.Remove(gameNumber);  

            // return player to lobby
            _playersInLobby.Add(player);

            // resend game list to all clients
            ResendLists();

            //Debug.Log(String.Format("Cancelling game {0} created by {1}", gameNumber, player.Name));
        }
        else
        {
            Debug.LogError("Trying to cancel game for player who did not create it");
        }
    }

    private void OnJoinGameMessage(NetworkMessage netMsg)
    {
        // determine player from the connectionId
        int connectionId = netMsg.conn.connectionId;
        var player = _playerConnections[connectionId];

        int gameNumber = netMsg.ReadMessage<MessageTypes.CancelGameMessage>().gameNumber;
        Game game = _games[gameNumber];

        // check player is in the lobby
        if (_playersInLobby.Contains(player)) // TODO - check isn't already in another game? Unnecessary?
        {
            // add player to the game 
            game.AddOpponent(player);
            _gameConnections[connectionId] = gameNumber;

            // send the game number to the client
            var msg = new MessageTypes.GameNumberMessage();
            msg.gameNumber = gameNumber;
            NetworkServer.SendToClient(connectionId, (short)MessageTypes.MessageType.GAME_NUMBER, msg);

            // transition players to the game scene
            var msg2 = new MessageTypes.StartGameMessage();
            msg2.opponentName = game.Opponent.Name;
            NetworkServer.SendToClient(game.Player.ConnectionId, (short)MessageTypes.MessageType.START_GAME, msg2);
            msg2.opponentName = game.Player.Name;
            NetworkServer.SendToClient(game.Opponent.ConnectionId, (short)MessageTypes.MessageType.START_GAME, msg2);

            // remove player from lobby
            _playersInLobby.Remove(player);

            // resend lists to all clients
            ResendLists();

            //Debug.Log(String.Format("Added player {1} to game {0}", _gameNumber, player.Name));
        }
        else
        {
            Debug.LogError("Trying to add a player not in the lobby to a game");
        }
    }

    private void OnStartGameMessage(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MessageTypes.StartGameMessage>();
        Opponent = new Player(msg.opponentName, 0);
        
        // show the game gui and hide the lobby gui 
        EnableDisableGameClientGUI(true);

        // also show the deck select dialog
        DeckSelectDialog.position = new Vector3(
            Camera.main.transform.position.x,
            Camera.main.transform.position.y, 
            0);
        DeckSelectDialog.gameObject.SetActive(true);
    }

    private void EnableDisableGameClientGUI(bool enable)
    {
        if (!isServer)
        {
            // point camera at the gui or the lobby as appropriate
            Camera.main.transform.position = new Vector3((enable ? 2000 : 0), 0, -10);
        }
    }
}
