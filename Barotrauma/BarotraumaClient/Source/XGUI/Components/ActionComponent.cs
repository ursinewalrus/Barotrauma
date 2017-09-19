using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Barotrauma.XGUI
{
    public class ActionComponent : GUIComponent
    {
        private List<Pair<string, string>> actionList;

        //TODO: remove delegates when their owners don't exist anymore
        public delegate void ActionDelegate(string parameters);
        public static Dictionary<string,ActionDelegate> registeredActions = new Dictionary<string,ActionDelegate>();

        public ActionComponent(GUIEntity creator, XElement elem) : base(creator,elem)
        {
            actionList = new List<Pair<string, string>>();
            
            GUIObject parentObject = GetParentObject();

            string actionStr = ToolBox.GetAttributeString(elem, "action", "");
            if (actionStr[0] == '$')
            {
                if (parentObject.attribs.ContainsKey(actionStr.Substring(1))) actionStr = parentObject.attribs[actionStr.Substring(1)];
            }

            string[] actions = actionStr.Split(';');

            foreach (string a in actions)
            {
                string action = a;
                string parameters = "";
                
                if (action.Contains(":"))
                {
                    int indexOfColon = action.IndexOf(':');
                    parameters = action.Substring(indexOfColon + 1);
                    action = action.Substring(0, indexOfColon);
                }

                actionList.Add(Pair<string, string>.Create(action, parameters));
            }
        }
        
        public override void Update(float deltaTime)
        {
            foreach (Pair<string, string> pair in actionList)
            {
                DebugConsole.NewMessage(pair.First);
                string action = pair.First; string parameters = pair.Second;
                if (registeredActions.ContainsKey(action)) registeredActions[action](parameters);
            }

            base.Update(deltaTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
