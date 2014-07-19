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
using FontEffectsLib.SpriteTypes;

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
        ArcadeFont insertCoins;
        AccelDropInFont by;

        GameSprite bgSprite;

        SoundEffect crashEffect;
        SoundEffect coinsEffect;

        KeyboardState lastKeyboardState;
        KeyboardState currentKeyboardState;

        StatefulSequence<SlidingFont> slidingText;
        TexturePanel coolPanel;

        AchievementPanel insertFive;
        AchievementPanel donateHundred;
        AchievementPanel noCoinSpam;
        AchievementPanel allAchievementsDone;

        static bool standardAchievementsCompleted = false;

        MouseState currentMS;

        TypingFont colorfulTypingText;
        TypingFont regularTypingText;

        TaskSchedulerComponent taskScheduler;

        Random random;

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
            CoinsInserted = 0;

            taskScheduler = new TaskSchedulerComponent(this);
            Components.Add(taskScheduler);

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

            bgSprite = new GameSprite(Content.Load<Texture2D>("bgImage"), Vector2.Zero, Color.White);
            bgSprite.ScaleToViewport(GraphicsDevice.Viewport);

            titleText1 = new DropInFont(Content.Load<SpriteFont>("GameFont"), new Vector2(viewport.HalfWidth(), -1000), screenSize / 2, new Vector2(0, 20), "Great Minds", Color.OrangeRed);
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

            Vector2 byPos = new Vector2(titleText1.Position.X - 100, -1000);
            by = new AccelDropInFont(Content.Load<SpriteFont>("GameFont"), byPos, screenSize / 2, new Vector2(0, 5), "By:", Color.Orange, new Vector2(0, 1.5f));
            by.TargetPosition = titleText1.TargetPosition - new Vector2(100, 100);
            by.SetCenterAsOrigin();
            by.ShadowPosition = new Vector2(by.Position.X - 4, by.Position.Y + 4);
            by.ShadowColor = Color.Black;
            by.MaxDropSpeed = new Vector2(15, 15);

            crashEffect = Content.Load<SoundEffect>("Crash");

            //Obtained from http://www.freesound.org/people/EdgardEdition/sounds/113095/
            coinsEffect = Content.Load<SoundEffect>("insertcoin");

            insertCoins = new ArcadeFont(Content.Load<SpriteFont>("ArcadeFont"), new Vector2(300, 400), Color.Red, Color.White, Color.Yellow, Color.Blue);
            insertCoins.ShadowPosition = new Vector2(insertCoins.Position.X - 1, insertCoins.Position.Y + 1);
            insertCoins.ColorCyclesPerSecond = 15;  //Default is 10; smaller number blinks slower
            insertCoins.Text.Append("INSERT COINS");
            insertCoins.IsVisible = false;

            slidingText = new StatefulSequence<SlidingFont>(SlidingFont.FontState.Done, typeof(SlidingFont.FontState));

            Vector2 targetPos = new Vector2(505, 400);
            foreach (char letter in "COOL")
            {
                slidingText.Add(new SlidingFont(Content.Load<SpriteFont>("SlidingFont"), new Vector2(50, 350), targetPos, 2f, letter.ToString(), Color.Red) { EnableShadow = false, IsVisible = false, TargetTolerance = 0.625f });
                targetPos.X += slidingText[slidingText.Count - 1].Size.X; //Magic # - bad idea
            }

            targetPos = new Vector2(505, 420);
            random = new Random();

            foreach (char letter in "Effects!")
            {
                slidingText.Add(new SlidingFont(Content.Load<SpriteFont>("SlidingFont"), new Vector2(random.Next(0, GraphicsDevice.Viewport.Width), random.Next(0, GraphicsDevice.Viewport.Height)), targetPos, 1f + (float)random.NextDouble(), letter.ToString(), Color.Red) { EnableShadow = false, IsVisible = false });
                targetPos.X += slidingText[slidingText.Count-1].Size.X; //Magic # - bad idea
            }

            coolPanel = new TexturePanel(Content.Load<Texture2D>("WavyEffect"), new Vector2(120, 60), Vector2.One * .5f, new Vector2(550, 425), new Color(60, 60, 60, 128));
            coolPanel.IsVisible = false;

            insertFive = new AchievementPanel(0, Content.Load<SpriteFont>("SlidingFont"), Content.Load<SpriteFont>("ArcadeFont"), "Insert 5 coins", GraphicsDevice);
            insertFive.Condition = AchievementPanel.AreFiveCoinsInsert;

            donateHundred = new AchievementPanel(0, Content.Load<SpriteFont>("SlidingFont"), Content.Load<SpriteFont>("ArcadeFont"), "Donate $100 to GMR", GraphicsDevice);
            donateHundred.Condition = AchievementPanel.AreHundredDollarsDonated;

            noCoinSpam = new AchievementPanel(0, Content.Load<SpriteFont>("SlidingFont"), Content.Load<SpriteFont>("ArcadeFont"), "Don't insert coins for 30 seconds", GraphicsDevice);
            noCoinSpam.Position = new Vector2(GraphicsDevice.Viewport.Width - noCoinSpam.Width, noCoinSpam.Position.Y);
            noCoinSpam.Condition = AchievementPanel.IsNoCoinsInsertedAtThirtySeconds;

            allAchievementsDone = new AchievementPanel(0, Content.Load<SpriteFont>("SlidingFont"), Content.Load<SpriteFont>("ArcadeFont"), "Finish all achievements", GraphicsDevice);
            allAchievementsDone.Condition = delegate(GameTime gt) { return standardAchievementsCompleted; };

            StatefulSequence<AchievementPanel> allAchievementPanels = new StatefulSequence<AchievementPanel>(AchievementPanel.PanelState.Done, typeof(AchievementPanel.PanelState));
            allAchievementPanels.Add(noCoinSpam);
            allAchievementPanels.Add(donateHundred);
            allAchievementPanels.Add(insertFive);

            allAchievementPanels.SequenceReachedMonitoredState += new StatefulSequence<AchievementPanel>.MonitoredStateReached(allAchievementPanels_SequenceReachedMonitoredState);

            slidingText.SequenceReachedMonitoredState += new StatefulSequence<SlidingFont>.MonitoredStateReached(slidingText_SequenceReachedMonitoredState);

            colorfulTypingText = new TypingFont(Content.Load<SpriteFont>("ArcadeFont"), new Vector2(200, 0), Color.Black, "Typing text here", TimeSpan.FromMilliseconds(200));
            colorfulTypingText.StateChanged += new EventHandler<StateEventArgs>(typingText_StateChanged);
            colorfulTypingText.CharacterTyped += new EventHandler<TypingFont.CharacterTypedEventArgs>(typingText_CharacterTyped);
            colorfulTypingText.Tag = "1";
            colorfulTypingText.Start();

            regularTypingText = new TypingFont(Content.Load<SpriteFont>("ArcadeFont"), new Vector2(400, 0), Color.Black, "... and more typing text here...", TimeSpan.FromMilliseconds(50));
            regularTypingText.StateChanged += new EventHandler<StateEventArgs>(typingText_StateChanged);
            regularTypingText.Tag = "2";
        }


        void allAchievementPanels_SequenceReachedMonitoredState()
        {
            taskScheduler.ScheduleFutureTask(TimeSpan.FromSeconds(3), () => standardAchievementsCompleted = true);
        }

        void typingText_StateChanged(object sender, StateEventArgs e)
        {
            if ((TypingFont.FontState)e.Data == TypingFont.FontState.Finished)
            {
                TypingFont typingFont = (TypingFont)sender;
                if (typingFont.Tag.ToString() == "1")
                {
                    regularTypingText.Reset();
                    regularTypingText.DelayTime = TimeSpan.FromMilliseconds(random.Next(50, 200));
                    regularTypingText.Start();
                }
                else
                {
                    colorfulTypingText.Reset();
                    colorfulTypingText.DelayTime = TimeSpan.FromMilliseconds(random.Next(50, 200));
                    colorfulTypingText.Start();
                }
            }
        }

        Color[] typingTextColors = new Color[] { Color.Black, Color.Red, Color.Green, Color.Blue, Color.White, Color.HotPink };
        int colorIndex = 0;

        void typingText_CharacterTyped(object sender, TypingFont.CharacterTypedEventArgs e)
        {
            colorfulTypingText.TintColor = typingTextColors[colorIndex];
            colorIndex++;
            colorIndex %= typingTextColors.Length;
        }


        void slidingText_SequenceReachedMonitoredState()
        {
            coolPanel.SetTextureWithBackground(Content.Load<Texture2D>("WavyEffect1"), true);

            //Alex's awesome reset function
            titleText1.Reset();
        }

        void titleText2_StateChanged(object sender, StateEventArgs e)
        {
            if (e.DataType == typeof(FadingFont.FontState))
            {   
                if ((FadingFont.FontState)e.Data == FadingFont.FontState.TargetValueReached)
                {
                    coolPanel.Expand();

                    foreach (SlidingFont slidingFont in slidingText)
                    {
                        slidingFont.Slide();
                    }
                    insertCoins.IsVisible = true;
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

        internal static int CoinsInserted;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            currentKeyboardState = Keyboard.GetState();
            currentMS = Mouse.GetState();
            titleText1.Update(gameTime);
            titleText2.Update(gameTime);
            titleText3.Update(gameTime);
            colorfulTypingText.Update(gameTime);
            regularTypingText.Update(gameTime);

            by.Update(gameTime);

            foreach (SlidingFont slidingFont in slidingText)
            {
                slidingFont.Update(gameTime);
            }

            insertCoins.Update(gameTime);
            
            //If we can insert a coin and we in fact did
            if (insertCoins.IsVisible && ((currentKeyboardState.IsKeyDown(Keys.Space) && lastKeyboardState.IsKeyUp(Keys.Space)) || (currentKeyboardState.IsKeyDown(Keys.Enter) && lastKeyboardState.IsKeyUp(Keys.Enter))))
            {
                coinsEffect.Play();
                CoinsInserted++;
                

                //Alex's awesome reset
                foreach (SlidingFont slidingFont in slidingText)
                {
                    slidingFont.Reset(true);
                }
            }

            coolPanel.Update(gameTime);
            insertFive.Update(gameTime);
            donateHundred.Update(gameTime);
            noCoinSpam.Update(gameTime);
            allAchievementsDone.Update(gameTime);
            base.Update(gameTime);

            lastKeyboardState = currentKeyboardState;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Turquoise);

            spriteBatch.Begin();

            

            bgSprite.Draw(spriteBatch);

            titleText2.Draw(spriteBatch);
            titleText1.Draw(spriteBatch);
            titleText3.Draw(spriteBatch);
            
            
            by.Draw(spriteBatch);

            coolPanel.Draw(spriteBatch);
            spriteBatch.DrawString(titleText1.Font, string.Format("X: {0} Y: {1}", currentMS.X, currentMS.Y), Vector2.Zero, Color.White);
            foreach (SlidingFont slidingFont in slidingText)
            {
                slidingFont.Draw(spriteBatch);
            }

            insertCoins.Draw(spriteBatch);

            insertFive.Draw(spriteBatch);
            donateHundred.Draw(spriteBatch);
            noCoinSpam.Draw(spriteBatch);
            colorfulTypingText.Draw(spriteBatch);
            regularTypingText.Draw(spriteBatch);
            allAchievementsDone.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
