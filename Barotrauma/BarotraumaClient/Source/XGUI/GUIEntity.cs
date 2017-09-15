using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;

namespace Barotrauma.XGUI
{
    abstract public class GUIEntity
    {
        public GUIRectangle rect;
        public GUIEntity owner;
        public List<GUIEntity> children;

        public GUIEntity(GUIEntity creator, XElement elem)
        {
            rect = new GUIRectangle(0, 0, 1, 1);

            owner = creator;

            if (elem!=null) CreateChildrenEntities(elem);
        }

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

        public GUI GetXGUI()
        {
            if (this is GUI) return (this as GUI);

            GUIEntity parentEntity = owner;
            while (!(parentEntity is GUI))
            {
                parentEntity = parentEntity.owner;
            }
            GUI parentObject = parentEntity as GUI;

            return parentObject;
        }

        public GUIRectangle GetScaledRect()
        {
            if (owner == null) return rect;
            return GUIRectangle.ScaleToOuterRect(rect, owner.GetScaledRect());
        }

        public void CreateChildrenEntities(XElement templateElem)
        {
            if (children == null)
            {
                children = new List<GUIEntity>();
            }

            foreach (XElement elem in templateElem.Elements())
            {
                GUIEntity newEntity = null;
                switch (elem.Name.ToString())
                {
                    case "Object":
                        newEntity = new GUIObject(this, elem);
                        break;
                    case "Cond":
                        newEntity = new CondComponent(this, elem);
                        break;
                    case "Action":
                        newEntity = new ActionComponent(this, elem);
                        break;
                    case "Sprite":
                        newEntity = new SpriteComponent(this, elem);
                        break;
                    case "Text":
                        newEntity = new TextComponent(this, elem);
                        break;
                }
                if (newEntity != null) children.Add(newEntity);
            }
        }
    }
}
