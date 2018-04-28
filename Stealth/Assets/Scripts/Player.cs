using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static event Action OnGameFinished;
    public float MoveSpeed = 7;
    // The time it takes for the smoothed move magnitude to catch up
    public float SmoothedMoveTime = .1f;
    public float TurnSpeed = 8;

    private float _smoothedInputMagnitude;
    private float _smoothedMoveVelocity;
    private float _currentAngle;
    private Vector3 _currentVelocity;
    private Rigidbody _rigidbody;
    private bool _disabled;

    private void Start()
    {
        // Rigidbody is used to update player's transform
        // Rigidbody should be updated in FixedUpdate
        _rigidbody = GetComponent<Rigidbody>();
        Guard.OnPlayerSpotted += Disable;
    }

    private void OnDestroy()
    {
        Guard.OnPlayerSpotted -= Disable;
    }

    private void Disable()
    {
        _disabled = true;
    }

    private void FixedUpdate()
    {
        _rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * _currentAngle));
        _rigidbody.MovePosition(_rigidbody.position + _currentVelocity * Time.deltaTime);
    }

    private void Update()
    {
        Vector3 inputDirection = Vector3.zero;
        if (!_disabled)
            inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        // inputDirection.magnitude will be 1 if any of the arrow keys held down, 0 otherwise
        float inputMagnitude = inputDirection.magnitude;
        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;

        // LerpAngle handles correct interpolation around 360 degrees
        _currentAngle = Mathf.LerpAngle(_currentAngle, targetAngle, TurnSpeed * Time.deltaTime * inputMagnitude);
        _smoothedInputMagnitude = Mathf.SmoothDamp(_smoothedInputMagnitude, inputMagnitude, ref _smoothedMoveVelocity, SmoothedMoveTime);
        _currentVelocity = inputDirection * MoveSpeed * _smoothedInputMagnitude;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Finish")
        {
            Disable();
            if (OnGameFinished != null)
                OnGameFinished.Invoke();
        }
    }
}
