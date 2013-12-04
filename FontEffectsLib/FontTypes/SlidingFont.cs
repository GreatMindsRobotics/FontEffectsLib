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

        /// <summary>
        /// Gets or sets the target position (the position to slide to).
        /// </summary>
        public virtual Vector2 TargetPosition
        {
            get { return _targetPosition; }
            set 
            { 
                _targetPosition = value;
                calculateSlide();
            }
        }

        /// <summary>
        /// Gets or sets the distance to slide.
        /// </summary>
        /// <remarks>
        /// The distance slided is calculated by multiplying the unit vector representing the direction from the position to the target positon by this speed by the time (in seconds) elapsed since the last frame.
        /// This value is further multiplied by the remaining distance to the target position. The final value is the distance that the sliding font moves forward.
        /// </remarks>
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
                    if (distanceToTarget > _targetTolerance)
                    {
                        _position += _unitVectorToTarget * distanceToTarget * (float)gameTime.ElapsedGameTime.TotalSeconds * _slideSpeed;
                    }
                    else
                    {
                        //We're close enough to the target to be considered there
                        _position = _targetPosition;

                        changeState(FontState.Done);
                    }
                    break;
            }
        }

        protected float _targetTolerance = 0.4825f;
        
        /// <summary>
        /// Gets or sets the distance from the target position at which the <see cref="SlidingFont"/> will be considered as done moving.
        /// </summary>
        public virtual float TargetTolerance
        {
            get { return _targetTolerance; }
            set { _targetTolerance = value; }
        }
        
        /// <summary>
        /// Sets the font to visible and begins sliding towards the target position.
        /// </summary>
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
