using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FontEffectsLib.CoreTypes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FontEffectsLib.FontTypes
{
    public class SlidingFont : ShadowFont, IStateful
    {
        public event EventHandler<StateEventArgs> StateChanged;

        public enum FontState
        {
            Ready,
            Sliding,
            Done
        }

        //State handling
        protected FontState _state;

        protected Vector2 _targetPosition;
        protected float _slideSpeed;

        //Calculations
        protected float _distanceToTarget;
        protected Vector2 _unitVectorToTarget;

        public virtual Vector2 TargetPosition
        {
            get { return _targetPosition; }
            set 
            { 
                _targetPosition = value;
                calculateSlide();
            }
        }

        public virtual float SlideSpeed
        {
            get { return _slideSpeed; }
            set { _slideSpeed = value; }
        }

        public SlidingFont(SpriteFont font, Vector2 startPosition, Vector2 endPosition, float slideSpeed, Color tintColor)
            : this(font, startPosition, endPosition, slideSpeed, String.Empty, tintColor)
        {
            //Pass-through constructor
        }

        public SlidingFont(SpriteFont font, Vector2 startPosition, Vector2 endPosition, float slideSpeed, String text, Color tintColor)
            : base(font, startPosition, tintColor)
        {
            _targetPosition = endPosition;
            _slideSpeed = slideSpeed;
            _text = new StringBuilder(text);

            calculateSlide();
            changeState(FontState.Ready);
        }

        public SlidingFont(SpriteFont font, Vector2 startPosition, Vector2 endPosition, float slideSpeed, String text, Color tintColor, Vector2 shadowPosition, Color shadowColor)
            : base(font, text, startPosition, tintColor, shadowPosition, shadowColor)
        {
            _targetPosition = endPosition;
            _slideSpeed = slideSpeed;

            calculateSlide();
            changeState(FontState.Ready);
        }

        public override void Update(GameTime gameTime)
        {
            if (!_isVisible)
            {
                return;
            }

            switch (_state)
            {
                case FontState.Ready:
                    break;

                case FontState.Done:
                    _position = _targetPosition;
                    changeState(FontState.Ready);
                    break;

                case FontState.Sliding:
                    float distanceToTarget = Math.Abs((_targetPosition - _position).Length());
                    if (distanceToTarget > 0.5f)
                    {
                        _position += _unitVectorToTarget * distanceToTarget * (float)gameTime.ElapsedGameTime.TotalSeconds * _slideSpeed;
                    }
                    else
                    {
                        changeState(FontState.Done);
                    }
                    break;
            }
        }

        public virtual void Slide()
        {
            IsVisible = true;
            changeState(FontState.Sliding);
        }

        protected virtual void calculateSlide()
        {
            _unitVectorToTarget = _targetPosition - _position;
            _unitVectorToTarget.Normalize();
        }

        protected virtual void changeState(FontState newState)
        {
            _state = newState;

            if (StateChanged != null)
            {
                StateChanged(this, new StateEventArgs(newState.GetType(), newState));
            }
        }
    }
}
