using Teo.AutoReference;
using TMPro;
using UnityEngine;

namespace _Strategy.Runtime.Piece
{
    public class EnemyTitleAndDescriptionSetter : MonoBehaviour
    {
        [SerializeField, GetInParent] private EnemyPiece _enemyPiece;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        
        private void Start()
        {
            if (_enemyPiece == null)
                return;
            
            _title.SetText(_enemyPiece.Settings.Name);
            _description.SetText(_enemyPiece.Settings.Description);
        }
    }
}