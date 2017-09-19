using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Barotrauma.XGUI;
using System.Xml.Linq;

namespace Barotrauma
{
    class EditMenuScreen : Screen
    {
        public EditMenuScreen()
        {
            ActionComponent.registeredActions.Add("EditMenuScreen", Select);
        }
    }
}
