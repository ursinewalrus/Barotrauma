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
        public GUI XGUI;
        
        public Dictionary<string,string> attribs;
        public List<string> queuedEvents;

        public string name;

        public bool isMouseOn = false;

        public GUIObject(GUI creator,XElement objElem)
        {
            owner = null;
            
            XGUI = creator;
            
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
                        if (!attribs.ContainsKey(attribute.Name.ToString())) attribs.Add(attribute.Name.ToString(), attribute.Value);
                        else attribs[attribute.Name.ToString()] = attribute.Value;
                        break;
                }
            }
            
            if (templateName == "") return;

            XElement templateElem = XGUI.templates[templateName];
            CreateChildrenComponents(templateElem);
        }
    }
}
