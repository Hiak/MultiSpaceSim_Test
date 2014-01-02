using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Menu.GUI_Addons
{
    public abstract class CustomWindow : MenuObject
    {

        public Rect position;
        public int id;

        public MenuObjectState DrawGUI()
        {
            GUI.Window(id, position, InnerDraw, "");

            return state;
        }

        protected MenuObjectState state=MenuObjectState.active;

        public abstract void InnerDraw(int windowID);

        public abstract object GetReturnData();

        public abstract MenuObjectReturnType GetReturnType();

        public abstract void Update();

        public abstract void Start();

        public abstract void End();
    }
}
