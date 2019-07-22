using CoreGraphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using UIKit;

namespace MonoGame.IMEHelper
{
    internal class TextInputViewController : UIViewController
    {
        public UITextField TextField { get; private set; }

        private IosIMEHandler imeHandler;

        public TextInputViewController(IosIMEHandler imeHandler) : base()
        {
            this.imeHandler = imeHandler;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TextField = new UITextField(new CGRect(0, -100, 200, 40));
            TextField.KeyboardType = UIKeyboardType.Default;
            TextField.ReturnKeyType = UIReturnKeyType.Done;
            TextField.ValueChanged += TextField_ValueChanged;

            View.AddSubview(TextField);
        }

        private void TextField_ValueChanged(object sender, EventArgs e)
        {
            foreach (var c in TextField.Text)
                imeHandler.OnTextInput(new TextInputEventArgs(c, KeyboardUtil.ToXna(c)));

            TextField.Text = string.Empty;
        }
    }

    internal class IosIMEHandler : IMEHandler
    {
        private UIWindow mainWindow;
        private UIViewController gameViewController;
        private TextInputViewController textInputViewController;

        private int virtualKeyboardHeight;

        public IosIMEHandler(Game game, bool showDefaultIMEWindow = false) : base(game, showDefaultIMEWindow)
        {
        }

        public override bool Enabled { get; protected set; }

        public override void PlatformInitialize()
        {
            mainWindow = GameInstance.Services.GetService<UIWindow>();
            gameViewController = GameInstance.Services.GetService<UIViewController>();
            textInputViewController = new TextInputViewController(this);

            UIKeyboard.Notifications.ObserveWillShow((s, e) =>
            {
                var r = UIKeyboard.FrameBeginFromNotification(e.Notification);
                virtualKeyboardHeight = (int)r.Height;
            });
        }

        public override void StartTextComposition()
        {
            if (Enabled)
                return;

            gameViewController.PresentViewController(textInputViewController, false, new Action(() =>
            {
                textInputViewController.TextField.BecomeFirstResponder();
            }));
        }

        public override void StopTextComposition()
        {
            if (!Enabled)
                return;

            textInputViewController.TextField.Text = string.Empty;
            textInputViewController.TextField.ResignFirstResponder();
            gameViewController.DismissViewController(false, null);
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
