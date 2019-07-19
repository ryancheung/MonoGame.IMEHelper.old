using System;

namespace MonoGame.IMEHelper
{
    public struct CandidateList
    {
        /// <summary>
        /// Array of the candidates
        /// </summary>
        public string[] Candidates;

        /// <summary>
        /// First candidate index of current page
        /// </summary>
        public uint CandidatesPageStart;

        /// <summary>
        /// How many candidates should display per page
        /// </summary>
        public uint CandidatesPageSize;

        /// <summary>
        /// The selected canddiate index
        /// </summary>
        public uint CandidatesSelection;
    }

    /// <summary>
    /// Arguments for the <see cref="IMEHandler.TextComposition" /> event.
    /// </summary>
    public class TextCompositionEventArgs : EventArgs
    {
        /// <summary>
        /// Initialize a TextCompositionEventArgs
        /// </summary>
        /// <param name="compositedText"></param>
        /// <param name="cursorPosition"></param>
        /// <param name="candidateList"></param>
        public TextCompositionEventArgs(string compositedText, int cursorPosition, CandidateList? candidateList = null)
        {
            CompositedText = compositedText;
            CursorPosition = cursorPosition;
            CandidateList = candidateList;
        }

        /// <summary>
        /// The full string as it's composited by the IMM.
        /// </summary>
        public string CompositedText { get; }

        /// <summary>
        /// The position of the cursor inside the composited string.
        /// </summary>
        public int CursorPosition { get; }

        /// <summary>
        /// The suggested alternative texts for the composition.
        /// </summary>
        public CandidateList? CandidateList { get; }

    }
}
