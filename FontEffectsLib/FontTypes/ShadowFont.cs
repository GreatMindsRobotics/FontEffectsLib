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
        protected bool _usingRelativePosition = true;

        protected Vector2 _shadowSize = new Vector2(4);

        /// <summary>
        /// Relative shadow position
        /// </summary>
        public virtual Vector2 ShadowSize
        {
            get { return _shadowSize; }
            set
            {
                if (_shadowSize != value)
                {
                    _shadowSize = value;
                    _usingRelativePosition = true;
                }
            }
        }
        

        protected const float _defaultShadowSize = 4;

        public virtual Color ShadowColor
        {
            get { return _shadowColor; }
            set { _shadowColor = value; }
        }

        public virtual Vector2 ShadowPosition
        {
            get { return _usingRelativePosition ? Position + _shadowSize : _shadowPosition; }
            set { _shadowPosition = value; _usingRelativePosition = false; }
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
            ShadowPosition = shadowPosition;
            _enableShadow = true;
            _shadowColor = shadowColor;
        }

         public override void Draw(SpriteBatch spriteBatch)
         {
             if (!_isVisible)
             {
                 return;
             }

             if (_enableShadow && _text != null)
             {
                spriteBatch.DrawString(_font, _text, ShadowPosition, _shadowColor, _rotation, _origin, _scale, _effects, _layerDepth);
             }

             base.Draw(spriteBatch);
         }
    }
}
