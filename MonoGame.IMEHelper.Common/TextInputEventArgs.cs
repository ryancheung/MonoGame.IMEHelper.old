using Microsoft.Xna.Framework.Input;
using System;

namespace MonoGame.IMEHelper
{
    public class TextInputEventArgs : EventArgs
    {
#if WINDOWSDX || DESKTOPGL
        public static TextInputEventArgs FromSDLEvent(Microsoft.Xna.Framework.TextInputEventArgs args)
        {
            return new TextInputEventArgs(args.Character, args.Key);
        }
#endif

        public TextInputEventArgs(char character, Keys key = Keys.None)
        {
            Character = character;
            Key = key;
        }

        public char Character { get; }
        public Keys Key { get; }
    }

}
