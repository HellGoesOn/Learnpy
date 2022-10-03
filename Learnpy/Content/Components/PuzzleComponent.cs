using Learnpy.Core.ECS;
using static Learnpy.Content.Enums;

namespace Learnpy.Content.Components
{
    public struct PuzzleComponent
    {
        public PieceType PieceType;

        public bool BeingDragged;

        /// <summary>
        /// Used to determine which puzzle piece THIS one connects to
        /// </summary>
        public int ConnectedTo { get; set; }

        /// <summary>
        /// Used to determine which puzzle piece connected to THIS one
        /// </summary>
        public int ConnectionTo { get; set; }

        public bool CanConnect { get; set; }

        public bool CanBeConnectedTo { get; set; }

        public string StoredText { get; set; }

        public PuzzleComponent(PieceType pieceType, bool beingDragged = false)
        {
            StoredText = "";
            ConnectionTo = -1;
            ConnectedTo = -1;
            CanConnect = true;
            CanBeConnectedTo = true;
            PieceType = pieceType;
            BeingDragged = beingDragged;
        }

    }
}
