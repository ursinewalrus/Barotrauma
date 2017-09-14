using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;

namespace Barotrauma.XGUI
{
    public class GUIEntity
    {
        public GUIRectangle rect;
        public GUIEntity owner;
        public List<GUIEntity> children;

        public virtual void Update(float deltaTime)
        {
            children.ForEach(c => c.Update(deltaTime));
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            children.ForEach(c => c.Draw(spriteBatch));
        }

        public GUIObject GetParentObject()
        {
            if (this is GUIObject) return (this as GUIObject);

            GUIEntity parentEntity = owner;
            while (!(parentEntity is GUIObject))
            {
                parentEntity = parentEntity.owner;
            }
            GUIObject parentObject = parentEntity as GUIObject;

            return parentObject;
        }

        public GUIRectangle GetScaledRect()
        {
            if (owner == null) return rect;
            return GUIRectangle.ScaleToOuterRect(rect, owner.GetScaledRect());
        }

        public void CreateChildrenComponents(XElement templateElem)
        {
            if (children == null)
            {
                children = new List<GUIEntity>();
            }

            foreach (XElement elem in templateElem.Elements())
            {
                GUIComponent newComponent = null;
                switch (elem.Name.ToString())
                {
                    case "Cond":
                        newComponent = new CondComponent(this, elem);
                        break;
                    case "Sprite":
                        newComponent = new SpriteComponent(this, elem);
                        break;
                    case "Text":
                        newComponent = new TextComponent(this, elem);
                        break;
                }
                if (newComponent != null) children.Add(newComponent);
            }
        }
    }
}
