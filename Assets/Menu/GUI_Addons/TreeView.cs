using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Assets.Menu.GUI_Addons
{

    public delegate void childrenOutOfDateHandler();

    public interface TreeViewNodeVisual {

        Vector2 Size { get; set; }

        event childrenOutOfDateHandler childrenOutOfDate;

        bool Draw(Vector2 place, bool open);
        ArrayList ReloadChildren();
        
    }


    internal class TreeViewNode {

        public TreeViewNode(TreeViewNodeVisual visual)
        {
            this.visual = visual;
            visual.childrenOutOfDate += UpdateChildren;
            children = new List<TreeViewNode>();
            UpdateChildren();
        }

        TreeView parent;
        internal List<TreeViewNode> children;

        internal bool open;

        internal TreeViewNodeVisual visual;


        void UpdateChildren() {

            children.Clear();

            var tmp=visual.ReloadChildren();

            if (tmp == null) return;

            foreach (TreeViewNodeVisual vis in tmp)
            {
                children.Add(new TreeViewNode(vis));
            }
        }

    }




    public class TreeView
    {
        public TreeView(Vector2 place, TreeViewNodeVisual visual, int space, int linespace)
        { 
            root = new TreeViewNode(visual); 
            position = place;
            this.space = space;
            this.linespace = linespace;
        }

        public Vector2 size { get; private set; }

        Vector2 position;

        TreeViewNode root;
        int space;
        int linespace;
        
        public void Draw() {

            Vector2 pos = position;
            Vector2 maxSize=Vector2.zero;

            RecursiveDraw(root, ref pos, ref maxSize, 0);

            maxSize.y -= linespace;

            size = maxSize;
        }

        void RecursiveDraw(TreeViewNode node, ref Vector2 pos, ref Vector2 maxSize, int depth) {

            bool isOpen=node.visual.Draw(pos+new Vector2(depth*space,0), node.open);

            pos += new Vector2(0, linespace + node.visual.Size.y);

            maxSize.y += node.visual.Size.y + linespace;
            float t=depth * space + node.visual.Size.x;

            if (t > maxSize.x)
                maxSize.x = t;

            if (isOpen)
                foreach (var child in node.children)
                {
                    RecursiveDraw(child, ref pos, ref maxSize , depth + 1);
                }

            node.open = isOpen;
        }

        //public void Resize() { size = root.GetExplicitSize(); }
    }






    public class TestTreeVisual : TreeViewNodeVisual
    {

        public TestTreeVisual(string num) { 
            this.num = num; _size = new Vector2(260,50);
            handle.onContent = new GUIContent("-");
            handle.offContent = new GUIContent("+");
        }

        string num;
        ToggleButton handle=new ToggleButton();

        public Vector2 Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
            }
        }

        Vector2 _size;
        bool isOpen=false;

        public event childrenOutOfDateHandler childrenOutOfDate;

        public bool Draw(Vector2 place, bool open)
        {
            isOpen = handle.Draw(new Rect(place.x, place.y, 50, 50));

            if (GUI.Button(new Rect(place.x + 10 + 50, place.y, 200, 50), num))
                childrenOutOfDate();

            return isOpen;
        }

        static System.Random rand = new System.Random();

        public ArrayList ReloadChildren()
        {
            
            if (num.Length > 2) return null;

            ArrayList retList=new ArrayList();

            for (int i = 0; i < rand.Next()%3; i++)
            {
                retList.Add(new TestTreeVisual(num + i.ToString()));
            }

            return retList;
        }
    }

}
