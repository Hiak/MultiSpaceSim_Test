using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Menu
{
    class ServerCreator : MenuObject
    {



        public object GetReturnData()
        {
            throw new NotImplementedException();
        }

        public MenuObjectReturnType GetReturnType()
        {
            throw new NotImplementedException();
        }



        static Rect bottomButtonsRect;

        public string ServerName = "";


        public MenuObjectState DrawGUI()
        {
            //GUI.Box(new Rect(5, 5, Screen.width - 10, Screen.height - 10), "");



            //ServerNameField
            GUI.BeginGroup(new Rect(10, 10, 300, 40));
                GUI.Box(new Rect(0,0,300,40),"ServerName");
                ServerName = GUI.TextField(new Rect(0, 20, 300, 20), ServerName); ;
            GUI.EndGroup();

            GUI.BeginGroup(bottomButtonsRect);
            {
                if (GUI.Button(new Rect(10, 10, 300, 100), "Cancel"))
                    return MenuObjectState.finished;
                if (GUI.Button(new Rect(320, 10, 300, 100), "Ok"))
                    StartServer();

            }
            GUI.EndGroup();

            return MenuObjectState.active;
        }

        public void StartServer()
        {
            if (ServerName == "") return;

            NetworkManager.StartServer(ServerName);


            //HostData hd = new HostData();
            //hd.gameName = ServerName;

            //NetworkManager.LoadLevelAsServer("MapTest", hd);

        }


        public void Update()
        {
            
        }

        public void Start()
        {
            bottomButtonsRect = new Rect(0, Screen.height * 0.8f, Screen.width, Screen.height * 0.2f);
            //Debug.Log(bottomButtonsRect + " " + Screen.width + " " + Screen.height);
        }

        public void End()
        {
            
        }
    }
}
