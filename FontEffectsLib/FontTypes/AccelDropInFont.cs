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
        private Vector2 _accel;
        private TimeSpan _accelDelayTarget;
        private Vector2 _maxDropSpeed;

        public Vector2 Accel
        {
            get { return _accel; }
            set { _accel = value; }
        }

        public TimeSpan AccelDelayTarget
        {
            get { return _accelDelayTarget; }
            set { _accelDelayTarget = value; }
        }


        public Vector2 MaxDropSpeed
        {
            get { return _maxDropSpeed; }
            set { _maxDropSpeed = value; }
        }

        TimeSpan accelDelay = new TimeSpan(0);
        TimeSpan accelDelayReset = new TimeSpan(0);

        public AccelDropInFont(SpriteFont font, Vector2 startPosition, Vector2 endPosition, Vector2 dropSpeed, Color tintColor, Vector2 accel)
            : this(font, startPosition, endPosition, dropSpeed, String.Empty, tintColor, accel)
        {
            //Pass-through constructor
        }

        public AccelDropInFont(SpriteFont font, Vector2 startPosition, Vector2 endPosition, Vector2 dropSpeed, String text, Color tintColor, Vector2 accel)
            : base(font, startPosition, endPosition, dropSpeed, text, tintColor)
        {
            Accel = accel;

            _accelDelayTarget = new TimeSpan(0, 0, 0, 0, 250);

            MaxDropSpeed = new Vector2(float.MaxValue);
        }

        public AccelDropInFont(SpriteFont font, Vector2 startPosition, Vector2 endPosition, Vector2 dropSpeed, String text, Color tintColor, Vector2 shadowPosition, Color shadowColor, Vector2 accel)
            : base(font, startPosition, endPosition, dropSpeed, text, tintColor, shadowPosition, shadowColor)
        {
            Accel = accel;

            _accelDelayTarget = new TimeSpan(0, 0, 0, 0, 250);

            MaxDropSpeed = new Vector2(float.MaxValue);
        }

        public AccelDropInFont(SpriteFont font, Vector2 startPosition, Vector2 endPosition, Vector2 dropSpeed, String text, Color tintColor, Vector2 shadowPosition, Color shadowColor, Vector2 accel, TimeSpan accelDelayTarget)
            : base(font, startPosition, endPosition, dropSpeed, text, tintColor, shadowPosition, shadowColor)
        {
            Accel = accel;

            AccelDelayTarget = accelDelayTarget;

            MaxDropSpeed = new Vector2(float.MaxValue);
        }

        public AccelDropInFont(SpriteFont font, Vector2 startPosition, Vector2 endPosition, Vector2 dropSpeed, String text, Color tintColor, Vector2 shadowPosition, Color shadowColor, Vector2 accel, Vector2 maxDropSpeed)
            : base(font, startPosition, endPosition, dropSpeed, text, tintColor, shadowPosition, shadowColor)
        {
            Accel = accel;

            AccelDelayTarget = new TimeSpan(0, 0, 0, 0, 250);

            MaxDropSpeed = maxDropSpeed;
        }

        public AccelDropInFont(SpriteFont font, Vector2 startPosition, Vector2 endPosition, Vector2 dropSpeed, String text, Color tintColor, Vector2 shadowPosition, Color shadowColor, Vector2 accel, TimeSpan accelDelayTarget, Vector2 maxDropSpeed)
            : base(font, startPosition, endPosition, dropSpeed, text, tintColor, shadowPosition, shadowColor)
        {
            Accel = accel;

            AccelDelayTarget = accelDelayTarget;

            MaxDropSpeed = maxDropSpeed;
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

                    if (MaxDropSpeed.Y > _dropSpeed.Y && MaxDropSpeed.X > _dropSpeed.X)
                    {
                        accelDelay += gameTime.ElapsedGameTime;

                        if (accelDelay >= AccelDelayTarget)
                        {
                            _dropSpeed *= Accel;
                            accelDelay = accelDelayReset;
                        }
                    }
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
