using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FontEffectsLib.CoreTypes;

namespace FontEffectsLib.SpriteTypes
{
    public class Panel : GameSprite, IStateful
    {
        public event EventHandler<StateEventArgs> StateChanged;

        public enum PanelState
        {
            Collapsed,
            ExpandingHorizontally,
            ExpandingVertically,
            Open,
            CollapsingVertically,
            CollapsingHorizontally
        }

        protected PanelState _state;
        protected Vector2 _speed;
        protected Vector2 _minScale;

        /// <summary>
        /// Creates an expandable/collapsable panel
        /// </summary>
        /// <param name="graphics">Graphics device</param>
        /// <param name="size">Size of panel when fully expanded</param>
        /// <param name="speed">Speed of expansion/collapse</param>
        /// <param name="position">Position of center of the panel. Panel expands/collapses from/to this point</param>
        /// <param name="color">Color of the panel</param>
        public Panel(GraphicsDevice graphics, Vector2 size, Vector2 speed, Vector2 position, Color color)
            : base(new Texture2D(graphics, (int)size.X, (int)size.Y), position, color)
        {
            _state = PanelState.Collapsed;
            _speed = speed;
            _minScale = new Vector2(1f / size.X, 1f / size.Y);

            _scale = _minScale;
            _isVisible = false;

            //Set texture data
            _texture.SetData<Color>(Enumerable.Repeat<Color>(Color.White, (int)size.X * (int)size.Y).ToArray<Color>());

            SetCenterAsOrigin();
        }

        public override void Update(GameTime gameTime)
        {
            if (!_isVisible)
            {
                return;
            }

            switch (_state)
            {
                case PanelState.ExpandingHorizontally:
                    if (Scale.X < 1f)
                    {
                        _scale.X = MathHelper.Clamp(_scale.X + (float)gameTime.ElapsedGameTime.TotalSeconds * _speed.X, _minScale.X, 1f);
                    }
                    else
                    {
                        switchState(PanelState.ExpandingVertically);
                    }
                    break;

                case PanelState.ExpandingVertically:
                    if (Scale.Y < 1.0f)
                    {
                        _scale.Y = MathHelper.Clamp(Scale.Y + (float)gameTime.ElapsedGameTime.TotalSeconds * _speed.Y, _minScale.Y, 1f);
                    }
                    else
                    {
                        switchState(PanelState.Open);
                    }
                    break;

                case PanelState.CollapsingHorizontally:
                    if (Scale.X > _minScale.X)
                    {
                        _scale.X = MathHelper.Clamp(Scale.X - (float)gameTime.ElapsedGameTime.TotalSeconds * _speed.X, _minScale.X, 1f);
                    }
                    else
                    {
                        _isVisible = false;
                        switchState(PanelState.Collapsed);
                    }
                    break;

                case PanelState.CollapsingVertically:
                    if (Scale.Y > _minScale.Y)
                    {
                        _scale.Y = MathHelper.Clamp(_scale.Y - (float)gameTime.ElapsedGameTime.TotalSeconds * _speed.Y, _minScale.Y, 1f);
                    }
                    else
                    {
                        switchState(PanelState.CollapsingHorizontally);
                    }
                    break;
            }
        }

        /// <summary>
        /// Expand the panel. The panel must be in Collapsed or Collapsing state.
        /// </summary>
        /// <returns>True if can succcessfully start expanding; false otherwise.</returns>
        public virtual bool Expand()
        {
            if (_state == PanelState.Collapsed || _state == PanelState.CollapsingHorizontally || _state == PanelState.CollapsingVertically)
            {
                _isVisible = true;
                PanelState newState = _state == PanelState.CollapsingVertically ? PanelState.ExpandingVertically : PanelState.ExpandingHorizontally;
                switchState(newState);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Collapse the panel. Panel must be in Open or Expanding states.
        /// </summary>
        /// <returns>True if can successfully start collapsing; false otherwise.</returns>
        public virtual bool Collapse()
        {
            if (_state == PanelState.Open || _state == PanelState.ExpandingHorizontally || _state == PanelState.ExpandingVertically)
            {
                PanelState newSate = _state == PanelState.ExpandingHorizontally ? PanelState.CollapsingHorizontally : PanelState.CollapsingVertically;
                switchState(newSate);
                return true;
            }

            return false;
        }

        protected virtual void switchState(PanelState panelState)
        {
            _state = panelState;
            
            if (StateChanged != null)
            {
                StateChanged(this, new StateEventArgs(typeof(PanelState), panelState));
            }
        }
    }
}
