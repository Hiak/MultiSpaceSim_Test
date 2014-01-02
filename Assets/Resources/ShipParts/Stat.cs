using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Resources.ShipParts
{
    public class Stat
    {
        public Stat(string type, float value) { Type = type; Value = value; }

        private Stat() { }

        public string Type { get; private set; }

        public float Value { get; private set; }


        public static  Stat operator +(Stat s1, Stat s2) { 
            if(s1.Type!=s2.Type)throw new ArgumentException("s1 and s2 have different types");
            Stat r = new Stat(s1.Type,s1.Value+s2.Value);
            return r;
        }

        public static Stat operator *(Stat s1, StatModifier s2)
        {
            if (s1.Type != s2.Type) throw new ArgumentException("s1 and s2 have different types");
            Stat r = new Stat(s1.Type, s1.Value * s2.Multiplier);
            return r;
        }

        public void Add(Stat s1)
        {
            if (s1.Type != Type) throw new ArgumentException("s1 and s2 have different types");

            Value += s1.Value;

        }

        public void Multiply(StatModifier m1)
        {
            if (m1.Type != Type) throw new ArgumentException("m1 and s2 have different types");

            Value *= m1.Multiplier;

        }


        public Stat Copy() {

            Stat ret = new Stat();

            ret.Type = Type;
            ret.Value = Value;

            return ret;
        }
    }


    public class StatModifier {

        public StatModifier(string type, float multiplier) { Type = type; Multiplier = multiplier; Valid = true;}

        public StatModifier(string type, float multiplier, int maxDepth) { 
            Type = type; Multiplier = multiplier; Valid = true; Depth = maxDepth;
            CheckDepth();
        }

        private StatModifier() { }

        public string Type { get; private set; }

        public float Multiplier { get; private set; }

        public int Depth = -1;

        public bool Valid { get; private set; }

        public bool DecraseDepth() { 
            if(Valid)
                Depth--;
            return CheckDepth();
        }

        bool CheckDepth() { 
            if (Depth == 0) 
                Valid = false; 
            return Valid; 
        }

        public StatModifier Copy()
        {

            StatModifier ret = new StatModifier();

            ret.Type = Type;
            ret.Multiplier = Multiplier;
            ret.Depth = Depth;
            ret.Valid = Valid;

            return ret;
        }
    
    }
}
