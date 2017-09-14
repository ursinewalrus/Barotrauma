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
        private static Dictionary<string,Texture2D> loadedTextures = null;

        public Texture2D texture;
        public GUIRectangle srcRect;
        public GUIRectangle destRect;

        private static GUIRectangle ParseRect(string txt)
        {
            GUIRectangle retVal = new GUIRectangle();
            string[] parts = txt.Split(',');
            float[] vals = new float[4];
            float[] valsFixed = new float[4];
            
            for (int i=0;i<4;i++)
            {
                bool parsed = false;
                for (int j=1;j<parts[i].Length;j++)
                {
                    if (parts[i][j]=='-' || parts[i][j]=='+')
                    {
                        string part1 = parts[i].Substring(0, j);
                        string part2 = parts[i].Substring(j);
                        part2 = part2.Substring(0, part2.Length - 2);
                        float.TryParse(part1, out vals[i]);
                        float.TryParse(part2, out valsFixed[i]);

                        parsed = true; break;
                    }
                }
                if (!parsed)
                {
                    if (parts[i].Contains("px"))
                    {
                        float.TryParse(parts[i].Substring(0,parts[i].Length-2),out valsFixed[i]);
                        vals[i] = 0.0f;
                    }
                    else
                    {
                        float.TryParse(parts[i], out vals[i]);
                        valsFixed[i] = 0.0f;
                    }
                }
            }
            retVal.ul = new Vector2(vals[0], vals[1]);
            retVal.br = new Vector2(vals[2], vals[3]);
            retVal.ulFixed = new Vector2(valsFixed[0], valsFixed[1]);
            retVal.brFixed = new Vector2(valsFixed[2], valsFixed[3]);

            retVal.Repair();
            return retVal;
        }

        public SpriteComponent(GUIEntity creator,XElement elem) : base(creator, elem)
        {
            string texName = ToolBox.GetAttributeString(elem, "texture", "");
            bool textureExists = false;
            if (loadedTextures == null)
            {
                loadedTextures = new Dictionary<string,Texture2D>();
            }
            else
            {
                if (loadedTextures.ContainsKey(texName))
                {
                    texture = loadedTextures[texName];
                    textureExists = true;
                }
            }

            if (!textureExists)
            {
                texture = TextureLoader.FromFile(texName);
                if (texture != null) loadedTextures.Add(texName, texture);
            }
            srcRect = ParseRect(ToolBox.GetAttributeString(elem, "src", "0,0,0,0"));
            destRect = ParseRect(ToolBox.GetAttributeString(elem, "dest", "0,0,0,0"));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //TODO: precalculate
            Rectangle xnaSrcRect = GUIRectangle.ScaleToXNARect(srcRect, texture.Bounds);
            Rectangle xnaDestRect = GUIRectangle.ScaleToXNARect(GUIRectangle.ScaleToOuterRect(destRect,owner.GetScaledRect()), new Rectangle(0, 0, GameMain.GraphicsWidth, GameMain.GraphicsHeight));

            spriteBatch.Draw(texture, xnaDestRect, xnaSrcRect, Color.White);

            base.Draw(spriteBatch);
        }
    }
}
