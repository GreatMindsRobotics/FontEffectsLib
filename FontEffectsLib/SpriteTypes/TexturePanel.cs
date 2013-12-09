using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FontEffectsLib.SpriteTypes
{
    /// <summary>
    /// A panel which contains a texture.
    /// </summary>
    public class TexturePanel : Panel
    {
        /// <summary>
        /// Creates a new panel housing a texture.
        /// </summary>
        /// <param name="image">The image to display within the panel.</param>
        /// <param name="speed">The speed of expansion and collapsing.</param>
        /// <param name="position">The position of the center of the panel. The panel expands/collapses from/to this point.</param>
        /// <param name="color">The tint color of the panel.</param>
        public TexturePanel(Texture2D image, Vector2 speed, Vector2 position, Color color)
            : base(image.GraphicsDevice, new Vector2(image.Width, image.Height), speed, position, color)
        {
            _texture = image;
        }
    }
}
