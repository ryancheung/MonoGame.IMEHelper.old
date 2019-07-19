using Microsoft.Xna.Framework;
using System;

namespace MonoGame.IMEHelper
{

    /// <summary>
    /// Integrate IME to DesktopGL(SDL2) platform.
    /// </summary>
    internal class SdlIMEHandler : IMEHandler
    {
        public SdlIMEHandler(Game game, bool showDefaultIMEWindow = false) : base(game, showDefaultIMEWindow)
        {
        }

        public override bool Enabled { get; protected set; }

        public override void PlatformInitialize()
        {
            GameInstance.Window.TextInput += Window_TextInput;
        }

        private void Window_TextInput(object sender, TextInputEventArgs e)
        {
            OnTextInput(e);
        }

        public override void StartTextComposition()
        {
            Sdl.StartTextInput();
        }

        public override void StopTextComposition()
        {
            Sdl.StopTextInput();
        }

        public override void SetTextInputRect(ref Rectangle rect)
        {
            var sdlRect = new Sdl.Rectangle() { X = rect.X, Y = rect.Y, Width = rect.Width, Height = rect.Height };
            Sdl.SetTextInputRect(ref sdlRect);
        }
    }
}
