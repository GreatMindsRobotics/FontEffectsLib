using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace FontEffectsLib.CoreTypes
{
    public static class Extensions
    {
        public static float HalfWidth(this Viewport viewport)
        {
            return viewport.Width / 2;
        }

        public static float HalfHeight(this Viewport viewport)
        {
            return viewport.Height / 2;
        }
    }
}
