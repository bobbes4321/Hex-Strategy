using System;
using System.Collections.Generic;
using _Strategy.Runtime.Gameloop;
using _Strategy.Runtime.Input;
using _Strategy.Runtime.Piece;
using _Strategy.Runtime.Util;
using PrimeTween;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Teo.AutoReference;
using UnityEngine;
using UnityEngine.EventSystems;
using NullPiece = _Strategy.Runtime.Piece.NullPiece;

namespace _Strategy.Runtime.Board
{
    [RequireComponent(typeof(Selectable))]
    public class HexView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [ReadOnly] public Hex Hex;
        [SerializeField, GetInChildren] private MeshRenderer _meshRenderer;
        [Inject] private Board _board;
        [Inject] private HexHighlightManager _hexHighlightManager;
        [SerializeField] private Material _defaultMaterial;
        [SerializeField] private Material _highlightedMaterial;
        [SerializeField] private Material _selectedMaterial;
        [SerializeField] private Material _hoveredMaterial;
        [SerializeField] private Material _enemyMaterial;

        private Dictionary<HighlightState, Material> _materials;
        private HighlightState _highlightState = HighlightState.None;

        public HighlightState HighlightState => _highlightState;
        private Selectable _selectable;
        public event Action<HexView> OnHoverEntered;
        public event Action<HexView> OnHoverExited;

        public static HexView Create(Hex hex, Transform parent, HexView prefab)
        {
            var hexView = Instantiate(prefab, parent);
            hexView.name = hex.ToString();
            hexView.transform.SetParent(parent);
            hexView.InitializeHexView(hex, hexView);
            return hexView;
        }

        private void InitializeHexView(Hex hex, HexView hexView)
        {
            hexView.Hex = hex;
            Vector3 position = Board.HexToWorld(hex) + new Vector3(0, -0.1f, 0);
            hexView.transform.position = position;
            hexView.transform.localScale = Vector3.one * Board.HexSize;
        }

        private void Awake()
        {
            _selectable = GetComponent<Selectable>();
            _selectable.OnSelected += TileClicked;

            SetupMaterials();
        }

        private void SetupMaterials()
        {
            _materials = new Dictionary<HighlightState, Material>
            {
                { HighlightState.None, _defaultMaterial },
                { HighlightState.Highlighted, _highlightedMaterial },
                { HighlightState.Hovered, _hoveredMaterial },
                { HighlightState.Selected, _selectedMaterial },
                { HighlightState.Enemy, _enemyMaterial }
            };
        }

        private void TileClicked() => Hex.Select();
        private void OnDestroy() => _selectable.OnSelected -= TileClicked;

        public void SetHighlightState(HighlightState state)
        {
            _highlightState = state;

            if (_meshRenderer == null || _materials == null)
            {
                Debug.LogError("[Board.SetHighlightState] MeshRenderer or Materials are null.", this);
                return;
            }

            _meshRenderer.material = _materials[state];
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!GameManager.IsPlayersTurn)
                return;

            Tween.Scale(_meshRenderer.transform, Vector3.one * 0.9f, 0.2f, Ease.OutCubic);
            OnHoverEntered?.Invoke(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_meshRenderer.transform.localScale != Vector3.one)
                Tween.Scale(_meshRenderer.transform, Vector3.one, 0.2f, Ease.OutCubic);
            
            OnHoverExited?.Invoke(this);
        }
    }

    public enum HighlightState
    {
        None, //Default
        Highlighted, //Can be clicked on to activate
        Selected, //Was activated and is the current epicenter
        Hovered, //Mouse is over the tile
        Enemy
    }
}