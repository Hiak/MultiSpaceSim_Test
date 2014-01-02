using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts;

public class NetworkManager : MonoBehaviour {

    public enum ConnectionState
    {
        offline,
        client,
        server
    }

    public static event Action<MasterServerEvent> onMasterServerEvent;
    public static event Action onConnenctedToServer;
    public static event Action onServerInitialised;

    public static ConnectionState connectionState = ConnectionState.offline;

	// Use this for initialization
	void Start () {
        
	}

    void Awake() { Network.SetLevelPrefix(0); }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (onMasterServerEvent != null)
            onMasterServerEvent(msEvent);
    }

    void OnServerInitialized()
    {
        if (onServerInitialised != null)
            onServerInitialised();
        //Debug.Log("Server Initializied");
    }


    void OnConnectedToServer()
    {
        if (onConnenctedToServer != null)
            onConnenctedToServer();
    }


    //static void StartS() {  }

    private const string typeName = "IWI";
    //private const string gameName = "HiakRoom";

    public static void StartServer(string serverName)
    {
        //TODO:Connection number not 4
        Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(typeName, serverName);
        connectionState = ConnectionState.server;

    }

    public static void SearchServers()
    {
        //Debug.Log("Request");
        MasterServer.RequestHostList(typeName);
    }


    public static void JoinServer(HostData hostData)
    {
        Network.Connect(hostData);
        connectionState = ConnectionState.client;
    }

    /*
    public static void LoadLevelAsServer(string levelName, HostData newHost) {

        InterSceneContainer.Add("connection type", "server");

        InterSceneContainer.Add("hostData", newHost);

        NetworkView.Find

        Application.LoadLevel(levelName);

        //StartServer(newHost.gameName);
    }

    public static void LoadLevelAsClient(string levelName, HostData host)
    {
        InterSceneContainer.Add("connection type", "client");

        InterSceneContainer.Add("hostData", host);

        Application.LoadLevel(levelName);

        //JoinServer(host);
    }*/

}
