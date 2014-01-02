using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Ship
{
    public abstract class ShipController
    {
        protected Ship ship;

        public abstract void FactoryInit(float[] parameters);

        public void ShipInit(Ship _ship) {
            GeneralInitialisation(_ship);
            SpecialInitialisation();
        }

        void GeneralInitialisation(Ship _ship) { ship = _ship; }
        protected abstract void SpecialInitialisation();

    }




    public class EngineController : ShipController 
    {
        public float maxSpeed { get; private set; }
        float rotationSpeed;
        float enginePower;

        // [float maxSpeed, float enginePower, float rotationSpeed]
        public override void FactoryInit(float[] parameters) {

            maxSpeed = parameters[0];
            enginePower = parameters[1];
            rotationSpeed = parameters[2];

            //acceleration = enginePower / mass;
        }

        protected override void SpecialInitialisation() {
            //ship.acceleration += Acceleration;
            //ship.rotation += Rotation;
        }

        public void Sliding(Vector2 v)
        {

            //ship.rigidbody.AddForce

        }

        public void Acceleration(float a)
        {
            if ((a > 0 && ship.rigidbody.velocity.magnitude < maxSpeed) || (a < 0 && ship.rigidbody.velocity.magnitude > 0 ))
                ship.rigidbody.AddForce(ship.transform.forward * a * enginePower * Time.deltaTime);
        }
        public void Rotation(Vector3 v)
        {
            float maxRotation = rotationSpeed * Time.deltaTime;

            v = v * maxRotation;

            //ship.transform.Rotate(v.y, v.x, v.z);
            //ship.rigidbody.MoveRotation(ship.rigidbody.rotation * Quaternion.Euler(new Vector3(v.y,v.x,v.z)));
            ship.rigidbody.AddRelativeTorque(new Vector3(v.y, 20*v.x, 15*v.z));


            float speed = ship.rigidbody.velocity.magnitude;
            ship.rigidbody.velocity = speed * ship.transform.forward;

        }
    }




    public class HealthController : ShipController
    {
            // [float maxHealth]
        public override void FactoryInit(float[] parameters) { }

        protected override void SpecialInitialisation() { }
    }




    public class ShieldController : ShipController
    {
            // [float maxShield, float shieldRegenerationMultiplier]
        public override void FactoryInit(float[] parameters) { }

        protected override void SpecialInitialisation() { }

    }




    public class MainComputerController : ShipController
    {
        public bool mouseControl { get; private set; }
        float mass = 0;
        
        EngineController engines;

        public static readonly float activeInputCircle = 0.7f;

            // [mass]
        public override void FactoryInit(float[] parameters) {

            mass = parameters[0];
        }



        protected override void SpecialInitialisation() {

            mouseControl = true;

            engines = ship.GetController<EngineController>();

            ship.rigidbody.mass = mass;

            ship.acceleration += AccelerationControl;
            ship.rotation += RotationControl;
            ship.switchControlMode += switchControlMode;
        }



        void switchControlMode()
        {
            mouseControl = !mouseControl;
            //mert itt fontos a hívás sorrendje
            ship.UIController.switchControlMode();
            
        }

        private void AccelerationControl(float val)
        {
            engines.Acceleration(val);
        }

        private void RotationControl(Vector3 val)
        {
            if (mouseControl)
            {
                if (val.magnitude > MainComputerController.activeInputCircle)
                    val *= MainComputerController.activeInputCircle / val.magnitude;

                engines.Rotation(val);

            }
        }


    }


    public class CameraController : ShipController
    {

        //Camera fpsCamera;
        TPSCameraInterface tpsCamera;

            // []
        public override void FactoryInit(float[] parameters) { }

        protected override void SpecialInitialisation() {

            tpsCamera=ship.GetComponentInChildren<TPSCameraInterface>();

            ship.switchCamera += SwitchCamera;
            ship.zoomCamera += ZoomCamera;

        }

        void SwitchCamera() { tpsCamera.isActiveCamera = true; }
        void ZoomCamera(float f) { tpsCamera.Zoom += f * 0.1f; }
        
    }




    public class FlightUIController : ShipController
    {
        MainComputerController mainComputer;
        EngineController engines;

        GUIStyle controlCircleStyle=new GUIStyle();
        GUIStyle speedometerBGStyle = new GUIStyle();
        //GUIStyle speedometerFrameStyle = new GUIStyle();
        GUIStyle speedometerTextStyle = new GUIStyle();

        
        // []
        public override void FactoryInit(float[] parameters) { }

        protected override void SpecialInitialisation() {

            mainComputer = ship.GetController<MainComputerController>();
            engines = ship.GetController<EngineController>();


            controlCircleStyle.normal.background =(Texture2D)UnityEngine.Resources.Load("Textures/in game gui/control_circle_1000x1000");
            speedometerBGStyle.normal.background = (Texture2D)UnityEngine.Resources.Load("Textures/in game gui/SpeedometerBG2");
            //speedometerFrameStyle.normal.background = (Texture2D)UnityEngine.Resources.Load("Textures/in game gui/SpeedometerFrame");

            speedometerTextStyle.alignment = TextAnchor.MiddleCenter;
            speedometerTextStyle.normal.textColor = Color.white;
        }

        public void switchControlMode()
        {
            if (mainComputer.mouseControl)
            {
                Screen.showCursor = false;
            }
            else
            {
                Screen.showCursor = true;
            }
        }


        public void OnGUI() {

            if (mainComputer.mouseControl)
            {
                float t = MainComputerController.activeInputCircle;
                float w = Screen.width;
                float h = Screen.height;
                GUI.Box(new Rect(w / 2 - h * t / 2, h / 2 - h * t / 2, h * t, h * t), "", controlCircleStyle);

                Vector3 mouse = Input.mousePosition - new Vector3(w / 2, h / 2);

                //cursor
                if (mouse.magnitude > MainComputerController.activeInputCircle * h / 2)
                    mouse *= MainComputerController.activeInputCircle * h / 2 / mouse.magnitude;

                GUI.Box(new Rect(w / 2 + mouse.x - h / 40, h / 2 - mouse.y - h / 40, h / 20, h / 20), "", controlCircleStyle);
                GUI.Box(new Rect(Input.mousePosition.x - h / 80, Screen.height - Input.mousePosition.y - h / 80, h / 40, h / 40), "", controlCircleStyle);

            }


            Vector3 d = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);

            string data = "position: " + ship.transform.position.ToString() +
                "\n corrected: " + (ship.transform.position + MapManager.offset) +
                "\n speed: " + ship.rigidbody.velocity.magnitude.ToString() +
                "\n fps: " + 1.0f / Time.deltaTime +
                "\n offset: " + MapManager.offset +
                "\n mouse: " + d;

            GUI.Box(new Rect(0, 0, 200, 120), data);

            //speedometer

            float speed = ship.rigidbody.velocity.magnitude;
            float speedProp = Math.Max(speed / engines.maxSpeed, 0.03f);

            GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height * 0.9f, 200, 20),"");
            GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height * 0.9f, speedProp * 200, 20), "", speedometerBGStyle);
            GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height * 0.9f, 200, 20), ((int)speed).ToString() + " m/s", speedometerTextStyle);


            //

            foreach (Ship otherShip in PlayerManager.ships)
            {
                if (otherShip.shipObject.networkView.isMine)
                    break;

                Vector3 posVector = Camera.main.WorldToScreenPoint(otherShip.shipObject.transform.position);

                Vector2 modVector=new Vector2(
                    Math.Min(Math.Max(posVector.x,0),Screen.width),
                    Math.Min(Math.Max(posVector.y,0),Screen.height)
                    );


                GUI.Box(new Rect(modVector.x, Screen.height - modVector.y, 1, 1),
                    otherShip.shipObject.networkView.viewID.ToString() + posVector + " " + modVector, speedometerTextStyle);

                //Debug.Log(otherShip.shipObject.networkView.viewID + ": " + posVector);
            }


        }

    }

}
