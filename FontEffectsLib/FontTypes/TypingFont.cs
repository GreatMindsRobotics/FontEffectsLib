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

        private string _textToType = String.Empty;
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
                return _font.MeasureString(_textToType) * _scale;
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
            _text.Clear();
            _text.Append(_textToType);
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

        public TypingFont(SpriteFont font, Vector2 position, Color color, string textToType, int delayTime):
            base(font, position, color)
        {
            _elapsedDelayTime = TimeSpan.Zero;
            _delayTime = TimeSpan.FromMilliseconds(delayTime);
            _currentLetterIndex = -1;
            _textToType = textToType;
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
                            Finished();
                            break;
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
