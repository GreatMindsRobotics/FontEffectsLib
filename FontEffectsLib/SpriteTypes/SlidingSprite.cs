using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FontEffectsLib.SpriteTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FontEffectsLib.SpriteTypes
{
    public class SlidingSprite : GameSprite
    {
        protected Vector2? _slideTo;

        /// <summary>
        /// Slide to position. Default is null (not sliding).
        /// </summary>        
        public Vector2? SlideTo
        {
            get { return _slideTo; }
            set
            {
                if (_slideTo.HasValue)
                {
                    return;
                }

                _slideTo = value;
            }
        }

        /// <summary>
        /// Slide update count. Default is 100 (slide in 100 updates)
        /// </summary>
        public int SlideUpdateCount { get; set; }

        protected int? _currentStep;
        protected Vector2 _startingPosition;

        public SlidingSprite(Texture2D texture, Vector2 position, Color tintColor)
            : base(texture, position, tintColor)
        {
            _slideTo = null;
            SlideUpdateCount = 100;

            _currentStep = null;
        }

        public override void Update(GameTime gameTime)
        {
            if (!_slideTo.HasValue)
            {
                return;
            }

            //If there is a position to slide to, save starting position and set step to zero
            if (!_currentStep.HasValue)
            {
                //Reset 
                _currentStep = 0;
                _startingPosition = _position;
            }
            else if (_currentStep.Value > SlideUpdateCount)
            {
                _slideTo = null;
                _currentStep = null;

                return;
            }

            _position = Vector2.SmoothStep(_startingPosition, _slideTo.Value, (float)_currentStep / (float)SlideUpdateCount);
            _currentStep++;


            base.Update(gameTime);
        }

        public void SlideToStartingPosition()
        {
            if (!_slideTo.HasValue)
            {
                _slideTo = _startingPosition;
            }

        }
    }
}
