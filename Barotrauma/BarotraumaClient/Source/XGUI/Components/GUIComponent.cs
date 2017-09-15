using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;

namespace Barotrauma.XGUI
{
    public abstract class GUIComponent : GUIEntity
    {
        public GUIComponent(GUIEntity creator, XElement elem) : base(creator,elem)
        {
        }
    }
}
