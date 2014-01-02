using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Assets.Resources.ShipParts;


public class JointPosition {

    //a PositionObject-et itt még nem állítjuk be, mivel a betöltés sorrendje nem garantált
    private JointPosition() { }
    public JointPosition(string _PositionName, int _ID) {ID = _ID;PositionName = _PositionName;}

    public string PositionName { get; protected set; }
    public int ID { get; protected set; }
    public Transform PositionObject { get; set; }

    public JointPosition Copy() {

        JointPosition ret = new JointPosition();

        ret.PositionName = PositionName;
        ret.ID = ID;
        ret.PositionObject = null;

        return ret;
    }

}

public class Joint
{
    
    public string DisplayName{ get; protected set; }
    public string Type { get; protected set; }
    public ShipPart subPart = null;

    public Dictionary<int, JointPosition> Positions = new Dictionary<int, JointPosition>();

    private Joint() { }

    public Joint(XmlNode part)
    {
        DisplayName = part.Attributes["DisplayName"].Value;
        Type = part.Attributes["Type"].Value;

        foreach (XmlNode item in part)
        {
            if (item.Name == "JointPosition")
            {
                int ID = int.Parse(item.Attributes["ID"].Value);
                string PositionName = item.Attributes["PositionName"].Value;
                Positions.Add(ID, new JointPosition(PositionName,ID));

            }
        }
    }


    public Joint Copy() {

        Joint ret = new Joint();

        ret.Positions = new Dictionary<int, JointPosition>();
        foreach (var pos in Positions)
        {
            ret.Positions.Add(pos.Key, pos.Value.Copy());
        }

        ret.DisplayName = DisplayName;
        ret.Type = Type;
        ret.subPart = null;

        return ret;
    }

}


public class ShipPart{

    public string Name {get; protected set; }
    public List<Joint> Joints=new List<Joint>();
    public Dictionary<int, GameObject> Res = new Dictionary<int, GameObject>();
    public List<Stat> Stats=new List<Stat>();
    public List<StatModifier> Modifiers = new List<StatModifier>();


    private ShipPart() { }

    public ShipPart(XmlNode part)
    {
        Name = part.Attributes["Name"].Value;
        
        foreach (XmlNode item in part)
        {
            switch (item.Name)
            {
                case "Joint":
                    Joints.Add(new Joint(item));
                    break;
                case "Resource":
                    string src = item.Attributes["src"].Value;
                    int ID = int.Parse(item.Attributes["ID"].Value);

                    GameObject constructedObj = (GameObject)Resources.Load("ShipParts/" + src);
                    constructedObj.SetActive(false);

                    Res.Add(ID, constructedObj);
                    
                    break;
                case "Stat":

                    string AttributeType = item.Attributes["AttributeType"].Value;
                    string StatType = item.Attributes["StatType"].Value;
                    float Value = float.Parse(item.Attributes["Value"].Value);

                    if (StatType == "value")
                    {
                        Stats.Add(new Stat(AttributeType, Value));
                    }
                    else if (StatType == "multiplier")
                    {
                        int depth=-1;
                        var t = item.Attributes["Depth"];
                        if (t != null) depth = int.Parse(t.Value);

                        Modifiers.Add(new StatModifier(AttributeType, Value, depth));
                    }

                    break;
                default:
                    break;
            }
         
        }

        FinishJoints();

    }

    public void FinishJoints()
    {

        foreach (Joint joint in Joints)
        {
            foreach (var jointPosition in joint.Positions)
            {
                string name = jointPosition.Value.PositionName;

                Transform posObject=null;

                foreach (var res in Res)
                {
                    var tmp= res.Value.transform.FindChild(name);
                    if (tmp != null)
                        posObject = tmp;
                }

                jointPosition.Value.PositionObject = posObject;
            }
        }

    }

    public ShipPart Instantiate() {

        ShipPart ret = new ShipPart();

        ret.Joints = new List<Joint>();
        foreach (var joint in Joints)
        {
            ret.Joints.Add(joint.Copy());
        }

        ret.Modifiers = new List<StatModifier>();
        foreach (var modifier in Modifiers)
        {
            ret.Modifiers.Add(modifier.Copy());
        }

        ret.Name = Name;

        ret.Res = new Dictionary<int, GameObject>();
        foreach (var res in Res)
        {
            ret.Res.Add(res.Key, (GameObject)MonoBehaviour.Instantiate(res.Value));
        }

        ret.Stats = new List<Stat>();

        foreach (var stat in Stats)
        {
            ret.Stats.Add(stat.Copy());
        }

        ret.FinishJoints();

        return ret;
    }


    public void CollectStats(Dictionary<string, Stat> collectedStats, Dictionary<string, List<StatModifier>> collectedModifiers)
    {

        foreach (var stat in Stats)
        {
            Stat statCopy = stat.Copy();

            List<StatModifier> selectedModifiers = null;


            if (collectedModifiers.TryGetValue(stat.Type, out selectedModifiers))
            {
                foreach (var mod in selectedModifiers)
                {
                    statCopy.Multiply(mod);
                }

                selectedModifiers.AddRange(Modifiers);

            }
            else
            {
                collectedModifiers.Add(stat.Type, new List<StatModifier>(Modifiers));
            }


            Stat selectedStat = null;



            if (collectedStats.TryGetValue(stat.Type, out selectedStat))
                selectedStat.Add(statCopy);
            else
                collectedStats.Add(stat.Type,statCopy);

        }


        foreach (var joint in Joints)
        {
            if (joint.subPart != null)
                joint.subPart.CollectStats(collectedStats, collectedModifiers);
        }

    }


    public void RemoveRes()
    {

        foreach (var res in Res)
        {
            Object.Destroy(res.Value);
            foreach (var joint in Joints)
            {
                if (joint.subPart != null)
                    joint.subPart.RemoveRes();
            }
        }

    }

}
