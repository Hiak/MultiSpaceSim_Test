using UnityEngine;
using System.Collections;
using Assets.Menu;

public class ServerBrowser : MenuObject{
    
    HostData[] serverList=new HostData[0];
    string[] serverNames=new string[0];

    public ServerBrowser() {

    }

    MenuObject activeChildMenu;

    //test only
    int searchServersCntr = 0;
    void searchServers() {

        //Debug.Log("Request");
        //MasterServer.RequestHostList("IWI");

        NetworkManager.SearchServers();

    }

    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        //Debug.Log("Event: " + msEvent);

        if (msEvent == MasterServerEvent.HostListReceived)
        {

            serverList = MasterServer.PollHostList();

            //Debug.Log(serverList.Length);

            serverNames = new string[serverList.Length];

            for (int i = 0; i < serverList.Length; i++)
            {
                serverNames[i] = serverList[i].gameName;
            }
        }
    }

    void OnServerInitialized()
    {
        Debug.Log("OnServerInitialized");

        //Network.SetLevelPrefix(1);

        Application.LoadLevel("MapTest");
    }


    void OnConnectedToServer()
    {
        Debug.Log("OnConnectedToServer");

        //Network.SetLevelPrefix(1);

        var list = MonoBehaviour.FindObjectsOfType<NetworkView>();
        
        Application.LoadLevel("MapTest");
    }


    int firstSeparator = 500;
    Vector2 scrollPos = Vector2.zero;
    int serverSelectionIter=-1;
    int serverBoxHeight = 30;

    public MenuObjectState DrawGUI() {

        if (activeChildMenu != null)
        {

            if (activeChildMenu.DrawGUI() == MenuObjectState.finished)
                activeChildMenu = null;

            return MenuObjectState.active;
        }


        GUI.Box(new Rect(5, 5, firstSeparator - 10, Screen.height - 135), "Server List");

        Rect scrollInside = new Rect(0, 0, firstSeparator - 36, serverBoxHeight * serverList.Length);

        scrollPos = GUI.BeginScrollView(new Rect(10, 35, firstSeparator - 20, Screen.height - 170), scrollPos, scrollInside);

            serverSelectionIter = GUI.SelectionGrid(scrollInside, serverSelectionIter, serverNames, 1);

        GUI.EndScrollView();


        if (GUI.Button(new Rect(5, Screen.height - 60, firstSeparator - 10, 55), "Back"))
            return MenuObjectState.finished;

        if (GUI.Button(new Rect(5, Screen.height - 120, firstSeparator / 2 - 10, 55), "CreateServer"))
        {
            activeChildMenu = new ServerCreator();
            activeChildMenu.Start();
        }

        if (GUI.Button(new Rect(5 + firstSeparator / 2, Screen.height - 120, firstSeparator / 2 - 10, 55), "Update Server List"))
            searchServers();


        GUI.Box(new Rect(firstSeparator + 5, 5,Screen.width - firstSeparator - 10, Screen.height - 110), "Server Details");

        if (GUI.Button(new Rect(firstSeparator + 5, Screen.height - 100, Screen.width - firstSeparator - 10, 95), "Connect to Server"))
        {
            if (serverSelectionIter >= 0)
            {
                NetworkManager.JoinServer(serverList[serverSelectionIter]);

                //NetworkManager.LoadLevelAsClient("MapTest", serverList[serverSelectionIter]);
            }
        }
            //return serverList[serverSelectionIter];

        return MenuObjectState.active;
    
    }




    public void Update()
    {
        
    }

    public void Start()
    {
        NetworkManager.onMasterServerEvent += OnMasterServerEvent;
        NetworkManager.onServerInitialised += OnServerInitialized;
        NetworkManager.onConnenctedToServer += OnConnectedToServer;

        searchServers();

    }

    public void End()
    {

    }

    public object GetReturnData()
    {
        throw new System.NotImplementedException();
    }

    public MenuObjectReturnType GetReturnType()
    {
        throw new System.NotImplementedException();
    }
}