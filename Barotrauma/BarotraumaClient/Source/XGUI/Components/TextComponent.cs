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
    public class TextComponent : GUIComponent
    {
        //public bool wrapping = true;
        //public List<Pair<bool,string>> lines;
        public string str;
        public string halign;
        public string valign;
        ScalableFont font;

        public TextComponent(GUIEntity creator, XElement elem) : base(creator, elem)
        {
            str = ToolBox.GetAttributeString(elem, "str", "");

            //owner = creator;
            GUIObject parentObject = GetParentObject();

            if (str[0]=='$')
            {
                str = parentObject.attribs[str.Substring(1)];
            }

            rect = new GUIRectangle(ToolBox.GetAttributeVector4(elem, "rect", Vector4.Zero));
            font = parentObject.XGUI.fonts[ToolBox.GetAttributeString(elem, "font", "default")];

            halign = ToolBox.GetAttributeString(elem, "halign", "left");
            valign = ToolBox.GetAttributeString(elem, "valign", "top");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 size = font.MeasureString(str);
            Rectangle targetRect = GUIRectangle.ScaleToXNARect(GUIRectangle.ScaleToOuterRect(rect, owner.GetScaledRect()), new Rectangle(0, 0, GameMain.GraphicsWidth, GameMain.GraphicsHeight));
            Vector2 pos = new Vector2(targetRect.X,targetRect.Y);

            if (halign == "center")
            {
                pos.X = targetRect.X + targetRect.Width / 2 - size.X / 2;
            }
            else if (halign == "right")
            {
                pos.X = targetRect.X + targetRect.Width - size.X;
            }
            if (valign == "center")
            {
                pos.Y = targetRect.Y + targetRect.Height / 2 - size.Y / 2;
            }
            else if (halign == "bottom")
            {
                pos.Y = targetRect.Y + targetRect.Height - size.Y;
            }

            pos.X = (float)Math.Round(pos.X); pos.Y = (float)Math.Round(pos.Y);

            font.DrawString(spriteBatch, str, pos, Color.White);

            base.Draw(spriteBatch);
        }
    }
}
