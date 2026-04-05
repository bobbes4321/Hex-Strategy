using Teo.AutoReference;
using TMPro;
using UnityEngine;

namespace _Strategy.Runtime.Board
{
    public class HexCoordinateView : MonoBehaviour
    {
        [SerializeField, Get] private HexView _hexView;
        [SerializeField] private TMP_Text _q;
        [SerializeField] private TMP_Text _r;
        [SerializeField] private TMP_Text _s;

        private void Start()
        {
            var hex = _hexView.Hex;
            _q.text = hex.Q.ToString();
            _r.text = hex.R.ToString();
            _s.text = hex.S.ToString();
        }
    }
}