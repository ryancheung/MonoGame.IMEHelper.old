using System;
using Microsoft.Xna.Framework;

namespace MonoGame.IMEHelper
{
    /// <summary>
    /// Integrate IME to XNA framework.
    /// </summary>
    public class WinFormsIMEHandler : IMEHandler, IDisposable
    {
        private IMENativeWindow _nativeWnd;

        public WinFormsIMEHandler(Game game, bool showDefaultIMEWindow = false) : base(game, showDefaultIMEWindow)
        {
        }

        public override void PlatformInitialize()
        {
            _nativeWnd = new IMENativeWindow(this, GameInstance.Window.Handle, ShowDefaultIMEWindow);

            GameInstance.Exiting += (o, e) => this.Dispose();
        }

        public override bool Enabled { get; protected set; }

        public override void StartTextComposition()
        {
            if (Enabled)
                return;

            Enabled = true;
            _nativeWnd.EnableIME();
        }

        public override void StopTextComposition()
        {
            if (!Enabled)
                return;

            Enabled = false;
            _nativeWnd.DisableIME();
        }

        public override void SetTextInputRect(ref Rectangle rect)
        {
            if (!Enabled)
                return;

            _nativeWnd.SetTextInputRect(ref rect);
        }

        public override string[] Candidates => _nativeWnd.Candidates;
        public override uint CandidatesPageSize => _nativeWnd.CandidatesPageSize;
        public override uint CandidatesPageStart => _nativeWnd.CandidatesPageStart;
        public override uint CandidatesSelection => _nativeWnd.CandidatesSelection;

        public override string Composition => _nativeWnd.CompositionString;
        public override string CompositionClause => _nativeWnd.CompositionClause;
        public override string CompositionRead => _nativeWnd.CompositionReadString;
        public override string CompositionReadClause => _nativeWnd.CompositionReadClause;
        public override int CompositionCursorPos => _nativeWnd.CompositionCursorPos;

        public override string Result => _nativeWnd.ResultString;
        public override string ResultClause => _nativeWnd.ResultClause;
        public override string ResultRead => _nativeWnd.ResultReadString;
        public override string ResultReadClause => _nativeWnd.ResultReadClause;

        public override CompositionAttributes GetCompositionAttr(int charIndex)
        {
            return _nativeWnd.GetCompositionAttr(charIndex);
        }

        public override CompositionAttributes GetCompositionReadAttr(int charIndex)
        {
            return _nativeWnd.GetCompositionReadAttr(charIndex);
        }

        /// <summary>
        /// Dispose everything
        /// </summary>
        public void Dispose()
        {
            _nativeWnd.Dispose();
        }
    }
}
