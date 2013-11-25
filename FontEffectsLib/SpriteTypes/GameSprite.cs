using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FontEffectsLib.CoreTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FontEffectsLib.SpriteTypes
{
    public class GameSprite : BaseGameObject
    {
        protected Texture2D _texture;
        protected Rectangle? _sourceRectangle;

        /// <summary>
        /// Gets or sets the <see cref="Texture2D"/> to render.
        /// </summary>
        public virtual Texture2D Texture
        {
            get { return _texture; }
            set {
                if (value == null)
                {
                    throw new ArgumentNullException("Texture");
                }
                _texture = value; }
        }

        public Rectangle? SourceRectangle
        {
            get { return _sourceRectangle; }
            set { _sourceRectangle = value; }
        }
        
        /// <summary>
        /// Creates a new <see cref="GameSprite"/>.
        /// </summary>
        /// <param name="texture">The image to display on the <see cref="GameSprite"/>.</param>
        /// <param name="position"></param>
        /// <param name="tintColor"></param>
        public GameSprite(Texture2D texture, Vector2 position, Color tintColor)
            : base(position, tintColor)
        {
            if (texture == null)
            {
                throw new ArgumentNullException("texture");
            }

            _texture = texture;
            _sourceRectangle = null;
        }
        
        /// <summary>
        /// Sets the center of this <see cref="GameSprite"/> as the rendering origin.
        /// </summary>
        public override void SetCenterAsOrigin()
        {
            _origin = new Vector2((float)_texture.Width / 2f, (float)_texture.Height / 2f);
        }

        /// <summary>
        /// Updates this <see cref="GameSprite"/>.
        /// </summary>
        /// <remarks>
        /// Intended to be overridden by a subclass.
        /// </remarks>
        /// <param name="gameTime">A snapshot of the game timing state values.</param>
        public override void Update(GameTime gameTime)
        {
            
        }

        /// <summary>
        /// Draws this <see cref="GameSprite"/> to the specified <see cref="SpriteBatch"/>.
        /// </summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> to render this <see cref="GameSprite"/> to.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_isVisible == true)
            {
                spriteBatch.Draw(_texture, _position, _sourceRectangle, _tintColor, _rotation, _origin, _scale, _effects, _layerDepth);
            }
        }
    }
}
