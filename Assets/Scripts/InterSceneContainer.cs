using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class InterSceneContainer
    {

        static Dictionary<string, Object> container=new Dictionary<string,object>();

        public static void Add(string label, Object obj){

            container.Add(label, obj);
        }

        public static Object Get(string label)
        {
            return container[label];
        }



    }
}
