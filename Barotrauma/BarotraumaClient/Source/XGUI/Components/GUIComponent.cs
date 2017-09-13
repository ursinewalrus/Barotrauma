using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Barotrauma.XGUI
{
    public abstract class GUIComponent
    {
        protected GUIObject owner;

        public virtual void Draw(SpriteBatch spriteBatch) { }
        public virtual void Update(float deltaTime) { }
    }
}
