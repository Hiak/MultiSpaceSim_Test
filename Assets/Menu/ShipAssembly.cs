using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Assets.Menu;
using Assets.Resources.ShipParts;
using Assets.Scripts.Ship;
using System;
using Assets.Scripts;

class ShipPartVisual : Assets.Menu.GUI_Addons.TreeViewNodeVisual
{
    public ShipPartVisual(ShipAssembly _assembly, List<ShipPart> _sourceList, bool initialElementOn, string _name)
    {
        ID = 0;
        shipAssembly = _assembly;
        sourceList = _sourceList;
        name = _name;
        if (initialElementOn) { part = sourceList[0]; selectedIndex = 0; }
    }

    public ShipPartVisual(ShipAssembly _assembly, List<ShipPart> _sourceList, bool initialElementOn, string _name, ShipPartVisual _parent, int _ID)
    {
        shipAssembly = _assembly;
        ID = _ID;
        sourceList = _sourceList;
        if (initialElementOn) { part = sourceList[0]; selectedIndex = 0; }
        parent = _parent;
        name = _name;
    }

    ShipAssembly shipAssembly;
    public List<ShipPart> sourceList;
    public ShipPart part;
    List<ShipPartVisual> children = new List<ShipPartVisual>();
    ShipPartVisual parent;
    int topLevelChildNum = 1;
    string name;
    int selectedIndex = -1;
    int ID;


    void setTopLevelChildNum(int delta) { if (parent != null)parent.setTopLevelChildNum(delta); else topLevelChildNum += delta; }

    int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    void changeSelection(bool direction)
    {
        if (sourceList == null || sourceList.Count == 0)
            return;

        shipAssembly.DecoupleShip();

        ShipPart selectedPart = null;

        if (direction)
        {
            selectedIndex = mod((selectedIndex + 1), sourceList.Count);
            selectedPart = sourceList[selectedIndex];
        }
        else
        {
            selectedIndex =  mod((selectedIndex - 1), sourceList.Count);
            selectedPart = sourceList[selectedIndex];
        }

        if (part != null)
            part.RemoveRes();
        part = selectedPart.Instantiate();

        if (parent != null) parent.part.Joints[ID].subPart = part;

        children.Clear();

        int i = 0;

        foreach (var joint in part.Joints)
        {
            List<ShipPart> selectedSourceList;

            switch (joint.Type)
            {
                case "Fuselage": selectedSourceList = shipAssembly.FuselageList; break;
                case "Wing": selectedSourceList = shipAssembly.WingList; break;
                case "Engine": selectedSourceList = shipAssembly.EngineList; break;
                default: selectedSourceList = null; break;
            }

            children.Add(new ShipPartVisual(shipAssembly, selectedSourceList, false, joint.DisplayName, this, i));
            i++;
        }

        shipAssembly.BuildShip();
        childrenOutOfDate();
    }



    public Vector2 Size { get { return boxSize; } set { } }

    public event Assets.Menu.GUI_Addons.childrenOutOfDateHandler childrenOutOfDate;

    static Vector2 buttonSize = new Vector2(20, 20);
    Vector2 boxSize = new Vector2(200, 40);


    public bool Draw(Vector2 place, bool open)
    {
        GUI.Box(new Rect(place.x, place.y, boxSize.x, boxSize.y), name);
        if (GUI.Button(new Rect(place.x, place.y + boxSize.y - buttonSize.y, buttonSize.x, buttonSize.y), "<"))
            changeSelection(false);
        GUI.Box(new Rect(place.x + buttonSize.x, place.y + boxSize.y - buttonSize.y, boxSize.x - 2 * buttonSize.x, buttonSize.y),
            part == null ? "Not Selected" : part.Name);
        if (GUI.Button(new Rect(place.x + boxSize.x - buttonSize.x, place.y + boxSize.y - buttonSize.y, buttonSize.x, buttonSize.y), ">"))
            changeSelection(true);

        if (part != null) return true;
        return false;
    }

    public ArrayList ReloadChildren()
    {
        return new ArrayList(children);
    }


}




public class ShipAssembly : MenuObject{

    internal List<ShipPart> FuselageList = new List<ShipPart>();
    internal List<ShipPart> WingList = new List<ShipPart>();
    internal List<ShipPart> EngineList = new List<ShipPart>();
    internal Dictionary<string,Stat> Stats=new Dictionary<string,Stat>();

    GameObject camera;
    GameObject shipPos;

    public ShipAssembly(GameObject shipAssemblyCamera, GameObject shipPosition)
    {
        LoadParts();
        topShipPartVisual = new ShipPartVisual(this,FuselageList, false, "Fuselage");
        treeView = new Assets.Menu.GUI_Addons.TreeView(new Vector2(0, 0), topShipPartVisual, 10, 10);
        camera = shipAssemblyCamera;
        shipPos = shipPosition;
    }

    void LoadParts() {

        TextAsset textAsset = (TextAsset)Resources.Load("ShipParts/shipParts");
        //Debug.Log("LoadPartsStart");
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.LoadXml(textAsset.text);

        XmlElement parts = xmldoc.DocumentElement;

        foreach (XmlNode part in parts.ChildNodes)
        {
            string type = part.Attributes["Type"].Value;
            //Debug.Log(type);

            switch (type)
            {
                case "Fuselage":
                    ShipPart f = new ShipPart(part);
                    FuselageList.Add(f);
                    break;

                case "Wing":
                    ShipPart w = new ShipPart(part);
                    WingList.Add(w);
                    break;

                case "Engine":
                    ShipPart e = new ShipPart(part);
                    EngineList.Add(e);
                    break;

                default:
                    break;
            }

        }

    }


