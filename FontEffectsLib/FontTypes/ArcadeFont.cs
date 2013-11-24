using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FontEffectsLib.CoreTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FontEffectsLib.FontTypes
{
    /// <summary>
    /// An arcade-style font which rapidly cycles through colors.
    /// </summary>
    public class ArcadeFont : ShadowFont
    {
        protected float _colorCyclesPerSecond;
        protected List<Color> _fontColors;

        protected int currentColor = 0;

        private const float _defaultColorCyclesPerSecond = 10;
        private TimeSpan _elapsedTime;
        private TimeSpan _delayTime;

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

        public ArcadeFont(SpriteFont font, Vector2 position, params Color[] tintColors)
            : this(font, position, String.Empty, _defaultColorCyclesPerSecond, tintColors)
        {
            //Pass-through constructor
        }

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

        public override void Update(GameTime gameTime)
        {
            if (_elapsedTime >= _delayTime)
            {
                currentColor = currentColor + 1 == _fontColors.Count ? 0 : currentColor + 1;
                _tintColor = _fontColors[currentColor];

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
