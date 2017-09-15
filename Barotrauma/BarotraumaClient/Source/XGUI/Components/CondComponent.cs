using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Barotrauma.XGUI
{
    public class CondComponent : GUIComponent
    {
        public Dictionary<string, string> conditions;

        public CondComponent(GUIEntity creator, XElement elem) : base(creator,elem)
        {
            rect = new GUIRectangle(0, 0, 1, 1);

            conditions = new Dictionary<string, string>();
            foreach(XAttribute attribute in elem.Attributes())
            {
                conditions.Add(attribute.Name.ToString(), attribute.Value);
            }
        }

        public bool ConditionsMet()
        {
            GUIObject parentObject = GetParentObject();
            foreach (string key in conditions.Keys)
            {
                switch (key)
                {
                    case "Hover":
                        if (conditions[key] == "true")
                        {
                            if (!parentObject.isMouseOn) return false;
                        }
                        else if (conditions[key] == "false")
                        {
                            if (parentObject.isMouseOn) return false;
                        }
                        break;
                    case "Clicked":
                        if (conditions[key] == "true")
                        {
                            if (!PlayerInput.LeftButtonClicked()) return false;
                        }
                        else if (conditions[key] == "false")
                        {
                            if (PlayerInput.LeftButtonClicked()) return false;
                        }
                        break;
                }
            }
            return true;
        }

        public override void Update(float deltaTime)
        {
            if (!ConditionsMet()) return;

            base.Update(deltaTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!ConditionsMet()) return;

            base.Draw(spriteBatch);
        }
    }
}
