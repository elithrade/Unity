using UnityEngine;

public class Player : MonoBehaviour {
	public const float Speed = 10;
	private void Update () {
		// Project settings -> InputManager -> Axes allows changing names of each axis
		// input will be -1 in horizontal axis if left or a is pressed, same with vertical axis
		var input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

		Vector3 direction = input.normalized;
		Vector3 velocity = direction * Speed;
		Vector3 movementAmount = velocity * Time.deltaTime;

		transform.Translate(movementAmount);
	}
}
