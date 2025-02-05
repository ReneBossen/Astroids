using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _thrustForce;
        [SerializeField] private float _drag;
        [SerializeField] private float _rotationSpeed;
        private PlayerInput _playerInput;
        private Rigidbody2D _rigidbody;
        private Vector2 _Input;

        private void Awake()
        {
            _playerInput = new PlayerInput();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            _playerInput.Enable();
            _playerInput.Player.Move.performed += OnMove;
            _playerInput.Player.Move.canceled += OnMoveCanceled;
        }

        private void OnDisable()
        {
            _playerInput.Player.Move.performed -= OnMove;
            _playerInput.Player.Move.canceled -= OnMoveCanceled;
            _playerInput.Disable();
        }

        private void FixedUpdate()
        {
            ApplyThrust(_Input.y);
            ApplyRotation(_Input.x);
        }

        private void ApplyRotation(float rotation)
        {
            _rigidbody.rotation -= rotation * _rotationSpeed * Time.deltaTime;
        }

        private void ApplyThrust(float thrust)
        {
            if (thrust > 0)
            {
                Vector2 thrustDirection = transform.up;
                _rigidbody.AddForce(thrustDirection * _thrustForce * thrust);
            }

            _rigidbody.linearVelocity *= _drag;
        }

        private void OnMove(InputAction.CallbackContext obj)
        {
            _Input = obj.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext obj)
        {
            _Input = Vector2.zero;
        }
    }
}