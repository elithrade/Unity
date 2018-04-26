using System;
using System.Collections;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public static event Action OnPlayerSpotted;

    public Transform Path;
    public float Speed = 7;
    public float WaitTime = .2f;
    // 90 degrees per seconds
    public float TurnSpeed = 90;
    public Light SpotLight;
    public float ViewDistance;
    public LayerMask ViewMask;
    public float TimeToSpotPlayer = .5f;

    private Transform _player;
    private Color _originalSpotLightColor;
    private float _playerVisibleTimer;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _originalSpotLightColor = SpotLight.color;

        Vector3[] waypoints = new Vector3[Path.childCount];
        for (int i = 0; i < Path.childCount; i++)
        {
            var childPosition = Path.GetChild(i).position;
            // Change waypoint height to be the same as transform
            waypoints[i] = new Vector3(childPosition.x, transform.position.y, childPosition.z);
        }

        StartCoroutine(FollowPath(waypoints));
    }

    IEnumerator FollowPath(Vector3[] path)
    {
        transform.position = path[0];

        int nextWaypointIndex = 1;
        Vector3 targetWaypoint = path[nextWaypointIndex];
        transform.LookAt(targetWaypoint);

        for(;;)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, Speed * Time.deltaTime);
            if (transform.position == targetWaypoint)
            {
                // Ensure we go back to the beginning once we reached the last waypoint
                nextWaypointIndex = (nextWaypointIndex + 1) % path.Length;
                targetWaypoint = path[nextWaypointIndex];
                // Wait for half a second once we reached the target
                yield return new WaitForSeconds(WaitTime);
                // Wait for rotation to finish
                yield return StartCoroutine(Turn(targetWaypoint));
            }
            // Wait for one frame
            yield return null;
        }
    }

    IEnumerator Turn(Vector3 lookAt)
    {
        Vector3 directionToTarget = (lookAt - transform.position).normalized;
        float angle = Mathf.Atan2(directionToTarget.z, directionToTarget.x) * Mathf.Rad2Deg;
        // Unit unit circle is 90 degrees off clockwise
        // You can do 90 - θ to get the correct angle, or you can swap the axes. So either do 90 - atan2(y,x); or simply atan2(x,y)
        float targetAngle = 90 - angle;
        // y is the axis we rotate around
        float deltaAngle = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);
        // deltaAngle will be negative is the turn is anti-clockwise
        while (Mathf.Abs(deltaAngle) > 0.05f)
        {
            float turnAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, TurnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * turnAngle;
            deltaAngle = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);
            yield return null;
        }
    }

    private bool CanSeePlayer()
    {
        // Based on 3 factors:
        // 1. Player is within the view distance
        // 2. Player is within the view angle
        // 3. Player is not behind an obstacle
        if (Vector3.Distance(transform.position, _player.position) < ViewDistance)
        {
            // Guard's forward direction and player's direction
            Vector3 directionToPlayer = (_player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleBetweenGuardAndPlayer < SpotLight.spotAngle / 2)
            {
                // Cast a ray between guard's position to player's position with a layer mask only picks obstacles
                // If that ray hits anything, that means we hit an obstacle
                if (!Physics.Linecast(transform.position, _player.position, ViewMask))
                    return true;
            }
        }

        return false;
    }

    private void Update()
    {
        if (CanSeePlayer())
            _playerVisibleTimer += Time.deltaTime;
        else
            _playerVisibleTimer -= Time.deltaTime;

        _playerVisibleTimer = Mathf.Clamp(_playerVisibleTimer, 0, TimeToSpotPlayer);
        SpotLight.color = Color.Lerp(_originalSpotLightColor, Color.red, _playerVisibleTimer / TimeToSpotPlayer);

        if (_playerVisibleTimer == TimeToSpotPlayer)
        {
            if (OnPlayerSpotted != null)
                OnPlayerSpotted();
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 startPosition = Path.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach (Transform waypoint in Path)
        {
            Gizmos.DrawSphere(waypoint.position, .3f);
            if (previousPosition == waypoint.position)
                continue;

            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }

        Gizmos.DrawLine(previousPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * ViewDistance);
    }
}
