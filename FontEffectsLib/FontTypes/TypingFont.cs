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
        #region Obsolete types support

        /// <summary>
        /// This type provides backwards compatibility and legacy support for:
        ///    public enum TypingFont.TypingState (now deprecated)
        /// </summary>
        [Obsolete("Please use TypingFont.FontState instead", false)]
        public enum TypingState
        {
            /// <summary>           
            /// Typing has not started yet
            /// </summary>
            [Obsolete("Please use TypingFont.FontState.NotStarted instead", false)]
            NotTyping,

            /// <summary>
            /// Typing letters
            /// </summary>
            [Obsolete("Please use TypingFont.FontState.Typing instead", false)]
            Typing = 1,

            /// <summary>
            /// Finished typing all letters.
            /// </summary>
            [Obsolete("Please use TypingFont.FontState.Finished instead", false)]
            Finished = 2
        }

        #endregion Obsolete types support

        /// <summary>
        /// Indicates the state of this TypingFont
        /// </summary>
        public enum FontState
        {
            /// <summary>
            /// Typing has not started yet
            /// </summary>
            NotStarted,
            
            /// <summary>
            /// Typing letters
            /// </summary>
            Typing,

            /// <summary>
            /// Finished typing all letters.
            /// </summary>
            Finished
        }


        public class CharacterTypedEventArgs : EventArgs
        {
            /// <summary>
            /// Gets the character that was just typed by the TypingFont
            /// </summary>
            public char TypedCharacter { get; set; }

            /// <summary>
            /// Creates instance of CharacterTypedEventArgs class
            /// </summary>
            /// <param name="typedCharacter">The character about to be typed (same update)</param>
            public CharacterTypedEventArgs(char typedCharacter)
            {
                TypedCharacter = typedCharacter;
            }
        }

        /// <summary>
        /// Fires when the font state changes.
        /// StateEventArgs.DataType is TypingFont.FontState
        /// StateEventArgs.Data is the new font state.
        /// </summary>
        public event EventHandler<StateEventArgs> StateChanged;

        /// <summary>
        /// Fires whenever a single character is typed. 
        /// CharacterTypedEventArgs.TypedCharacter contains the character about to be typed (same update)
        /// </summary>
        public event EventHandler<CharacterTypedEventArgs> CharacterTyped;

        /// <summary>
        /// Since we're doing primarily array access, an array is better used internally.
        /// </summary>
        private char[] _textToType = new char[0];

        /// <summary>
        /// This value can be cached as the text to type is immutable after creation.
        /// </summary>
        private Vector2 _cachedTextTypeSize;

        private int _currentLetterIndex;

        public TimeSpan DelayTime { get; set; }

        private TimeSpan _elapsedDelayTime;

        private FontState _state;


        public FontState State
        {
            get
            {
                return _state;
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
            _state = FontState.NotStarted;
            _text.Clear();
            _currentLetterIndex = -1;

            changeState(FontState.NotStarted);
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

            changeState(FontState.Finished);
        }

        public void Start()
        {
            changeState(FontState.Typing);
        }

        public TypingFont(SpriteFont font, Vector2 position, Color color, string textToType, TimeSpan delayTime) :
            base(font, position, color)
        {
            _elapsedDelayTime = TimeSpan.Zero;
            DelayTime = delayTime;
            _currentLetterIndex = -1;
            _textToType = textToType.ToCharArray();
            _cachedTextTypeSize = _font.MeasureString(textToType);
            _state = FontState.NotStarted;
        }

        public override void Update(GameTime gameTime)
        {

            switch (_state)
            {
                case FontState.Typing:
                    _elapsedDelayTime += gameTime.ElapsedGameTime;

                    if (_elapsedDelayTime >= DelayTime)
                    {
                        _elapsedDelayTime = TimeSpan.Zero;                                               
                        _currentLetterIndex++;
     
                        if (_currentLetterIndex >= _textToType.Length)
                        {
                            Finished(false);
                            return;
                        }

                        EventHandler<CharacterTypedEventArgs> handler = CharacterTyped;                
                        if (handler != null)
                        { 
                            handler(this, new CharacterTypedEventArgs(_textToType[_currentLetterIndex]));
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

        protected virtual void changeState(FontState newState)
        {
            _state = newState;

            EventHandler<StateEventArgs> handler = StateChanged;
            if (handler != null)
            {
                handler(this, new StateEventArgs(newState.GetType(), newState));
            }
        }
    }
}
