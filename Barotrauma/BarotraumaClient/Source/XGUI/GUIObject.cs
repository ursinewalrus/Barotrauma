using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using System.Xml.Linq;

namespace Barotrauma.XGUI
{
    public class GUIObject : GUIEntity
    {
        public Dictionary<string,string> attribs;
        public List<string> queuedEvents;

        public string name;

        public bool isMouseOn = false;

        public GUIObject(GUIEntity creator,XElement objElem) : base(creator, objElem)
        {
            attribs = new Dictionary<string, string>();

            rect = new GUIRectangle(0,0,0,0);
            
            string templateName = "";

            foreach (XAttribute attribute in objElem.Attributes())
            {
                switch (attribute.Name.ToString())
                {
                    case "template":
                        templateName = attribute.Value;
                        break;
                    case "rect":
                        rect = new GUIRectangle(ToolBox.ParseToVector4(attribute.Value));
                        break;
                    default:
                        //DebugConsole.NewMessage(attribute.Name.ToString(),Color.White);
                        if (!attribs.ContainsKey(attribute.Name.ToString())) attribs.Add(attribute.Name.ToString(), attribute.Value);
                        else attribs[attribute.Name.ToString()] = attribute.Value;
                        break;
                }
            }
            
            if (templateName == "") return;
            name = templateName;

            XElement templateElem = GetXGUI().templates[templateName];

            List<GUIEntity> prevChildren = children;
            children = CreateChildrenEntities(this,templateElem);
            if (prevChildren != null)
            {
                children.AddRange(prevChildren);
            }
        }

        public void ResetMouseOn()
        {
            isMouseOn = false;
            foreach (GUIObject obj in children.OfType<GUIObject>())
            {
                obj.ResetMouseOn();
            }
        }

        public bool UpdateMouseOn()
        {
            for (int i=children.Count-1;i>=0;i--)
            {
                if (children[i] is GUIObject)
                {
                    GUIObject obj = children[i] as GUIObject;
                    if (obj.UpdateMouseOn()) return true;
                }
            }
            Rectangle xnaRect = GUIRectangle.ScaleToXNARect(GetScaledRect(), new Rectangle(0, 0, GameMain.GraphicsWidth, GameMain.GraphicsHeight));
            if (xnaRect.Contains(PlayerInput.MousePosition))
            {
                isMouseOn = true;
                return true;
            }

            return false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
