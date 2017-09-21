using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;
using System.IO;
using EventInput;

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
            Vector2 ul = new Vector2((float)Math.Round((float)xnaRect.X + (guiRect.ul.X * xnaRect.Width) + guiRect.ulFixed.X),
                                     (float)Math.Round((float)xnaRect.Y + (guiRect.ul.Y * xnaRect.Height) + guiRect.ulFixed.Y));
            Vector2 br = new Vector2((float)Math.Round((float)xnaRect.X + (guiRect.br.X * xnaRect.Width) + guiRect.brFixed.X),
                                     (float)Math.Round((float)xnaRect.Y + (guiRect.br.Y * xnaRect.Height) + guiRect.brFixed.Y));
            return new Rectangle((int)ul.X,
                                 (int)ul.Y,
                                 (int)(br.X-ul.X),
                                 (int)(br.Y-ul.Y));
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

    public class GUI : GUIEntity
    {
        public Dictionary<string, ScalableFont> fonts;
        public GraphicsDevice graphicsDevice;

        private Texture2D blankTex;
        
        public Dictionary<string,XElement> templates;
        public Dictionary<string, List<GUIObject>> menus;
        
        public KeyboardDispatcher KeyboardDispatcher
        {
            get; private set;
        }

        public string currentMenu = "";

        public void LoadTemplates(string directory)
        {
            foreach (string filename in Directory.GetFiles(directory))
            {
                XDocument doc = ToolBox.TryLoadXml(filename);
                XElement root = doc.Root;
                if (root.Name != "GUI") return;
                foreach (XElement elem in root.Elements())
                {
                    if (elem.Name.ToString() != "Template") continue;

                    templates.Add(ToolBox.GetAttributeString(elem, "name", "<no name>"), elem);
                }
            }
        }

        public void LoadMenus(string directory)
        {
            foreach (string filename in Directory.GetFiles(directory))
            {
                XDocument doc = ToolBox.TryLoadXml(filename);
                XElement root = doc.Root;
                if (root.Name != "GUI") return;
                foreach (XElement elem in root.Elements())
                {
                    if (elem.Name.ToString() != "Menu") continue;

                    List<GUIObject> newMenu = new List<GUIObject>();
                    foreach (XElement objElem in elem.Elements())
                    {
                        newMenu.Add(new GUIObject(this, objElem));
                    }
                    menus.Add(ToolBox.GetAttributeString(elem, "name", "<no name>"), newMenu);
                }
            }
        }

        public GUI() : base(null,null)
        {
            fonts = new Dictionary<string, ScalableFont>();
            templates = new Dictionary<string, XElement>();
            menus = new Dictionary<string, List<GUIObject>>();

            ActionComponent.registeredActions.Add("ChangeMenu", ChangeMenu);
        }

        public void Init(GameWindow window)
        {
            fonts.Add("default", new ScalableFont("Content/Exo2-Medium.otf",(uint)(14*GameMain.GraphicsHeight/720),graphicsDevice));

            // create 1x1 texture for line drawing
            blankTex = new Texture2D(graphicsDevice, 1, 1);
            blankTex.SetData(new Color[] { Color.White });// fill the texture with white

            KeyboardDispatcher = new KeyboardDispatcher(window);
        }

        public override void Update(float deltaTime) {
            if (!menus.ContainsKey(currentMenu)) return;

            foreach (GUIObject obj in menus[currentMenu])
            {
                obj.ResetMouseOn();
            }

            for (int i=menus[currentMenu].Count-1;i>=0;i--)
            {
                GUIObject obj = menus[currentMenu][i];
                if (obj.UpdateMouseOn()) break;
            }

            foreach (GUIObject obj in menus[currentMenu])
            {
                obj.Update(deltaTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!menus.ContainsKey(currentMenu)) return;
            foreach (GUIObject obj in menus[currentMenu])
            {
                obj.Draw(spriteBatch);
            }
        }

        public void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color clr, float depth = 0.0f, int width = 1)
        {
            //TODO: replace this with primitives for performance, calculating angles can't be a good idea here

            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            
            sb.Draw(blankTex,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    width), //width of line, change this to make thicker line
                null,
                clr, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                depth);
        }

        public void DrawRectangle(SpriteBatch sb, Vector2 start, Vector2 size, Color clr, bool isFilled = false, float depth = 0.0f, int thickness = 1)
        {
            if (size.X < 0)
            {
                start.X += size.X;
                size.X = -size.X;
            }
            if (size.Y < 0)
            {
                start.Y += size.Y;
                size.Y = -size.Y;
            }
            DrawRectangle(sb, new Rectangle((int)start.X, (int)start.Y, (int)size.X, (int)size.Y), clr, isFilled, depth, thickness);
        }

        public void DrawRectangle(SpriteBatch sb, Rectangle rect, Color clr, bool isFilled = false, float depth = 0.0f, int thickness = 1)
        {
            if (isFilled)
            {
                sb.Draw(blankTex, rect, null, clr, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
            }
            else
            {
                sb.Draw(blankTex, new Rectangle(rect.X + thickness, rect.Y, rect.Width - thickness * 2, thickness), null, clr, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
                sb.Draw(blankTex, new Rectangle(rect.X + thickness, rect.Y + rect.Height - thickness, rect.Width - thickness * 2, thickness), null, clr, 0.0f, Vector2.Zero, SpriteEffects.None, depth);

                sb.Draw(blankTex, new Rectangle(rect.X, rect.Y, thickness, rect.Height), null, clr, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
                sb.Draw(blankTex, new Rectangle(rect.X + rect.Width - thickness, rect.Y, thickness, rect.Height), null, clr, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
            }
        }

        public void ChangeMenu(string parameters)
        {
            if (menus.ContainsKey(parameters)) currentMenu = parameters;
        }
    }
}
