using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Chess
{
    [RequireComponent(typeof(Selectable))]
    public class TileView : MonoBehaviour
    {
        [ReadOnly] public Tile Tile;

        private Color _defaultColor;
        private Color _currentColor;

        private MeshRenderer _meshRenderer;
        private Selectable _selectable;

        public Color CurrentColor
        {
            get => _currentColor;
            set
            {
                if (_currentColor != value)
                {
                    if (_meshRenderer == null) _meshRenderer = GetComponent<MeshRenderer>();
                    _meshRenderer.material.color = value;
                    _currentColor = value;
                }
            }
        }

        public static TileView Create(Tile tile, float tileSize, Vector3 bottomLeftPosition, Material material)
        {
            var tileView = new GameObject(tile.ToString()).AddComponent<TileView>();

            tileView.InitializeTileView(tile, tileSize, bottomLeftPosition, material, tileView);
            return tileView;
        }

        public void InitializeTileView(Tile tile, float tileSize, Vector3 bottomLeftPosition, Material material, TileView tileView)
        {
            tileView.Tile = tile;

            var meshFilter = tileView.gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = MeshUtils.CreateCubeMesh();
            gameObject.AddComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
            var meshRenderer = tileView.gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
            var color = (tile.X + tile.Y) % 2 == 0 ? Color.white : Color.black;
            tileView.CurrentColor = color;
            _defaultColor = color;
            Vector3 position = (bottomLeftPosition + new Vector3(tile.X, -0.5f, tile.Y) * tileSize) - tileSize * Vector3.one * 0.5f;
            tileView.transform.position = position;
        }

        private void Awake()
        {
            _selectable = GetComponent<Selectable>();

            _selectable.OnSelected += TileClicked;
        }

        private void TileClicked()
        {
            Tile.Select();
        }

        private void OnDestroy()
        {
            _selectable.OnSelected -= TileClicked;
        }

        public void Highlight()
        {
            CurrentColor = Color.yellow;
        }

        public void Unhighlight()
        {
            CurrentColor = _defaultColor;
        }
    }
}