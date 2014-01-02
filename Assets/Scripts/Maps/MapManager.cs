using UnityEngine;
using System.Collections;
using Assets.Scripts;
using Assets.Scripts.Ship;
using System;

public class MapManager : MonoBehaviour {


    GameObject playerObj;

    public GameObject mapObjects;

	// Use this for initialization
	void Start () {

        Ship ship;

        try
        {
            ship = InterSceneContainer.Get("ship") as Ship;

            ship.shipObject.transform.position = new Vector3(0, 0, 0);

        }
        catch
        {
            GameObject shipObj=null;

            if (!Network.isClient && !Network.isServer)
                shipObj = (GameObject)Instantiate(Resources.Load("ShipParts/Temp/Ship"));
            else
            shipObj = (GameObject)Network.Instantiate(Resources.Load("ShipParts/Temp/Ship"), Vector3.zero, Quaternion.identity, 0);

            ship=shipObj.GetComponent<Ship>();

            System.Collections.Generic.Dictionary<Type, ShipController> controllers = new System.Collections.Generic.Dictionary<Type, ShipController>();

            EngineController ec = new EngineController();
            ec.FactoryInit(new float[]{333.333f,200000,600});
            controllers.Add(typeof(EngineController),ec);

            MainComputerController mcc = new MainComputerController();
            mcc.FactoryInit(new float[] { 100 });
            controllers.Add(typeof(MainComputerController), mcc);

            CameraController cc = new CameraController();
            controllers.Add(typeof(CameraController), cc);

            FlightUIController fuc = new FlightUIController();
            controllers.Add(typeof(FlightUIController), fuc);


            ship.Initialize(controllers);

            ship.inputActive = true;
            ship.controllersActive = true;
        }

        //UnityEngine.Object.DontDestroyOnLoad(ship.shipObject);

        playerObj = ship.shipObject;

        ship.SwitchCamera();

        Screen.showCursor = false;


	}
	
	// Update is called once per frame
	void Update () {
        UpdateOffset();
	}

    public static Vector3 offset = Vector3.zero;

    void UpdateOffset()
    {
        if (playerObj.transform.position.magnitude > 1000) {
            offset += playerObj.transform.position;
            mapObjects.transform.position -= playerObj.transform.position;
            playerObj.transform.position = Vector3.zero;
        }
    }
}