    public void BuildShip() {

        topShipPartVisual.part.Res[0].transform.parent = shipPos.transform;

        RecursiveBuildShip(topShipPartVisual.part);

        shipPos.SetActiveRecursively(true);

        Stats.Clear();
        topShipPartVisual.part.CollectStats(Stats, new Dictionary<string, List<StatModifier>>());
    }


    public void DecoupleShip(){

        shipPos.SetActiveRecursively(false);

        RecursiveDecoupleShip(topShipPartVisual.part);
    
    }

    void RecursiveDecoupleShip(ShipPart part)
    {
        if (part == null)
            return;

        foreach (var res in part.Res)
        {
            //Debug.Log("a:" + res.Value.transform.parent);
            res.Value.transform.parent = null;
            //Debug.Log("b:" + res.Value.transform.parent);

        }

        foreach (var joint in part.Joints)
        {
            RecursiveDecoupleShip(joint.subPart);
        }
    
    
    }


    void RecursiveBuildShip(ShipPart part)
    {
        foreach (var joint in part.Joints)
        {
            if (joint.subPart != null)
            {
                foreach (var jointPosition in joint.Positions)
                {

                    GameObject childObj = joint.subPart.Res[jointPosition.Key];
                    Transform parentObj = jointPosition.Value.PositionObject;

                    
                    childObj.transform.parent = parentObj;
                    childObj.transform.position = parentObj.position;
                    childObj.transform.rotation = parentObj.rotation;

                }

                RecursiveBuildShip(joint.subPart);
            }
        }
    
    }
    

    ShipPartVisual topShipPartVisual;

    int firstSeparator = 250;
    int secondSeparator = 650;
    Vector2 scrollPos = Vector2.zero;
    Vector2 scrollPos2 = Vector2.zero;

    Assets.Menu.GUI_Addons.TreeView treeView;
    int statHeight = 20;


    public MenuObjectState DrawGUI()
    {

        GUI.Box(new Rect(5, 5, firstSeparator - 10, Screen.height - 70), "Ship Parts");

        Rect scrollInside = new Rect(0, 0, firstSeparator - 36, topShipPartVisual.Size.y + 10);

        scrollPos = GUI.BeginScrollView(new Rect(10, 35, firstSeparator - 20, Screen.height - 105), scrollPos, scrollInside);

            treeView.Draw();

        GUI.EndScrollView();


        if (GUI.Button(new Rect(5, Screen.height - 65,firstSeparator - 10, 60), "Back"))
            return MenuObjectState.finished;


        GUI.Box(new Rect(secondSeparator + 5, 5, Screen.width - secondSeparator -10, Screen.height - 70), "Statistics");

        Rect scrollInside2 = new Rect(0, 0, secondSeparator, 2000);

        scrollPos2 = GUI.BeginScrollView(new Rect(secondSeparator + 5, 5, secondSeparator - 20, Screen.height - 105), scrollPos2, scrollInside);

        int h = 5;

        foreach (var statPair in Stats)
        {
            GUI.Label(new Rect(5, h, secondSeparator - 10, statHeight), statPair.Value.Type + " " + statPair.Value.Value.ToString());

            h += statHeight;
        }

        GUI.EndScrollView();

        if (GUI.Button(new Rect(secondSeparator + 5, Screen.height - 65, Screen.width - secondSeparator - 10, 60), "OK"))
            {
                Dictionary<Type, ShipController> controllers = ShipControllerFactory.GenerateControllers(Stats);

                UnityEngine.Object.DontDestroyOnLoad(shipPos);

                Ship ship = shipPos.GetComponent<Ship>();
                InterSceneContainer.Add("ship", ship);

                ship.Initialize(controllers);
                ship.inputActive = true;
                ship.controllersActive = true;

                Application.LoadLevel("MapTest");
            }
            

        /*
        EngineController ec = new EngineController(0, 0, 0);
        MainComputerController mcc = new MainComputerController();
        ShipController sc = ec;
        
        if (GUI.Button(new Rect(500, 100, 100, 100), "Test"))
            Debug.Log(typeof(EngineController) + " " + typeof(MainComputerController) + " " + sc.GetType());
        */
         
        return MenuObjectState.active;
    }


    Vector3 lastPos = Vector3.zero;
    bool lastState;

    public void Update()
    {
        bool state=Input.GetMouseButton(1);

        if (state)
        {
            
            Vector3 pos = Input.mousePosition;
            if (lastState)
            {
                Vector3 delta = lastPos - pos;

                delta *= 0.5f;

                //camera.transform.Rotate(new Vector3(delta.y,-delta.x,0));
                camera.transform.Rotate(new Vector3(0, -delta.x, 0));
                //camera.transform.Rotate(new Vector3(delta.y, 0, 0));
            }
            lastPos = pos;
        }

        lastState = state;


        if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
        {
            if (camera.transform.localScale.x < 2)
                camera.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);

        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            if (camera.transform.localScale.x > 0.5)
                camera.transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);

        }
    }

    public void Start()
    {
        camera.SetActive(true);
    }

    public void End()
    {
        camera.SetActive(false);
    }

    public object GetReturnData()
    {
        throw new NotImplementedException();
    }

    public MenuObjectReturnType GetReturnType()
    {
        throw new NotImplementedException();
    }
}