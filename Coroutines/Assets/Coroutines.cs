using System.Collections;
using UnityEngine;

public class Coroutines : MonoBehaviour
{
    public Transform[] Waypoints;

    private Coroutine _moveCoroutine;

    private void Start ()
    {
        string[] messages = {"Welcome", "to", "this", "amasing", "game"};
        StartCoroutine(PrintMessages(messages, 1f));
        StartCoroutine(MoveToWaypoints());
    }

    private IEnumerator Move(Vector3 destination, float speed)
    {
        while (transform.position != destination)
        {
            // Note the maxDistanceDelta is the amount of distance to move in the current frame
            transform.position =  Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            // Pause coroutine until the next frame
            yield return null;
        }
    }

    private IEnumerator MoveToWaypoints()
    {
        for(int i = 0; i < Waypoints.Length; i++)
        {
            Transform waypoint = Waypoints[i];

            // We don't know how long the coroutine is running
            // we want to wait for it to finish
            yield return Move(waypoint.position, 5);
        }
    }

    private IEnumerator PrintMessages(string[] messages, float delay)
    {
        foreach (string message in messages)
        {
            print(message);
            // Pasuse coroutine for specified delay seconds
            yield return new WaitForSeconds(delay);
        }
    }
    
    private void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);

            _moveCoroutine = StartCoroutine(Move(Random.onUnitSphere * 5, 8));
        }
    }
}
