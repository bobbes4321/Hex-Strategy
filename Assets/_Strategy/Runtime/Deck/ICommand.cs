using System.Collections.Generic;
using _Strategy.Runtime.Board;
using _Strategy.Runtime.Piece;

namespace _Strategy.Runtime.Deck
{
    public interface ICommand
    {
        /// <summary>
        /// Returns the hexes that this command can be executed on.
        /// </summary>
        /// <param name="piece">The piece that the hexes will be calculated to, relatively.
        /// Moves are usually relative to the piece that is selected.</param>
        /// <returns></returns>
        public IEnumerable<Hex> GetHexes(IPiece piece);
        /// <summary>
        /// Is used to highlight the hexes that will be triggered when hovering over a hex.
        /// For example, the AOE attack will highlight all 7 hexes that will take damage.
        /// This is not used for all commands (in fact, only needed for AOE attacks?)
        /// </summary>
        /// <param name="hoveredHex">The current Hex that the mouse is over</param>
        /// <returns></returns>
        public IEnumerable<Hex> GetHoverHexes(Hex hoveredHex);
        public void Execute(IPiece piece, Hex selectedHex);
    }
}