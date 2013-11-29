using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FontEffectsLib.CoreTypes;

namespace FontEffectsLib.FontTypes
{
    public class FadingFont : ShadowFont, IStateful
    {
        public event EventHandler<StateEventArgs> StateChanged;

        public enum FadingType
        {
            In,
            Out
        }

        public enum FontState
        {
            NotFading,
            Fading,
            TargetValueReached
        }

        protected FadingType _fadeType;
        protected FontState _state;
        protected float _fadeStep;
        protected float _startingAlpha;
        protected float _targetAlpha;
        protected float _alpha;

        protected Color _targetTintColor;
        protected Color _targetShadowColor;

        protected Color _startingTintColor;
        protected Color _startingShadowColor;

        public virtual FadingType FadeType
        {
            get { return _fadeType; }
            set { _fadeType = value; }
        }

        public virtual bool Fade
        {
            get { return _state == FontState.NotFading; }
            set { changeState(value ? FontState.Fading : FontState.NotFading); }
        }

        public virtual float FadeStep
        {
            get { return _fadeStep; }
            set { _fadeStep = value; }
        }

        public virtual float StartingAlpha
        {
            get { return _startingAlpha; }
            set { _startingAlpha = value; }
        }

        public virtual float TargetAlpha
        {
            get { return _targetAlpha; }
            set { _targetAlpha = value; }
        }

        public virtual float Alpha
        {
            get { return _alpha; }
        }

        public FadingFont(SpriteFont font, Vector2 position, Color tintColor)
            : this(font, position, 0f, 1f, 0.01f, 1f, String.Empty, tintColor, true)
        {
            //Pass-through constructor
        }

        public FadingFont(SpriteFont font, Vector2 position, float startingAlpha, float targetAlpha, float fadeStep, float brightness, String text, Color tintColor, bool startFading)
            : this(font, position, startingAlpha, targetAlpha, fadeStep, brightness, text, tintColor, new Vector2(position.X - _defaultShadowSize, position.Y + _defaultShadowSize), Color.Black, FadingType.In, startFading)
        {
            //Pass-through constructor
        }


        public FadingFont(SpriteFont font, Vector2 position, float startingAlpha, float targetAlpha, float fadeStep, float brightness, String text, Color tintColor, Vector2 shadowPosition, Color shadowColor, FadingType fadingType, bool startFading)
            : base(font, text, position, tintColor, shadowPosition, shadowColor)
        {
            _startingAlpha = startingAlpha;
            _targetAlpha = targetAlpha;
            _fadeStep = fadeStep;
            _fadeType = fadingType;

            _startingTintColor = tintColor;
            _startingShadowColor = shadowColor;

            Reset();
        }

        public void Reset()
        {
            _state = FontState.Fading;
            _alpha = _startingAlpha;

            if (_fadeType == FadingType.In)
            {
                _targetTintColor = _startingTintColor;
                _tintColor = Color.Transparent;

                _targetShadowColor = _startingShadowColor;
                _shadowColor = Color.Transparent;
            }
            else
            {
                _targetTintColor = Color.Transparent;
                _targetShadowColor = Color.Transparent;
            }        
        }

        public override void Update(GameTime gameTime)
        {
            if (!_isVisible)
            {
                return;
            }

            switch (_state)
            {
                case FontState.NotFading:
                    return;

                case FontState.Fading:
                    if (_fadeType == FadingType.In)
                    {
                        if (_alpha < _targetAlpha)
                        {
                            _alpha += _fadeStep;
                        
                            adjustColor(ref _tintColor, _targetTintColor, _alpha);
                            adjustColor(ref _shadowColor, _targetShadowColor, _alpha);
                        }
                        else
                        {
                            changeState(FontState.TargetValueReached);
                            return;
                        }
                    }
                    else
                    {
                        if (_alpha > _targetAlpha)
                        {
                            _alpha -= _fadeStep;

                            _tintColor *= _alpha;
                            _shadowColor *= _alpha;
                        }
                        else
                        {
                            changeState(FontState.TargetValueReached);
                            return;
                        }
                    }

                    break;

                case FontState.TargetValueReached:
                    changeState(FontState.NotFading);
                    return;
            }

        }

        private void adjustColor(ref Color adjustMe, Color target, float alpha)
        {
            adjustMe.R = (byte)MathHelper.Clamp(target.R * alpha, 0, target.R);
            adjustMe.G = (byte)MathHelper.Clamp(target.G * alpha, 0, target.G);
            adjustMe.B = (byte)MathHelper.Clamp(target.B * alpha, 0, target.B);
            adjustMe.A = (byte)MathHelper.Clamp(target.A * alpha, 0, target.A);
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
