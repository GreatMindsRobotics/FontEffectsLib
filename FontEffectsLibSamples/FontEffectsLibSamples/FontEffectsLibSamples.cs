using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FontEffectsLib.CoreTypes;
using FontEffectsLib.FontTypes;

namespace FontEffectsLibSamples
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class FontEffectsLibSamples : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Viewport viewport;
        Vector2 screenSize;

        DropInFont titleText1;
        FadingFont titleText2;
        DropInFont titleText3;

        SoundEffect crashEffect;

        public FontEffectsLibSamples()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;
            
            viewport = GraphicsDevice.Viewport;
            screenSize = new Vector2(viewport.Width, viewport.Height);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            titleText1 = new DropInFont(Content.Load<SpriteFont>("GameFont"), new Vector2(viewport.HalfWidth(), -400), screenSize / 2, new Vector2(0, 20), "Great Minds", Color.OrangeRed);
            titleText1.TargetPosition = new Vector2(titleText1.TargetPosition.X, titleText1.TargetPosition.Y - titleText1.Size.Y / 4);
            titleText1.SetCenterAsOrigin();
            titleText1.ShadowPosition = new Vector2(titleText1.Position.X - 4, titleText1.Position.Y + 4);
            titleText1.ShadowColor = Color.Black;
            titleText1.StateChanged += new EventHandler<StateEventArgs>(statefulFont_StateChanged);

            Vector2 titleText2Pos = new Vector2(titleText1.Position.X + titleText1.Size.X / 2, viewport.HalfHeight() + titleText1.Size.Y / 4);
            titleText2 = new FadingFont(Content.Load<SpriteFont>("GameFont"), titleText2Pos, Color.Orange);
            titleText2.Text.Append("Robotics");
            titleText2.FadeStep /= 5f;
            titleText2.StateChanged += new EventHandler<StateEventArgs>(titleText2_StateChanged);
            titleText2.Fade = false;
            titleText2.Position -= new Vector2(titleText2.Size.X / 2, 0);
            titleText2.ShadowPosition -= new Vector2(titleText2.Size.X / 2, 0);
            titleText2.SetCenterAsOrigin();

            titleText1.Text.Clear();
            titleText1.Text.Append("Great ");

            Vector2 titleText3Pos = new Vector2(titleText1.Position.X + titleText1.Size.X / 2, -2000);
            titleText3 = new DropInFont(Content.Load<SpriteFont>("GameFont"), titleText3Pos, screenSize / 2, new Vector2(0, 20), "Minds", Color.OrangeRed);
            titleText3.TargetPosition = titleText1.TargetPosition;
            titleText3.SetCenterAsOrigin();
            titleText3.ShadowPosition = new Vector2(titleText3.Position.X - 4, titleText3.Position.Y + 4);
            titleText3.ShadowColor = Color.Black;
            titleText3.StateChanged += new EventHandler<StateEventArgs>(statefulFont_StateChanged);
            titleText3.Tag = GameTags.StartTitleFadeIn;

            crashEffect = Content.Load<SoundEffect>("Crash");            
        }

        void titleText2_StateChanged(object sender, StateEventArgs e)
        {
            if (e.DataType == typeof(FadingFont.FontState))
            {   
                if ((FadingFont.FontState)e.Data == FadingFont.FontState.TargetValueReached)
                {
                    //TODO: Transition to next screen
                    Exit();
                }
            }
        }

        void statefulFont_StateChanged(object sender, StateEventArgs e)
        {
            if (e.DataType == typeof(DropInFont.FontState))
            {
                if ((DropInFont.FontState)e.Data == DropInFont.FontState.Compress)
                {
                    crashEffect.Play();
                }

                BaseGameObject gameObject = (BaseGameObject)sender;
                if (gameObject.Tag != null && (GameTags)gameObject.Tag == GameTags.StartTitleFadeIn)
                {
                    titleText2.Fade = true;
                }
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            titleText1.Update(gameTime);
            titleText2.Update(gameTime);
            titleText3.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Turquoise);

            spriteBatch.Begin();

            titleText2.Draw(spriteBatch);
            titleText1.Draw(spriteBatch);
            titleText3.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
