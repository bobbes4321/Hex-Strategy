using _Strategy.Runtime.Board;
using _Strategy.Runtime.Gameloop;
using Cysharp.Threading.Tasks;
using Doozy.Runtime.UIManager.Components;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Strategy.Runtime.Deck
{
    public class UpgradeCard : MonoBehaviour, IPointerClickHandler
    {
        [Inject] private UpgradeSelection _upgradeSelection;
        
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        private UpgradeCardData _cardData;
        
        public void OnPointerClick(PointerEventData _)
        {
            _cardData.Upgrade.Execute(DI.GetSceneContainer());
            _upgradeSelection.FinishCardSelection();
        }

        public void Initialize(UpgradeCardData cardData)
        {
            _cardData = cardData;
            _image.sprite = cardData.Icon;
            _title.SetText(cardData.Name);
            _description.SetText(cardData.Description);
        }
    }
}