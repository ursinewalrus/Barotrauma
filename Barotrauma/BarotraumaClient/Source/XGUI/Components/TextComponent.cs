using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barotrauma.XGUI
{
    public class TextComponent : GUIComponent
    {
        public GUIRectangle rect;
        public bool wrapping = true;
        public List<Pair<bool,string>> lines;
    }
}
