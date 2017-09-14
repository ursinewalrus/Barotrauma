using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Barotrauma.XGUI
{
    public struct GUIRectangle
    {
        public Vector2 ul;
        public Vector2 br;
        public Vector2 ulFixed;
        public Vector2 brFixed;

        public float X
        {
            get { return ul.X; }
            set { float oldX = ul.X; ul.X = value; br.X += ul.X - oldX; }
        }
        public float Y
        {
            get { return ul.Y; }
            set { float oldY = ul.Y; ul.Y = value; br.Y += ul.Y - oldY; }
        }
        public float Width
        {
            get { return br.X - ul.X; }
            set { br.X = ul.X + value; }
        }
        public float Height
        {
            get { return br.Y - ul.Y; }
            set { br.Y = ul.Y + value; }
        }

        public GUIRectangle(Vector2 a, Vector2 b)
        {
            ulFixed = Vector2.Zero; brFixed = Vector2.Zero;
            ul = a; br = b; Repair();
        }

        public GUIRectangle(float ax,float ay,float bx,float by)
        {
            ulFixed = Vector2.Zero; brFixed = Vector2.Zero;
            ul = new Vector2(ax, ay); br = new Vector2(bx, by); Repair();
        }

        public GUIRectangle(Vector4 vec4)
        {
            ulFixed = Vector2.Zero; brFixed = Vector2.Zero;
            ul = new Vector2(vec4.X, vec4.Y); br = new Vector2(vec4.Z, vec4.W); Repair();
        }

        public void AddPoint(Vector2 point)
        {
            ul = new Vector2(Math.Min(ul.X, point.X), Math.Min(ul.Y, point.Y));
            br = new Vector2(Math.Max(br.X, point.X), Math.Max(br.Y, point.Y));
        }

        public void Repair()
        {
            Vector2 a = ul; Vector2 b = br;
            ul = new Vector2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
            br = new Vector2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
        }
        
        public static Rectangle ScaleToXNARect(GUIRectangle guiRect,Rectangle xnaRect)
        {
            return new Rectangle((int)((float)xnaRect.X + (guiRect.ul.X * xnaRect.Width) + guiRect.ulFixed.X),
                                 (int)((float)xnaRect.Y + (guiRect.ul.Y * xnaRect.Height) + guiRect.ulFixed.Y),
                                 (int)((guiRect.br.X - guiRect.ul.X) * xnaRect.Width - guiRect.ulFixed.X + guiRect.brFixed.X),
                                 (int)((guiRect.br.Y - guiRect.ul.Y) * xnaRect.Height - guiRect.ulFixed.Y + guiRect.brFixed.Y));
        }

        public static GUIRectangle ScaleToOuterRect(GUIRectangle innerRect,GUIRectangle outerRect)
        {
            GUIRectangle retVal = new GUIRectangle(outerRect.ul.X + (innerRect.ul.X * outerRect.Width),
                                    outerRect.ul.Y + (innerRect.ul.Y * outerRect.Height),
                                    outerRect.ul.X + (innerRect.br.X * outerRect.Width),
                                    outerRect.ul.Y + (innerRect.br.Y * outerRect.Height));
            retVal.ulFixed = innerRect.ulFixed;
            retVal.brFixed = innerRect.brFixed;
            return retVal;
        }
    }

    public class GUI
    {
        public static ScalableFont Font, SmallFont, LargeFont;

        public List<GUIObject> objects;

        public GUI()
        {
            objects = new List<GUIObject>();
        }

        public void Update(float deltaTime) {
            foreach (GUIObject obj in objects)
            {
                obj.Update(deltaTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (GUIObject obj in objects)
            {
                obj.Draw(spriteBatch);
            }
        }
    }
}
