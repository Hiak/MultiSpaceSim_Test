using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Menu.GUI_Addons
{
    class ToggleButton
    {
        public GUIStyle onStyle = null;
        public GUIStyle offStyle = null;
        public GUIContent onContent;
        public GUIContent offContent;
        public bool value;

        //le van e épp nyomva
        private bool state=false;
        private static bool globalState=false;

        public bool Draw(Rect pos){

            bool tmp=false;

            if (value)
                if (onStyle != null) tmp = GUI.Button(pos, onContent, onStyle);
                else tmp = GUI.Button(pos, onContent);
            else
                if (offStyle != null) GUI.Button(pos, offContent, offStyle);
                else tmp = GUI.Button(pos, offContent);

            if (!tmp && state)
                value = !value;

            state = tmp;

            return value;
        }

        public static bool Draw(Rect pos, bool value, GUIContent onContent, GUIContent offContent, GUIStyle onStyle, GUIStyle offStyle)
        {

            bool tmp = false;


            if (value)
                if (onStyle != null) tmp = GUI.Button(pos, onContent, onStyle);
                else tmp = GUI.Button(pos, onContent);
            else
                if (offStyle != null) tmp = GUI.Button(pos, offContent, offStyle);
                else tmp = GUI.Button(pos, offContent);


            if (!tmp && globalState)
                value = !value;

            globalState = tmp;

            return value;
        }


    }
}
