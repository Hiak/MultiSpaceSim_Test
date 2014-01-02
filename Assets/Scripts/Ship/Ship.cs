using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Ship
{
    public class Ship : MonoBehaviour
    {

        public bool inputActive=false;
        public bool controllersActive = false;

        public event Action<float> acceleration;
        public event Action<Vector3> rotation;
        public event Action switchCamera;
        public event Action<float> zoomCamera;
        public event Action switchControlMode;

        //Temp GUI elements
        //public GUIStyle controlCircleStyle;

        Dictionary<Type, ShipController> controllers;

        public GameObject shipObject { get; private set; }

        public FlightUIController UIController;



        public void SwitchCamera() { switchCamera(); }

        public void Initialize(Dictionary<Type, ShipController> _controllers)
        {

            GameObject tpsCamera = (GameObject)Instantiate(UnityEngine.Resources.Load("ShipParts/Cameras/TPSCamera"));

            tpsCamera.transform.parent = shipObject.transform;

            SetControllers(_controllers);

            UIController = GetController<FlightUIController>();
        
        }

        public void SetControllers(Dictionary<Type, ShipController> _controllers) { 
            controllers = _controllers;
            foreach (var controller in controllers)
            {
                controller.Value.ShipInit(this);
            }
        }

        public T GetController<T>() where T : ShipController
        {
            try
            {
                return controllers[typeof(T)] as T;
            }
            catch{return null;}
        }

        public void Awake() {
    
            shipObject = gameObject; 
        
        }


        public void Update() {

            if (networkView.isMine || NetworkManager.connectionState==NetworkManager.ConnectionState.offline)
            {
                InputUpdate();
            }
        }

        void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
        {
            Vector3 syncPosition = Vector3.zero;
            Vector3 syncVelocity = Vector3.zero;
            Quaternion syncRotation = Quaternion.identity;

            if (stream.isWriting)
            {
                syncPosition = transform.localPosition + MapManager.offset;
                syncVelocity = rigidbody.velocity;
                syncRotation = rigidbody.rotation;

                stream.Serialize(ref syncPosition);
                stream.Serialize(ref syncVelocity);
                stream.Serialize(ref syncRotation);

            }
            else
            {
                stream.Serialize(ref syncPosition);
                stream.Serialize(ref syncVelocity);
                stream.Serialize(ref syncRotation);

                transform.localPosition = syncPosition - MapManager.offset;
                rigidbody.velocity = syncVelocity;
                rigidbody.rotation = syncRotation;

            }
        }


        void OnNetworkInstantiate(NetworkMessageInfo nmi)
        {

            //Debug.Log("OnNetworkInstantiate");

            UnityEngine.Object.DontDestroyOnLoad(this);


            if (!networkView.isMine)
            {
                /*PlayerManager pm = FindObjectOfType<PlayerManager>();
                if (pm!=null)
                {
                    pm.ships.Add(this);
                    transform.parent = pm.transform;
                }*/

                PlayerManager.ships.Add(this);
            }
        }


        /*
        void Start()
        {


        }*/

        void OnGUI()
        {
            if (controllersActive == false)
                return;

            /*
            Vector3 d = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);

            string data = "position: " + transform.position.ToString() +
                "\n speed: " + rigidbody.velocity.magnitude.ToString() +
                "\n fps: " + 1.0f / Time.deltaTime +
                "\n offset: " + MapManager.offset +
                "\n mouse: " + d +
                "\n corrected: " + (transform.position + MapManager.offset);

            GUI.Box(new Rect(0, 0, 200, 120), data);
            */

            UIController.OnGUI();

        }



        public void InputUpdate()
        {
            if (controllersActive == false || inputActive == false)
                return;

            if (rotation != null)
            {
                Vector3 mouseD = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                mouseD.y *= -1;
                mouseD /= Screen.height / 2;
                mouseD.z = Input.GetAxis("Z-rotation");
                rotation(mouseD);
            }

            if (acceleration != null)
                acceleration(Input.GetAxis("Acceleration"));
            if (Input.GetButtonDown("SwitchCamera") && switchCamera != null)
            {
                switchCamera();
            }
            if (zoomCamera != null)
            {
                zoomCamera(-1*Input.GetAxis("ZoomCamera"));
            }

            if (Input.GetButtonDown("Switch Control Mode") && switchControlMode != null)
            {
                switchControlMode();
            }

        }
        
    }
}
