using UnityEngine;

public class Player : MonoBehaviour
{
    public float MoveSpeed = 7;
    // The time it takes for the smoothed move magnitude to catch up
    public float SmoothedMoveTime = .1f;
    public float TurnSpeed = 8;

    private float _smoothedInputMagnitude;
    private float _smoothedMoveVelocity;
    private float _currentAngle;

    void Update () {
        Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        // inputDirection.magnitude will be 1 if any of the arrow keys held down, 0 otherwise
        float inputMagnitude = inputDirection.magnitude;
        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;

        // LerpAngle handles correct interpolation around 360 degrees
        _currentAngle = Mathf.LerpAngle(_currentAngle, targetAngle, TurnSpeed * Time.deltaTime * inputMagnitude);
        transform.eulerAngles = _currentAngle * Vector3.up;

        _smoothedInputMagnitude = Mathf.SmoothDamp(_smoothedInputMagnitude, inputMagnitude, ref _smoothedMoveVelocity, SmoothedMoveTime);
        transform.Translate(inputDirection * MoveSpeed * _smoothedInputMagnitude * Time.deltaTime, Space.World);
    }
}
