using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FontEffectsLib.FontTypes
{
    public class ShadowFont : GameFont
    {
        protected Color _shadowColor;
        protected bool _enableShadow;
        protected Vector2 _shadowPosition;

        protected const float _defaultShadowSize = 4;

        public virtual Color ShadowColor
        {
            get { return _shadowColor; }
            set { _shadowColor = value; }
        }

        public virtual Vector2 ShadowPosition
        {
            get { return _shadowPosition; }
            set { _shadowPosition = value; }
        }

        public virtual bool EnableShadow
        {
            get { return _enableShadow; }
            set { _enableShadow = value; }
        }

         public ShadowFont(SpriteFont font, Vector2 position, Color tintColor)
            : this(font, String.Empty, position, tintColor, new Vector2(position.X - _defaultShadowSize, position.Y + _defaultShadowSize), Color.Black)
        {
             //Pass-through constructor
        }

         public ShadowFont(SpriteFont font, String text, Vector2 position, Color tintColor, Vector2 shadowPosition, Color shadowColor)
            : base(font, text, position, tintColor)
        {
            _shadowPosition = shadowPosition;
            _enableShadow = true;
            _shadowColor = shadowColor;
        }

         public override void Draw(SpriteBatch spriteBatch)
         {
             if (_enableShadow && _text != null)
             {
                spriteBatch.DrawString(_font, _text, _shadowPosition, _shadowColor, _rotation, _origin, _scale, _effects, _layerDepth);
             }

             base.Draw(spriteBatch);
         }
    }
}
