#if ENABLE_UNET

namespace UnityEngine.Networking
{
    //[AddComponentMenu("Network/NetworkManagerHUD")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class LogonGUI : MonoBehaviour
    {
        public string playerName;
        public NetworkManager manager;
        [SerializeField]
        public bool showGUI = true;
        [SerializeField]
        public int offsetX;
        [SerializeField]
        public int offsetY;

        // Runtime variable
       // bool showServer = false;

        void Start()
        {
            playerName = "DefaultPlayer";
        }

        void Awake()
        {
            manager = GetComponent<NetworkManager>();
        }

        void Update()
        {

        }

        public void SetPlayerName(string name)
        {
            playerName = name;
        }

        public void StartServer()
        {
            manager.StartServer();
        }

        public void StartClient()
        {
            manager.StartClient();

            //ClientScene.Ready(manager.client.connection);
            //if (ClientScene.localPlayers.Count == 0)
            //{
            //    ClientScene.AddPlayer(0);
            //}
            
        }

        void OnGUI()
        {
            if (!showGUI)
                return;

            //int xpos = 10 + offsetX;
            //int ypos = 40 + offsetY;
            //int spacing = 24;

            //if (!NetworkClient.active && !NetworkServer.active)
            //{
            //    playerName = GUI.TextField(new Rect(xpos, ypos, 95, 20), playerName);
            //    ypos += spacing;
                              
            //    if (GUI.Button(new Rect(xpos, ypos, 105, 20), "Start client"))
            //    {
            //        manager.StartClient();
            //    }
            //    manager.networkAddress = GUI.TextField(new Rect(xpos + 100, ypos, 95, 20), manager.networkAddress);
            //    ypos += spacing;

            //    if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Start server"))
            //    {
            //        manager.StartServer();
            //    }
            //    ypos += spacing;
            //}
            //else
            //{
            //    if (NetworkServer.active)
            //    {
            //        GUI.Label(new Rect(xpos, ypos, 300, 20), "Server: port=" + manager.networkPort);
            //        ypos += spacing;
            //    }
            //    if (NetworkClient.active)
            //    {
            //        GUI.Label(new Rect(xpos, ypos, 300, 20), "Client: address=" + manager.networkAddress + " port=" + manager.networkPort);
            //        ypos += spacing;
            //    }
            //}

            //if (NetworkClient.active && !ClientScene.ready)
            //{
            //    if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Client Ready"))
            //    {
            //        ClientScene.Ready(manager.client.connection);

            //        if (ClientScene.localPlayers.Count == 0)
            //        {
            //            ClientScene.AddPlayer(0);
            //        }
            //    }
            //    ypos += spacing;
            //}

            //if (NetworkServer.active || NetworkClient.active)
            //{
            //    if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Stop"))
            //    {
            //        manager.StopHost();
            //    }
            //    ypos += spacing;
            //}            
        }
    }
};
#endif //ENABLE_UNET
