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
    public class SpriteComponent : GUIComponent
    {
        public Texture2D texture;
        public GUIRectangle srcRect;
        public GUIRectangle destRect;

        public SpriteComponent(GUIObject creator,XElement elem)
        {
            owner = creator;

            texture = TextureLoader.FromFile(ToolBox.GetAttributeString(elem,"texture",""));
            srcRect = new GUIRectangle(ToolBox.GetAttributeVector4(elem,"src",Vector4.Zero));
            destRect = new GUIRectangle(ToolBox.GetAttributeVector4(elem, "dest", Vector4.Zero));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //TODO: precalculate
            Rectangle xnaSrcRect = GUIRectangle.ScaleToXNARect(srcRect, texture.Bounds);
            Rectangle xnaDestRect = GUIRectangle.ScaleToXNARect(GUIRectangle.ScaleToOuterRect(destRect,owner.rect), new Rectangle(0, 0, GameMain.GraphicsWidth, GameMain.GraphicsHeight));

            spriteBatch.Draw(texture, xnaDestRect, xnaSrcRect, Color.White);
        }
    }
}
