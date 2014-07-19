using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FontEffectsLib.CoreTypes
{
    public abstract class BaseGameObject : ISprite
    {
        public event EventHandler VisibilityStateChanged;

        protected void FireVisibilityStateChangedEvent()
        {
            EventHandler handler = VisibilityStateChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected Vector2 _position;
        protected Color _tintColor;
        protected float _rotation;
        protected Vector2 _origin;
        protected Vector2 _scale;
        protected SpriteEffects _effects;
        protected float _layerDepth;
        protected bool _isVisible;

        protected object _tag;

        public virtual bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (value != _isVisible)
                {
                    _isVisible = value;
                    FireVisibilityStateChangedEvent();
                }
            }
        }

        public virtual Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public virtual Color TintColor
        {
            get { return _tintColor; }
            set { _tintColor = value; }
        }

        public virtual float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public virtual Vector2 Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }

        public virtual Vector2 Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public virtual SpriteEffects Effects
        {
            get { return _effects; }
            set { _effects = value; }
        }

        public virtual float LayerDepth
        {
            get { return _layerDepth; }
            set { _layerDepth = value; }
        }

        public virtual object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }


        public BaseGameObject(Vector2 position, Color tintColor)
        {
            _position = position;
            _tintColor = tintColor;

            _rotation = 0f;
            _scale = Vector2.One;
            _effects = SpriteEffects.None;
            _layerDepth = 0f;
            _isVisible = true;
        }

        public abstract void SetCenterAsOrigin();

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
