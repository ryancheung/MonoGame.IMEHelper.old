﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.IMEHelper
{
    /// <summary>
    /// Helper class for getting correct mouse / keyboard state
    /// </summary>
    public class InputRemapper : GameComponent
    {
        /// <summary>
        /// Current Mouse State
        /// </summary>
        public MouseState MouseState { get; private set; }

        /// <summary>
        /// Current Keyboard State
        /// </summary>
        public KeyboardState KeyboardState { get; private set; }

        internal InputRemapper(Game game) : base(game)
        {
            game.Components.Add(this);
        }

        /// <summary>
        /// Called when the game updates.
        /// </summary>
        /// <param name="gameTime">Game time snapshot</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            MouseState ms = Mouse.GetState();

            MouseState = new MouseState(
                ms.X,
                ms.Y,
                // Mouse wheel is not correct if the IME helper is used, thus it is needed to remap here.
                CharMessageFilter.Added ? CharMessageFilter.MouseWheel : ms.ScrollWheelValue,
                ms.LeftButton,
                ms.MiddleButton,
                ms.RightButton,
                ms.XButton1,
                ms.XButton2);
            KeyboardState = Keyboard.GetState();
            // TODO: I don't know if there has any input events need to remap, please add them if it is neccessary.
        }

    }
}
