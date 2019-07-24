using CoreGraphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using UIKit;

namespace MonoGame.IMEHelper
{
    internal class IosIMEHandler : IMEHandler
    {
        private UIWindow mainWindow;
        private UIViewController gameViewController;

        private UITextField textField;

        private int virtualKeyboardHeight;

        public IosIMEHandler(Game game, bool showDefaultIMEWindow = false) : base(game, showDefaultIMEWindow)
        {
        }

        public override bool Enabled { get; protected set; }

        private void TextField_ValueChanged(object sender, EventArgs e)
        {
            foreach (var c in textField.Text)
                OnTextInput(new TextInputEventArgs(c, KeyboardUtil.ToXna(c)));

            textField.Text = string.Empty;
        }

        public override void PlatformInitialize()
        {
            mainWindow = GameInstance.Services.GetService<UIWindow>();
            gameViewController = GameInstance.Services.GetService<UIViewController>();

            textField = new UITextField(new CGRect(0, -400, 200, 40));
            textField.KeyboardType = UIKeyboardType.Default;
            textField.ReturnKeyType = UIReturnKeyType.Done;
            textField.EditingChanged += TextField_ValueChanged;

            gameViewController.Add(textField);

            UIKeyboard.Notifications.ObserveWillShow((s, e) =>
            {
                virtualKeyboardHeight = (int)e.FrameBegin.Height;
            });
        }

        public override void StartTextComposition()
        {
            if (Enabled)
                return;

            textField.BecomeFirstResponder();
            Enabled = true;
        }

        public override void StopTextComposition()
        {
            if (!Enabled)
                return;

            textField.Text = string.Empty;
            textField.ResignFirstResponder();
            Enabled = false;
        }

        public override int VirtualKeyboardHeight => virtualKeyboardHeight;

        const int KeyboardHideOffset = 80;

        public override void Update(GameTime gameTime)
        {
            TouchCollection touchCollection = TouchPanel.GetState();
            foreach (TouchLocation touchLocation in touchCollection)
            {
                if (TouchLocationState.Pressed == touchLocation.State)
                {
                    if (touchLocation.Position.Y < ((mainWindow.GetFrame().Y - virtualKeyboardHeight) - KeyboardHideOffset))
                        StopTextComposition();
                }
            }
        }
    }
}
