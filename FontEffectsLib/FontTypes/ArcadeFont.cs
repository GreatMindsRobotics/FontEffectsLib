using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FontEffectsLib.CoreTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;

namespace FontEffectsLib.FontTypes
{
    /// <summary>
    /// An arcade-style font which rapidly cycles through colors.
    /// </summary>
    public class ArcadeFont : ShadowFont
    {
        /// <summary>
        /// The number of times to cycle through <see cref="FontColors"/> in one second.
        /// </summary>
        protected float _colorCyclesPerSecond;


        //Private: When would the subclass ever need to set this variable?
        private List<Color> _fontColors;

        /// <summary>
        /// The index of the current color in <see cref="FontColors"/>.
        /// </summary>
        protected int currentColor = 0;

        private const float _defaultColorCyclesPerSecond = 10;
        private TimeSpan _elapsedTime;
        private TimeSpan _delayTime;

        /// <summary>
        /// Gets or sets the number of times to cycle through <see cref="FontColors"/> in one second.
        /// </summary>
        public float ColorCyclesPerSecond
        {
            get { return _colorCyclesPerSecond; }
            set 
            { 
                _colorCyclesPerSecond = value;
                _delayTime = TimeSpan.FromMilliseconds(1000 / _colorCyclesPerSecond / _fontColors.Count);
            }
        }

        /// <summary>
        /// Gets the list of colors for the font to cycle through.
        /// </summary>
        public List<Color> FontColors
        {
            get { return _fontColors; }
        }

        /// <summary>
        /// Create a new instance of <see cref="ArcadeFont"/>.
        /// </summary>
        /// <param name="font">The font to render.</param>
        /// <param name="position">The position of the font.</param>
        /// <param name="tintColors">The colors to cycle through.</param>
        public ArcadeFont(SpriteFont font, Vector2 position, params Color[] tintColors)
            : this(font, position, String.Empty, _defaultColorCyclesPerSecond, tintColors)
        {
            //Pass-through constructor
        }

        /// <summary>
        /// Arguments for the event fired when the arcade font color is about to cycle.
        /// </summary>
        public class ColorChangedEventArgs : CancelEventArgs
        {
            /// <summary>
            /// Gets the color which the ArcadeFont will change to.
            /// </summary>
            public Color NewColor { get; private set; }

            /// <summary>
            /// Creates instance of ColorChangedEventArgs class.
            /// </summary>
            /// <param name="newColor">The color about to be changed to.</param>
            public ColorChangedEventArgs(Color newColor)
            {
                NewColor = newColor;
            }
        }

        /// <summary>
        /// Fires whenever the color of the arcade font is about to be cycled through.
        /// </summary>
        public event EventHandler<ColorChangedEventArgs> ColorChanged;

        /// <summary>
        /// Create a new instance of <see cref="ArcadeFont"/>.
        /// </summary>
        /// <param name="font">The font to render.</param>
        /// <param name="position">The position of the font.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="colorChangeDelay">The number of times to cycle through <see cref="FontColors"/> in one second.</param>
        /// <param name="tintColors">The colors to cycle through.</param>
        public ArcadeFont(SpriteFont font, Vector2 position, String text, float colorChangeDelay, params Color[] tintColors)
            : base(font, position, Color.White)
        {
            //Color.White = null check for color array, color is set later

            if (text == null)
            {
                //Null string = assume empty string, should I throw ArgumentNullException instead?
                text = String.Empty;
            }

            if (tintColors == null)
            {
                throw new ArgumentNullException("tintColors");
            }

            if (tintColors.Length < 1)
            {
                throw new ArgumentException("The tintColors array must contain at least one element.");
            }

            _text = new StringBuilder(text);

            _colorCyclesPerSecond = colorChangeDelay;
            _elapsedTime = TimeSpan.Zero;
            _fontColors = tintColors.ToList();

            _delayTime = TimeSpan.FromMilliseconds(1000 / colorChangeDelay / tintColors.Length);

            _tintColor = tintColors[0];
        }

        /// <summary>
        /// Update this <see cref="ArcadeFont"/>.
        /// </summary>
        /// <param name="gameTime">The current <see cref="GameTime"/>, representing the time elapsed over the course of the game.</param>
        public override void Update(GameTime gameTime)
        {
            if (!_isVisible)
            {
                return;
            }

            if (_elapsedTime >= _delayTime)
            {
                int newColor = currentColor + 1 == _fontColors.Count ? 0 : currentColor + 1;
                Color newColorVal = _fontColors[newColor];

                EventHandler<ColorChangedEventArgs> handler = ColorChanged;
                ColorChangedEventArgs arguments = new ColorChangedEventArgs(newColorVal);
                if (handler != null)
                {
                    handler(this, arguments);
                }

                if (!arguments.Cancel)
                {
                    currentColor = newColor;
                    _tintColor = newColorVal;
                }

                _elapsedTime = TimeSpan.Zero;
            }
            else
            {
                _elapsedTime += gameTime.ElapsedGameTime;
            }

            base.Update(gameTime);
        }

    }
}
