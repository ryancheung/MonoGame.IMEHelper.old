using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpriteFontPlus;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MonoGame.IMEHelper.Test
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        DynamicSpriteFont font1;

        IMEHandler imeHandler;
        KeyboardState lastState;
        Texture2D whitePixel;
        string inputContent = string.Empty;

        const int UnicodeSimplifiedChineseMin = 0x4E00;
        const int UnicodeSimplifiedChineseMax = 0x9FA5;
        const string DefaultChar = "?";

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreparingDeviceSettings += Graphics_PreparingDeviceSettings;

            Activated += (o, e) =>
            {
                var mode = DisplayModeHelper.DisplayModes.SupportedDisplayModes["1366x768"];
                DisplayModeHelper.ResolutionHelper.ChangeResolution(ref mode);
            };

            Deactivated += (o,e) =>
            {
                var mode = DisplayModeHelper.DisplayModes.SupportedDisplayModes["1920x1080"];
                DisplayModeHelper.ResolutionHelper.ChangeResolution(ref mode);
            };

            Exiting += (o,e) =>
            {
                var mode = DisplayModeHelper.DisplayModes.SupportedDisplayModes["1366x768"];
                DisplayModeHelper.ResolutionHelper.ChangeResolution(ref mode);
            };
        }

        private void Graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.HardwareModeSwitch = false;
            e.GraphicsDeviceInformation.PresentationParameters.IsFullScreen = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            imeHandler = IMEHandler.Create(this, true);

            imeHandler.TextInput += (s, e) =>
            {
                switch ((int)e.Character)
                {
                    case 8:
                        if (inputContent.Length > 0)
                            inputContent = inputContent.Remove(inputContent.Length - 1, 1);
                        break;
                    case 27:
                    case 13:
                        inputContent = "";
                        break;
                    default:
                        if (e.Character > UnicodeSimplifiedChineseMax)
                            inputContent += DefaultChar;
                        else
                            inputContent += e.Character;
                        break;
                }
            };

            IsMouseVisible = true;

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

            font1 = DynamicSpriteFont.FromTtf(File.ReadAllBytes(@"Content\simsun.ttf"), 20);

            whitePixel = new Texture2D(GraphicsDevice, 1, 1);
            whitePixel.SetData<Color>(new Color[] { Color.White });
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.F12) && lastState.IsKeyUp(Keys.F12))
            {
                if (imeHandler.Enabled)
                    imeHandler.StopTextComposition();
                else
                    imeHandler.StartTextComposition();
            }

            lastState = ks;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            Vector2 len = font1.MeasureString(inputContent.Trim());

            spriteBatch.DrawString(font1, "按下 F12 启用 / 停用 IME", new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(font1, inputContent, new Vector2(10, 30), Color.White);

            Vector2 drawPos = new Vector2(15 + len.X, 30);
            Vector2 measStr = new Vector2(0, font1.MeasureString("|").Y);
            Color compColor = Color.White;

            if (imeHandler.CompositionCursorPos == 0)
                spriteBatch.Draw(whitePixel, new Rectangle((int)drawPos.X, (int)drawPos.Y, 1, (int)measStr.Y), Color.White);

            for (int i = 0; i < imeHandler.Composition.Length; i++)
            {
                string val = imeHandler.Composition[i].ToString();
                switch (imeHandler.GetCompositionAttr(i))
                {
                    case CompositionAttributes.Converted: compColor = Color.LightGreen; break;
                    case CompositionAttributes.FixedConverted: compColor = Color.Gray; break;
                    case CompositionAttributes.Input: compColor = Color.Orange; break;
                    case CompositionAttributes.InputError: compColor = Color.Red; break;
                    case CompositionAttributes.TargetConverted: compColor = Color.Yellow; break;
                    case CompositionAttributes.TargetNotConverted: compColor = Color.SkyBlue; break;
                }

                if (val[0] > UnicodeSimplifiedChineseMax)
                    val = DefaultChar;

                spriteBatch.DrawString(font1, val, drawPos, compColor);

                measStr = font1.MeasureString(val);
                drawPos += new Vector2(measStr.X, 0);

                if ((i + 1) == imeHandler.CompositionCursorPos)
                    spriteBatch.Draw(whitePixel, new Rectangle((int)drawPos.X, (int)drawPos.Y, 1, (int)measStr.Y), Color.White);
            }

            for (uint i = imeHandler.CandidatesPageStart;
                i < Math.Min(imeHandler.CandidatesPageStart + imeHandler.CandidatesPageSize, imeHandler.Candidates.Length);
                i++)
            {
                if (imeHandler.Candidates[i][0] > UnicodeSimplifiedChineseMax)
                    imeHandler.Candidates[i] = DefaultChar;

                spriteBatch.DrawString(font1,
                    String.Format("{0}.{1}", i + 1 - imeHandler.CandidatesPageStart, imeHandler.Candidates[i]),
                    new Vector2(15 + len.X, 50 + (i - imeHandler.CandidatesPageStart) * 20),
                    i == imeHandler.CandidatesSelection ? Color.Yellow : Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
