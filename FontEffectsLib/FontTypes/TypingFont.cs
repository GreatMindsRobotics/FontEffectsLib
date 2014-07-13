using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FontEffectsLib.CoreTypes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FontEffectsLib.FontTypes
{
    public class TypingFont : GameFont, IStateful
    {
        public enum TypingState
        {
            NotStarted,
            Typing,
            Finished
        }

        public event EventHandler<StateEventArgs> StateChanged;

        /// <summary>
        /// Since we're doing primarily array access, an array is better used internally.
        /// </summary>
        private char[] _textToType = new char[0];

        /// <summary>
        /// This value can be cached as the text to type is immutable after creation.
        /// </summary>
        private Vector2 _cachedTextTypeSize;

        private int _currentLetterIndex;

        private TimeSpan _delayTime;
        private TimeSpan _elapsedDelayTime;

        private TypingState _currentState;

        public TypingState State
        {
            get
            {
                return _currentState;
            }
        }

        public override Vector2 Size
        {
            get
            {
                return _cachedTextTypeSize * _scale;
            }
        }

        public void Reset()
        {
            _currentState = TypingState.NotStarted;
            _text.Clear();
            _currentLetterIndex = 0;
            if (StateChanged != null)
            {
                StateChanged(this, new StateEventArgs(typeof(TypingState), TypingState.NotStarted));
            }
        }

        public void Finished()
        {
            Finished(true);
        }

        protected void Finished(bool typeText)
        {
            if (typeText)
            {
                _text.Clear();
                _text.Append(_textToType);
            }
            _currentState = TypingState.Finished;
            if (StateChanged != null)
            {
                StateChanged(this, new StateEventArgs(typeof(TypingState), TypingState.Finished));
            }
        }

        public void Start()
        {
            _currentState = TypingState.Typing;
        }

        public TypingFont(SpriteFont font, Vector2 position, Color color, string textToType, TimeSpan delayTime) :
            base(font, position, color)
        {
            _elapsedDelayTime = TimeSpan.Zero;
            _delayTime = delayTime;
            _currentLetterIndex = -1;
            _textToType = textToType.ToCharArray();
            _cachedTextTypeSize = _font.MeasureString(textToType);
            _currentState = TypingState.NotStarted;
        }

        public override void Update(GameTime gameTime)
        {

            switch (_currentState)
            {
                case TypingState.Typing:
                    _elapsedDelayTime += gameTime.ElapsedGameTime;

                    if (_elapsedDelayTime >= _delayTime)
                    {
                        _elapsedDelayTime = TimeSpan.Zero;
                        _currentLetterIndex++;


                        if (_currentLetterIndex >= _textToType.Length)
                        {
                            Finished(false);
                            return;
                        }

                        _text.Append(_textToType[_currentLetterIndex]);

                        
                        
                    }

                    break;

                default:
                    //pass
                    break;
            }

            base.Update(gameTime);
        }
    }
}
