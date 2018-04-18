using UnityEngine;

public class Chaser : MonoBehaviour {

	public Transform TargetTransform;
	public float Speed = 7;

	void Update () {
		// Substract our own position from the target
		// and normalise it to get a direction towards the target
		Vector3 displacementFromTarget = TargetTransform.position - transform.position;
		Vector3 directionToTarget = displacementFromTarget.normalized;
		Vector3 velocity = directionToTarget * Speed;
		Vector3 movementAmount = velocity * Time.deltaTime;

		float distanceToPlayer = displacementFromTarget.magnitude;

		// Keeps distance between player and chaser
		if (distanceToPlayer > 1.5f)
			transform.Translate(movementAmount);
	}
}
