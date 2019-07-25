using CoreGraphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using UIKit;

namespace MonoGame.IMEHelper
{
    internal class UIBackwardsTextField : UITextField
    {
        // A delegate type for hooking up change notifications.
        public delegate void DeleteBackwardEventHandler(object sender, EventArgs e);

        // An event that clients can use to be notified whenever the
        // elements of the list change.
        public event DeleteBackwardEventHandler OnDeleteBackward;

        public UIBackwardsTextField(CGRect rect) : base(rect)
        {
        }

        public void OnDeleteBackwardPressed()
        {
            if (OnDeleteBackward != null)
            {
                OnDeleteBackward(null, null);
            }
        }

        public UIBackwardsTextField()
        {
            BorderStyle = UITextBorderStyle.RoundedRect;
            ClipsToBounds = true;
        }

        public override void DeleteBackward()
        {
            base.DeleteBackward();
            OnDeleteBackwardPressed();
        }
    }

    internal class IosIMEHandler : IMEHandler
    {
        private UIWindow mainWindow;
        private UIViewController gameViewController;

        private UIBackwardsTextField textField;

        private int virtualKeyboardHeight;

        public IosIMEHandler(Game game, bool showDefaultIMEWindow = false) : base(game, showDefaultIMEWindow)
        {
        }

        public override bool Enabled { get; protected set; }

        private void TextField_ValueChanged(object sender, EventArgs e)
        {
            foreach (var c in textField.Text)
            {
                var nc = c;
                if (nc == 8198) // Handle invalid character
                    nc = ' ';

                OnTextInput(new TextInputEventArgs(nc, KeyboardUtil.ToXna(nc)));
            }

            textField.Text = string.Empty;
        }

        private bool TextField_ShouldReturn(UITextField textfield)
        {
            // Not found, so remove keyboard.
            textfield.ResignFirstResponder();

            // We do not want UITextField to insert line-breaks.
            return false;
        }

        public override void PlatformInitialize()
        {
            mainWindow = GameInstance.Services.GetService<UIWindow>();
            gameViewController = GameInstance.Services.GetService<UIViewController>();

            textField = new UIBackwardsTextField(new CGRect(0, -400, 200, 40));
            textField.KeyboardType = UIKeyboardType.Default;
            textField.ReturnKeyType = UIReturnKeyType.Done;
            textField.EditingChanged += TextField_ValueChanged;
            textField.OnDeleteBackward += TextField_OnDeleteBackward;
            textField.ShouldReturn += TextField_ShouldReturn;

            gameViewController.Add(textField);

            UIKeyboard.Notifications.ObserveWillShow((s, e) =>
            {
                virtualKeyboardHeight = (int)e.FrameBegin.Height;
            });

            UIKeyboard.Notifications.ObserveWillHide((s, e) =>
            {
                virtualKeyboardHeight = 0;
            });
        }

        private void TextField_OnDeleteBackward(object sender, EventArgs e)
        {
            var key = Keys.Back;
            OnTextInput(new TextInputEventArgs((char)key, key));
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
