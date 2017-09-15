using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Barotrauma.XGUI
{
    public class ActionComponent : GUIComponent
    {
        string action;
        string parameters;

        public ActionComponent(GUIEntity creator, XElement elem) : base(creator,elem)
        {
            action = ToolBox.GetAttributeString(elem, "action", "");
            parameters = "";

            GUIObject parentObject = GetParentObject();

            if (action[0]=='$')
            {
                if (parentObject.attribs.ContainsKey(action.Substring(1))) action = parentObject.attribs[action.Substring(1)];
            }

            if (action.Contains(":"))
            {
                int indexOfColon = action.IndexOf(':');
                parameters = action.Substring(indexOfColon+1);
                action = action.Substring(0, indexOfColon);
            }
        }
        
        public override void Update(float deltaTime)
        {
            switch (action)
            {
                case "ChangeMenu":
                    GetXGUI().ChangeMenu(parameters);
                    break;
            }

            base.Update(deltaTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
