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
    public class GUIObject
    {
        public GUI owner;

        public List<GUIComponent> components;
        public Dictionary<string,string> attribs;
        public List<string> queuedEvents;

        public string name;
        public GUIRectangle rect;

        public GUIObject(GUI creator,string filename)
        {
            owner = creator;

            components = new List<GUIComponent>();
            attribs = new Dictionary<string, string>();

            rect = new GUIRectangle(0,0,0,0);

            XDocument doc = ToolBox.TryLoadXml(filename);

            XElement objElem = doc.Elements().First(); //TODO: don't just take the first element in the document, actually find a GUIObject tag

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

            XElement templateElem = owner.templates[templateName];
            foreach (XElement elem in templateElem.Elements())
            {
                GUIComponent newComponent = null;
                switch (elem.Name.ToString())
                {
                    case "Sprite":
                        newComponent = new SpriteComponent(this,elem);
                        break;
                    case "Text":
                        newComponent = new TextComponent(this, elem);
                        break;
                }
                if (newComponent != null) components.Add(newComponent);
            }
        }

        public void Update(float deltaTime)
        {
            foreach (GUIComponent component in components)
            {
                component.Update(deltaTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (GUIComponent component in components)
            {
                component.Draw(spriteBatch);
            }
        }
    }
}
