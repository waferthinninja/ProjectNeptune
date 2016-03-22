using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {
    private NetworkClient mClient;

    // Use this for initialization
    public override void OnStartLocalPlayer()
    {
        if (isLocalPlayer)
        {
            mClient = NetworkManager.singleton.client;

            // get the player name entered on the login screen
            string playerName = FindObjectOfType<LogonGUI>().playerName;
            MessageTypes.JoinLobbyMessage msg = new MessageTypes.JoinLobbyMessage();
            msg.playerName = playerName;
            Debug.Log(String.Format("Playername={0} ConnectionId={1}", playerName,mClient.connection.connectionId));
            mClient.Send((short)MessageTypes.MessageType.JOIN_LOBBY, msg);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
