using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Menu
{
    public enum MenuObjectState
    {

        finished,
        active

    }

    public enum MenuObjectReturnType
    {

        ok,
        cancel

    }

    public interface MenuObject
    {
        Object GetReturnData();

        MenuObjectReturnType GetReturnType();

        MenuObjectState DrawGUI();

        void Update();

        void Start();

        void End();

    }
}
