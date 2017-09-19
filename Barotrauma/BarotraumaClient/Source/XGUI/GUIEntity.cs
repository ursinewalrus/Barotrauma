using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
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

            children = null;
            if (elem != null)
            {
                children = CreateChildrenEntities(this,elem);
            }
        }

        public virtual void Update(float deltaTime)
        {
            if (children.Count == 0) return;
            children.ForEach(c => c.Update(deltaTime));
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (children.Count == 0) return;
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

        public static List<GUIEntity> CreateChildrenEntities(GUIEntity owner,XElement xElem)
        {
            List<GUIEntity> entities = new List<GUIEntity>();
            foreach (XElement elem in xElem.Elements())
            {
                GUIEntity newEntity = null;
                switch (elem.Name.ToString())
                {
                    case "Object":
                        newEntity = new GUIObject(owner, elem);
                        break;
                    case "Cond":
                        newEntity = new CondComponent(owner, elem);
                        break;
                    case "Action":
                        newEntity = new ActionComponent(owner, elem);
                        break;
                    case "Sprite":
                        newEntity = new SpriteComponent(owner, elem);
                        break;
                    case "Text":
                        newEntity = new TextComponent(owner, elem);
                        break;
                }
                if (newEntity != null) entities.Add(newEntity);
            }
            return entities;
        }
    }
}
