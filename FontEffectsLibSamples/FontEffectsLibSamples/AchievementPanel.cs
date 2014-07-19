using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FontEffectsLib.SpriteTypes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FontEffectsLib.FontTypes;
using FontEffectsLib.CoreTypes;

namespace FontEffectsLibSamples
{
    /// <summary>
    /// Example implementation of ComplexSprite.
    /// Uses 3 sprites: An "Achievement!" title, a description text field, and a black background.
    /// Also raises events upon state change.
    /// </summary>
    public class AchievementPanel : ComplexSprite, IStateful
    {
        public delegate bool ConditionChecker(GameTime currentGameTime);

        public static bool AreFiveCoinsInsert(GameTime currentGameTime)
        {
            return FontEffectsLibSamples.CoinsInserted == 5;
        }

        public event EventHandler AchievementEarned;

        public static bool IsNoCoinsInsertedAtThirtySeconds(GameTime currentGameTime)
        {
            return FontEffectsLibSamples.CoinsInserted == 0 && currentGameTime.TotalGameTime >= TimeSpan.FromMinutes(0.5);
        }

        public static bool AreHundredDollarsDonated(GameTime currentGameTime)
        {
            return FontEffectsLibSamples.CoinsInserted == 400;
        }

        public ConditionChecker Condition;

        public AchievementPanel(float startX, SpriteFont title, SpriteFont desc, string achievementDesc, GraphicsDevice dev)
            : base(Vector2.Zero)
        {
            // Create "Achievement!" and description GameFonts
            GameFont titleObj = new GameFont(title, "Achievement!", new Vector2(5), Color.Yellow) { LayerDepth = 0.5f };
            GameFont descObj = new GameFont(desc, achievementDesc, new Vector2(2, titleObj.Size.Y + 5 + 4.5f), Color.White) { LayerDepth = 0.5f };
            Subsprites.Add(titleObj);
            Subsprites.Add(descObj);

            int achievePanelWidth = -1;
            int achievePanelHeight = -1;


            // Positioning logic for text elements
            foreach (var spr in Subsprites)
            {
                Vector2 size = Vector2.Zero;

                if (!(spr is GameFont) && !(spr is GameSprite))
                {
                    continue;
                }
                if (spr is GameSprite)
                {
                    size = new Vector2((spr as GameSprite).Texture.Width, (spr as GameSprite).Texture.Height);
                }
                else
                {
                    size = (spr as GameFont).Size;
                }

                if (size.Y + spr.Position.Y > achievePanelHeight)
                {
                    achievePanelHeight = Convert.ToInt32(Math.Ceiling(size.Y + spr.Position.Y));
                }

                if (size.X + spr.Position.X > achievePanelWidth)
                {
                    achievePanelWidth = Convert.ToInt32(Math.Ceiling(size.X + spr.Position.X));
                }
            }

            titleObj.Position = new Vector2(achievePanelWidth / 2 - (titleObj.Size.X / 2), titleObj.Position.Y);
            descObj.Position = new Vector2(achievePanelWidth / 2 - (descObj.Size.X / 2), descObj.Position.Y);

            // Create a solid color background
            Subsprites.Insert(0, GameSprite.CreateSolidColor(Vector2.Zero, achievePanelWidth + 5, achievePanelHeight + 10, Color.Black, dev));
            Subsprites[0].LayerDepth = 1;

            Position = new Vector2(startX, -achievePanelHeight);
            IsVisible = false;

            Width = achievePanelWidth;
            Height = achievePanelHeight;

            changeState(PanelState.Waiting);
        }

        public readonly int Height;
        public readonly int Width;

        protected Vector2 _velocity = Vector2.Zero;

        /// <summary>
        /// Gets or sets the velocity of this <see cref="AchievementPanel"/>, as in pixels moved per frame.
        /// </summary>
        public Vector2 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        private float _colorDeteriorationfactor = 255;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _position += _velocity;

            if (_panelState == PanelState.Waiting && Condition != null && Condition(gameTime))
            {
                IsVisible = true;
                Velocity = new Vector2(0, 0.75f);
                changeState(PanelState.Sliding);
            }

            if (_panelState == PanelState.Sliding && Position.Y > 0)
            {
                Velocity = Vector2.Zero;
                if (AchievementEarned != null)
                {
                    AchievementEarned(this, EventArgs.Empty);
                }
                changeState(PanelState.Fading);
            }

            if (_panelState == PanelState.Fading)
            {
                TintColor = new Color(TintColor.ToVector4() - (Vector4.One / _colorDeteriorationfactor));
                _colorDeteriorationfactor *= 0.98f;

                if (TintColor.R <= 0 && TintColor.G <= 0 && TintColor.B <= 0 && TintColor.A <= 0)
                {
                    IsVisible = false;
                    TintColor = Color.White;
                    changeState(PanelState.Done);
                }
            }


        }

        private PanelState _panelState = PanelState.Waiting;

        public enum PanelState
        {
            Waiting,
            Sliding,
            Fading,
            Done
        }

        private void changeState(PanelState newState)
        {
            if (newState == _panelState)
            {
                return;
            }

            EventHandler<StateEventArgs> handler = StateChanged;
            if (handler != null)
            {
                handler(this, new StateEventArgs(typeof(AchievementPanel.PanelState), newState));
            }
            _panelState = newState;
        }

        public event EventHandler<StateEventArgs> StateChanged;
    }
}
