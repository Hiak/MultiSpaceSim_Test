using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Assets.Scripts.Ship;

namespace Assets.Resources.ShipParts
{
    class ShipControllerFactory
    {

        public static Dictionary<Type, ShipController> GenerateControllers(Dictionary<String, Stat> stats)
        {
            Dictionary<Type, ShipController> ret=new Dictionary<Type,ShipController>();

            ArrayList optionalList=new ArrayList();

            optionalList.Add(GenerateController<ShieldController>(true, stats, new string[] { "ShieldPower" }));

            ret.Add(typeof(EngineController), GenerateController<EngineController>(false, stats, new string[] { "MaxSpeed", "EnginePower", "RotationSpeed", "Mass" }));
            ret.Add(typeof(MainComputerController), GenerateController<MainComputerController>(false, stats, new string[] { "Mass" }));            
            ret.Add(typeof(CameraController), new CameraController());
            ret.Add(typeof(FlightUIController), new FlightUIController());
            
            foreach (var optional in optionalList)
            {
                if (optional != null)
                    ret.Add(optional.GetType(), (ShipController)optional);
            }

            return ret;
        }





        static T GenerateController<T>(bool optional,Dictionary<String, Stat> stats, string[] statTypes) where T : ShipController, new()
        {
            List<float> parameters = new List<float>();

            foreach (var typeName in statTypes)
            {
                Stat tmp=null;
                if (stats.TryGetValue(typeName, out tmp))
                    parameters.Add(tmp.Value);
                else
                {
                    if (optional == false)
                        throw new InsufficientStatExceptionException(typeName);
                    else return null;
                }
            }

            T controller = new T();
            controller.FactoryInit(parameters.ToArray());

            return controller;
        }


    }

[Serializable]
public class InsufficientStatExceptionException : Exception
{
  public InsufficientStatExceptionException() { }
  public InsufficientStatExceptionException( string message ) : base( message ) { }
  public InsufficientStatExceptionException( string message, Exception inner ) : base( message, inner ) { }
  protected InsufficientStatExceptionException( 
	System.Runtime.Serialization.SerializationInfo info, 
	System.Runtime.Serialization.StreamingContext context ) : base( info, context ) { }
}
}
