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

        public virtual Texture2D Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }

        public Rectangle? SourceRectangle
        {
            get { return _sourceRectangle; }
            set { _sourceRectangle = value; }
        }
        

        public GameSprite(Texture2D texture, Vector2 position, Color tintColor)
            : base(position, tintColor)
        {
            _texture = texture;
            _sourceRectangle = null;
        }
        
        public override void SetCenterAsOrigin()
        {
            _origin = new Vector2((float)_texture.Width / 2f, (float)_texture.Height / 2f);
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, _sourceRectangle, _tintColor, _rotation, _origin, _scale, _effects, _layerDepth);
        }
    }
}
