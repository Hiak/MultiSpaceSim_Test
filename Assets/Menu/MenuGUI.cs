using UnityEngine;
using System.Collections;
using System.Xml;
using Assets.Menu.GUI_Addons;
using Assets.Menu;



public class MenuGUI : MonoBehaviour, MenuObject
{

    public GUIStyle style;
    public GUIStyle onStyle;
    public GUIStyle offStyle;
   // public GUISettings settings;
   // public GUISkin skin;

    public Camera mainMenuCamera;
    public GameObject shipAssemblyCamera;
    public GameObject shipAssemblyShipPosition;

    ServerBrowser serverBrowser;
    ShipAssembly shipAssembly;
    
    MenuObject activeChildMenu;

    Rect menuSize;

    void Awake()
    {
        serverBrowser = new ServerBrowser();
        shipAssembly = new ShipAssembly(shipAssemblyCamera, shipAssemblyShipPosition);
        //menuState = MenuState.shipAssembly;
        activeChildMenu = null;

        int w=Screen.width;
        int h=Screen.height;

        menuSize = new Rect(w / 3.0f, 0.1f * h, w / 3.0f, 0.8f * h);
    }


    void ChangeMenu(MenuObject newActive) {

        activeChildMenu = newActive;
        this.End();
        activeChildMenu.Start();
        
    }

    void ChildMenuReturns() {

        activeChildMenu.End();
        activeChildMenu = null;
        this.Start();
    
    }


    public void OnGUI()
    {
        if (this.DrawGUI() == MenuObjectState.finished)
        {
            Application.Quit();
        }
    }


    public MenuObjectState DrawGUI(){

        if (activeChildMenu == null)
        {

            if (GUI.Button(new Rect(0, 0, 100, 50), "Quick Offline"))
                Application.LoadLevel("MapTest");


            GUI.Box(menuSize, "Menu");

            GUI.BeginGroup(menuSize);

                if (GUI.Button(new Rect(0, 1 * menuSize.height / 5, menuSize.width, menuSize.height / 5), "Server Browser"))
                    ChangeMenu(serverBrowser);

                if (GUI.Button(new Rect(0, 2 * menuSize.height / 5, menuSize.width, menuSize.height / 5), "Ship Assembly"))
                    ChangeMenu(shipAssembly);                    

                if (GUI.Button(new Rect(0, 3 * menuSize.height / 5, menuSize.width, menuSize.height / 5), "Exit"))
                    return MenuObjectState.finished;

            GUI.EndGroup();
        }


        else
        {
            if (activeChildMenu.DrawGUI() == MenuObjectState.finished)
                activeChildMenu = null;
        }
        return MenuObjectState.active;
    }


    // Use this for initialization
    public void Start()
    {
        mainMenuCamera.gameObject.SetActive(true);
	}
    
	// Update is called once per frame
    public void Update()
    {
        if(activeChildMenu!=null)
            activeChildMenu.Update();

	}

    public void End()
    {
        mainMenuCamera.gameObject.SetActive(false);
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