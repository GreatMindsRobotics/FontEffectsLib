using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FontEffectsLib.CoreTypes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.ComponentModel;

namespace FontEffectsLib.FontTypes
{
    public class TypingFont : GameFont, IStateful
    {
        #region Obsolete types support

        /* Implementation Note:
         * Since .NET enums are implemented with a backing numerical type, this type and FontState are interchangeable and can be safely casted to each other.
         */

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
            NotTyping = 0,

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
        /// Indicates the state of a TypingFont instance.
        /// </summary>
        public enum FontState
        {
            /// <summary>
            /// Typing has not started yet
            /// </summary>
            NotStarted = 0,
            
            /// <summary>
            /// Typing letters
            /// </summary>
            Typing = 1,

            /// <summary>
            /// Finished typing all letters.
            /// </summary>
            Finished = 2
        }

        /// <summary>
        /// Arguments for the event fired when a character is about to be typed.
        /// </summary>
        public class CharacterTypedEventArgs : CancelEventArgs
        {
            /// <summary>
            /// Gets or sets the character that will be typed by the TypingFont.
            /// </summary>
            public char TypedCharacter { get; set; }

            /// <summary>
            /// Creates instance of CharacterTypedEventArgs class.
            /// </summary>
            /// <param name="typedCharacter">The character about to be typed.</param>
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
        /// Fires whenever a single character is about to be typed. 
        /// CharacterTypedEventArgs.TypedCharacter contains the character about to be typed within the same frame.
        /// </summary>
        public event EventHandler<CharacterTypedEventArgs> CharacterTyped;

        /// <summary>
        /// Since we're overriding Text property, we need a StringBuilder to match the parent class
        /// </summary>
        private StringBuilder _textToType;

        /// <summary>
        /// Since StringBuilder is a sealed class, and does not provide a "TextChanged" event, we need to track it manually
        /// </summary>
        private bool _isTextChanged;

        /// <summary>
        /// We'll calculate the text size once; we'll re-calculate whenever _isTextChanged is true
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
                //Do we need to recalculate the size?
                if (_isTextChanged)
                {
                    _cachedTextTypeSize = _font.MeasureString(_textToType.ToString());
                    _isTextChanged = false;
                }

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
                _text.Append(_textToType.ToString());
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
            
            _textToType = new StringBuilder(textToType);
            _isTextChanged = false;

            _cachedTextTypeSize = _font.MeasureString(textToType);
            _state = FontState.NotStarted;
        }

        public override StringBuilder Text
        {
            get
            {
                //Since we have no way of knowing if the text changed via this property, we'll presume it has
                _isTextChanged = true;

                return _textToType;
            }
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
     
                        if (_currentLetterIndex >= _textToType.ToString().Length)
                        {
                            Finished(false);
                            return;
                        }

                        EventHandler<CharacterTypedEventArgs> handler = CharacterTyped;                
                        CharacterTypedEventArgs arguments = new CharacterTypedEventArgs(_textToType.ToString()[_currentLetterIndex]);
                        if (handler != null)
                        {
                            handler(this, arguments);
                        }

                        if (!arguments.Cancel)
                        {
                            _text.Append(arguments.TypedCharacter);
                        }
                        else
                        {
                            // Undo increment of the index - will retry to type later
                            _currentLetterIndex--;
                        }
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
                handler(this, new StateEventArgs(typeof(TypingFont.FontState), newState));
            }
        }
    }
}
