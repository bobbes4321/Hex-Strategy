using System;
using System.Collections.Generic;
using System.Linq;
using _Strategy.Runtime.Board;
using _Strategy.Runtime.Piece;
using UnityEngine.EventSystems;

namespace _Strategy.Runtime.Deck
{
    [Serializable]
    public class HexCardProcessor : CardProcessor
    {
        public override void OnPointerEnter(PointerEventData eventData)
        {
            _hexHighlightManager.HighlightHexes(_data.Command.GetHexes(new NullPiece()));
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            _hexHighlightManager.UnhighlightHexes();
        }

        public override void OnEndDrag(Hex hex)
        {
            var piece = _board.PieceAt(hex);
            var validHexes = _data.Command.GetHexes(piece);

            if (validHexes.Contains(hex))
                _card.Activate(piece, hex);

            _hexHighlightManager.UnhighlightHexes();
        }

        public override IEnumerable<Hex> GetHexes()
        {
            yield break;
        }
    }
}