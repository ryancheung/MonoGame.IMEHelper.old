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

            //_nativeWnd.CandidatesReceived += (s, e) => { if (CandidatesReceived != null) CandidatesReceived(s, e); };

            GameInstance.Exiting += (o, e) => this.Dispose();
        }

        /// <summary>
        /// Enable / Disable IME
        /// </summary>
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

        /// <summary>
        /// Array of the candidates
        /// </summary>
        public override string[] Candidates => _nativeWnd.Candidates;

        /// <summary>
        /// How many candidates should display per page
        /// </summary>
        public override uint CandidatesPageSize => _nativeWnd.CandidatesPageSize;

        /// <summary>
        /// First candidate index of current page
        /// </summary>
        public override uint CandidatesPageStart => _nativeWnd.CandidatesPageStart;

        /// <summary>
        /// The selected canddiate index
        /// </summary>
        public override uint CandidatesSelection => _nativeWnd.CandidatesSelection;

        /// <summary>
        /// Composition String
        /// </summary>
        public override string Composition => _nativeWnd.CompositionString;

        /// <summary>
        /// Composition Clause
        /// </summary>
        public override string CompositionClause => _nativeWnd.CompositionClause;

        /// <summary>
        /// Composition Reading String
        /// </summary>
        public override string CompositionRead => _nativeWnd.CompositionReadString;

        /// <summary>
        /// Composition Reading Clause
        /// </summary>
        public override string CompositionReadClause => _nativeWnd.CompositionReadClause;

        /// <summary>
        /// Caret position of the composition
        /// </summary>
        public override int CompositionCursorPos => _nativeWnd.CompositionCursorPos;

        /// <summary>
        /// Result String
        /// </summary>
        public override string Result => _nativeWnd.ResultString;

        /// <summary>
        /// Result Clause
        /// </summary>
        public override string ResultClause => _nativeWnd.ResultClause;

        /// <summary>
        /// Result Reading String
        /// </summary>
        public override string ResultRead => _nativeWnd.ResultReadString;

        /// <summary>
        /// Result Reading Clause
        /// </summary>
        public override string ResultReadClause => _nativeWnd.ResultReadClause;

        /// <summary>
        /// Get the composition attribute at character index.
        /// </summary>
        /// <param name="index">Character Index</param>
        /// <returns>Composition Attribute</returns>
        public override CompositionAttributes GetCompositionAttr(int compStringIndex)
        {
            return _nativeWnd.GetCompositionAttr(compStringIndex);
        }

        /// <summary>
        /// Get the composition read attribute at character index.
        /// </summary>
        /// <param name="index">Character Index</param>
        /// <returns>Composition Attribute</returns>
        public override CompositionAttributes GetCompositionReadAttr(int compStringIndex)
        {
            return _nativeWnd.GetCompositionReadAttr(compStringIndex);
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
