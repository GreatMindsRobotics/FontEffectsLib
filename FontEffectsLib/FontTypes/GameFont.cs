using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FontEffectsLib.CoreTypes;

namespace FontEffectsLib.FontTypes
{
    public class GameFont : BaseGameObject
    {
        protected SpriteFont _font;
        protected StringBuilder _text;

        public virtual SpriteFont Font
        {
            get { return _font; }
            set { _font = value; }
        }

        public virtual StringBuilder Text
        {
            get { return _text; }
        }

        public GameFont(SpriteFont font, Vector2 position, Color tintColor)
            : this(font, String.Empty, position, tintColor)
        {
            //Pass-through constructor
        }

        public GameFont(SpriteFont font, String text, Vector2 position, Color tintColor)
            : base(position, tintColor)
        {
            _font = font;
            _text = new StringBuilder(text);
        }

        public override void SetCenterAsOrigin()
        {
            if (_text.Length > 0)
            {
                _origin = _font.MeasureString(_text) * _scale / 2;
            }
        }

        public virtual Vector2 Size
        {
            get { return _font.MeasureString(_text) * _scale; }
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_text != null)
            {
                spriteBatch.DrawString(_font, _text, _position, _tintColor, _rotation, _origin, _scale, _effects, _layerDepth);
            }
        }
    }
}
