using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FontEffectsLib.FontTypes
{
    public class AccelDropInFont : DropInFont
    {
        private Vector2 accel;

        public Vector2 _accel
        {
            get { return accel; }
            set { accel = value; }
        }


        public AccelDropInFont(SpriteFont font, Vector2 startPosition, Vector2 endPosition, Vector2 dropSpeed, Color tintColor, Vector2 accel)
            : this(font, startPosition, endPosition, dropSpeed, String.Empty, tintColor, accel)
        {
            //Pass-through constructor
        }

        public AccelDropInFont(SpriteFont font, Vector2 startPosition, Vector2 endPosition, Vector2 dropSpeed, String text, Color tintColor, Vector2 accel)
            : base(font, startPosition, endPosition, dropSpeed, text, tintColor)
        {
            _accel = accel;
        }

        public AccelDropInFont(SpriteFont font, Vector2 startPosition, Vector2 endPosition, Vector2 dropSpeed, String text, Color tintColor, Vector2 shadowPosition, Color shadowColor, Vector2 accel)
            : base(font, startPosition, endPosition, dropSpeed, text, tintColor, shadowPosition, shadowColor)
        {
            _accel = accel;
        }

        public override void Update(GameTime gameTime)
        {
            switch (_state)
            {
                case FontState.Drop:

                    _dropSpeed *= accel;

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
        }
    }
}
