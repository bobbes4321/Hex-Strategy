using System;
using _Strategy.Runtime.Board;
using _Strategy.Runtime.Gameloop;
using _Strategy.Runtime.Piece;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Reflex.Attributes;
using Runtime.Messaging;
using UnityEngine;
using UnityEngine.InputSystem;
using Ease = PrimeTween.Ease;
using Tween = PrimeTween.Tween;

namespace _Strategy.Runtime.Input
{
    public class CameraController : MonoBehaviour, IMessageHandler<TurnFinishedMessage>
    {
        public float PanSpeed = 20f;
        public float ScreenEdgeBorderThickness = 10f;
        public float ZoomSpeed = 10f;
        public float MinZoomDistance = 10f;
        public float MaxZoomDistance = 100f;
        public float RotationSpeed;

        private Vector2 _panDirection;
        private float _zoomInput;
        private Camera _camera;
        private Vector3 _initialPosition;

        [Inject] private EnemyAI _enemyAI;
        [Inject] private MessagingManager _messagingManager;
        [Inject] private Board.Board _board;
        private Vector3 _lastPositionBeforeEnemyTurn;
        private bool _isFollowingEnemyMovement;
        private bool _isGoingToStartPosition = false;
        
        private void Awake() => _camera = Camera.main;
        private void Start()
        {
            GoToStartPosition().Forget();
            _messagingManager.RegisterHandler(this);
        }

        private void OnDestroy() => _messagingManager.UnregisterHandler(this);

        private async UniTask GoToStartPosition()
        {
            await UniTask.WaitWhile(() => !(Piece.Piece)_board.GetPiecesOfTeam(Team.White)[0]);
            _isGoingToStartPosition = true;
            await UniTask.WaitForSeconds(0.5f);
            _isGoingToStartPosition = false;
        }
        
        private void Update()
        {
#if !UNITY_EDITOR
            HandleMousePan();
#endif
            if (_isGoingToStartPosition)
            {
                FollowPiece((Piece.Piece)_board.GetPiecesOfTeam(Team.White)[0]);
            }
            else if (_isFollowingEnemyMovement) 
            {
                FollowPiece(_enemyAI.CurrentPiece);
            }
            else
            {
                HandleKeyboardPan();
                HandleZoom();
                HandleRotation();
            }
        }

        private void FollowPiece(Piece.Piece currentEnemyPiece)
        {
            if (currentEnemyPiece == null) return;

            Vector3 piecePosition = currentEnemyPiece.transform.position;
            Vector3 currentCameraPosition = _camera.transform.position;

            // Calculate the offset from the piece to where the camera should be positioned
            // to keep the piece centered in the angled view
            Vector3 cameraForward = _camera.transform.forward;
            Vector3 cameraUp = _camera.transform.up;

            // Project the camera's forward direction onto the horizontal plane (ignore Y component)
            Vector3 horizontalForward = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;

            // Calculate the distance from camera to the piece position on the ground
            float currentHeight = currentCameraPosition.y - piecePosition.y;
            float horizontalDistance = currentHeight / Mathf.Tan(_camera.transform.eulerAngles.x * Mathf.Deg2Rad);

            // Calculate the target camera position
            Vector3 targetPosition = piecePosition - horizontalForward * horizontalDistance;
            targetPosition.y = currentCameraPosition.y; // Maintain current height

            // Smoothly move the camera towards the target position
            Vector3 newPosition = Vector3.Lerp(currentCameraPosition, targetPosition, PanSpeed * Time.deltaTime);
            _camera.transform.position = newPosition;
        }

        private void HandleRotation()
        {
            if (!Mouse.current.middleButton.isPressed) return;
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            transform.RotateAround(transform.position, Vector3.up, mouseDelta.x * RotationSpeed);
        }

        private void HandleMousePan()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            _panDirection = Vector2.zero;

            if (mousePosition.x >= Screen.width - ScreenEdgeBorderThickness)
                _panDirection.x = 1;
            else if (mousePosition.x <= ScreenEdgeBorderThickness)
                _panDirection.x = -1;

            if (mousePosition.y >= Screen.height - ScreenEdgeBorderThickness)
                _panDirection.y = 1;
            else if (mousePosition.y <= ScreenEdgeBorderThickness)
                _panDirection.y = -1;

            Vector3 movement = new Vector3(_panDirection.x, 0, _panDirection.y) * PanSpeed * Time.deltaTime;
            _camera.transform.Translate(movement, Space.World);
        }

        private void HandleKeyboardPan()
        {
            Vector2 moveInput = Keyboard.current.wKey.isPressed ? Vector2.up : Vector2.zero;
            moveInput += Keyboard.current.sKey.isPressed ? Vector2.down : Vector2.zero;
            moveInput += Keyboard.current.aKey.isPressed ? Vector2.left : Vector2.zero;
            moveInput += Keyboard.current.dKey.isPressed ? Vector2.right : Vector2.zero;

            Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y) * PanSpeed * Time.deltaTime;
            _camera.transform.Translate(movement, Space.World);
        }

        private void HandleZoom()
        {
            _zoomInput = Mouse.current.scroll.y.ReadValue();
            float zoomAmount = _zoomInput * ZoomSpeed;
            Vector3 newPosition = _camera.transform.position + _camera.transform.forward * zoomAmount;
            _camera.transform.position = newPosition;
        }

        public void Handle(TurnFinishedMessage message)
        {
            var currentTeam = GameManager.CurrentTeam;

            if (currentTeam == Team.White)
            {
                ReturnToLastPosition().Forget();
            }
            else
            {
                _isFollowingEnemyMovement = true;
                _lastPositionBeforeEnemyTurn = _camera.transform.position;
            }
        }

        private async UniTask ReturnToLastPosition()
        {
            await Tween.Position(_camera.transform, _lastPositionBeforeEnemyTurn, 0.5f, Ease.InOutCubic);
            _isFollowingEnemyMovement = false;
        }
    }
}