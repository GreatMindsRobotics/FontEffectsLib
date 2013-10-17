using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FontEffectsLib.CoreTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FontEffectsLib.FontTypes
{
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

        public List<Color> FontColors
        {
            get { return _fontColors; }
            set { _fontColors = value; }
        }

        public ArcadeFont(SpriteFont font, Vector2 position, List<Color> tintColors)
            : this(font, position, String.Empty, tintColors, _defaultColorCyclesPerSecond)
        {
            //Pass-through constructor
        }

        public ArcadeFont(SpriteFont font, Vector2 position, String text, List<Color> tintColors, float colorChangeDelay)
            : base(font, position, tintColors[0])
        {
            _text = new StringBuilder(text);

            _colorCyclesPerSecond = colorChangeDelay;
            _elapsedTime = TimeSpan.Zero;
            _fontColors = tintColors;

            _delayTime = TimeSpan.FromMilliseconds(1000 / colorChangeDelay / tintColors.Count);
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
