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
        public List<GUIComponent> components;
        public List<Pair<string, string>> attribs;
        public List<string> queuedEvents;

        public string name;
        public GUIRectangle rect;

        public GUIObject(string filename)
        {
            components = new List<GUIComponent>();

            rect = new GUIRectangle(new Vector2(0.4f,0.45f), new Vector2(0.5f,0.6f));

            XDocument doc = ToolBox.TryLoadXml(filename);

            XElement objElem = doc.Elements().First(); //TODO: don't just take the first element in the document, actually find a GUIObject tag
            rect = new GUIRectangle(ToolBox.GetAttributeVector4(objElem, "rect", Vector4.Zero));

            foreach (XElement elem in objElem.Elements())
            {
                GUIComponent newComponent = null;
                switch (elem.Name.ToString())
                {
                    case "Sprite":
                        newComponent = new SpriteComponent(this,elem);
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
