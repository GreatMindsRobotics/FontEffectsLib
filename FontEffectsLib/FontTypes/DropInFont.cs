using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FontEffectsLib.CoreTypes;

namespace FontEffectsLib.FontTypes
{
    public class DropInFont : ShadowFont, IStateful
    {
        public event EventHandler<StateEventArgs> StateChanged;

        public enum FontState
        {
            Drop,
            Compress,
            Expand,
            Done
        }

        protected Vector2 _targetPosition;
        protected Vector2 _dropSpeed;
        protected Vector2 _startingPosition;
        protected Vector2 _startingShadowPosition;

        //State handling
        protected FontState _state;

        //Bounce action
        protected Vector2 _fontSize;
        protected Vector2 _originalScale;
        protected float _step = 0.05f;
        protected Vector2 _targetScale;

        public virtual Vector2 TargetPosition
        {
            get { return _targetPosition; }
            set { _targetPosition = value; }
        }

        public virtual Vector2 DropSpeed
        {
            get { return _dropSpeed; }
            set { _dropSpeed = value; }
        }

        public DropInFont(SpriteFont font, Vector2 startPosition, Vector2 endPosition, Vector2 dropSpeed, Color tintColor)
            : this(font, startPosition, endPosition, dropSpeed, String.Empty, tintColor)
        {
            //Pass-through constructor
        }

        public DropInFont(SpriteFont font, Vector2 startPosition, Vector2 endPosition, Vector2 dropSpeed, String text, Color tintColor)
            : base(font, startPosition, tintColor)
        {
            _startingPosition = startPosition;
            _targetPosition = endPosition;
            _dropSpeed = dropSpeed;
            _text = new StringBuilder(text);
            _originalScale = _scale;
            _startingShadowPosition = _shadowPosition;

            Reset();
        }

        public DropInFont(SpriteFont font, Vector2 startPosition, Vector2 endPosition, Vector2 dropSpeed, String text, Color tintColor, Vector2 shadowPosition, Color shadowColor)
            : base(font, text, startPosition, tintColor, shadowPosition, shadowColor)
        {
            _startingPosition = startPosition;
            _targetPosition = endPosition;
            _dropSpeed = dropSpeed;
            _originalScale = _scale;
            _startingShadowPosition = _shadowPosition;

            Reset();
        }

        public void Reset()
        {
            _position = _startingPosition;
            _shadowPosition = _startingShadowPosition;

            _scale = _originalScale;

            changeState(FontState.Drop);
        }

        public override void Update(GameTime gameTime)
        {
            if (!_isVisible)
            {
                return;
            }

            switch (_state)
            {
                case FontState.Drop:
                    if (_position.X < _targetPosition.X)
                    {
                        if (_position.X + _dropSpeed.X < _targetPosition.X)
                        {
                            _position.X += _dropSpeed.X;
                            _shadowPosition.X += _dropSpeed.X;
                        }
                        else
                        {
                            _shadowPosition.X += _targetPosition.X - _position.X;
                            _position.X = _targetPosition.X;
                        }
                    }

                    if (_position.Y < _targetPosition.Y)
                    {
                        if (_position.Y + _dropSpeed.Y < _targetPosition.Y)
                        {
                            _position.Y += _dropSpeed.Y;
                            _shadowPosition.Y += _dropSpeed.Y;
                        }
                        else
                        {
                            _shadowPosition.Y += _targetPosition.Y - _position.Y;
                            _position.Y = _targetPosition.Y;
                        }
                    }

                    if (_position.X >= _targetPosition.X && _position.Y >= _targetPosition.Y)
                    {
                        _fontSize = _font.MeasureString(_text);
                        _originalScale = _scale;
                        _targetScale.X = _scale.X * 1.2f;
                        _targetScale.Y = _scale.Y * .7f;

                        changeState(FontState.Compress);
                    }
                    break;

                case FontState.Compress:

                    if (_scale.Y > _targetScale.Y)
                    {
                        _scale.Y -= _step;
                        _position.Y += (_step * _fontSize.Y) / 2;
                        _shadowPosition.Y += (_step * _fontSize.Y) / 2;

                        if (_scale.X < _targetScale.X)
                        {
                            _scale.X += _step / 2;
                        }
                    }
                    else
                    {
                        changeState(FontState.Expand);
                    }

                    break;

                case FontState.Expand:
                    if (_scale.Y < _originalScale.Y)
                    {
                        _scale.Y += _step;
                        _position.Y -= (_step * _fontSize.Y) / 2;
                        _shadowPosition.Y -= (_step * _fontSize.Y) / 2;

                        if (_scale.X > _originalScale.X)
                        {
                            _scale.X -= _step / 2;
                        }
                    }
                    else
                    {
                        changeState(FontState.Done);
                    }
                    break;

                case FontState.Done:
                    break;

            }
            base.Update(gameTime);
        }

        protected virtual void changeState(FontState newState)
        {
            _state = newState;

            EventHandler<StateEventArgs> handler = StateChanged;
            if (handler != null)
            {
                handler(this, new StateEventArgs(typeof(DropInFont.FontState), newState));
            }
        }
    }
}
