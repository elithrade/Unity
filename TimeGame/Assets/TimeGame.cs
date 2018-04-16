using UnityEngine;

public class TimeGame : MonoBehaviour
{
    private static float s_roundDelaySeconds = 3;
    private float _roundStartTime;
    private float _waitTime;
    private bool _roundStarted;

    // Use this for initialization
    private void Start()
    {
        print("Press the space bar once you think the allotted time is up.");
        StartNewRandomTime();
    }

    private void SetNewRandomTime()
    {
        // Generate random seconds between 5 to 20
        _waitTime = Random.Range(5, 21);
        // Round starts now
        _roundStartTime = Time.time;
        print("Wait time: " + _waitTime);

        _roundStarted = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) && _roundStarted)
        {
            ReceivedInput();
        }
    }

    private void ReceivedInput()
    {
        _roundStarted = false;

        float playerWaitTime = Time.time - _roundStartTime;
        float deltaTime = Mathf.Abs(_waitTime - playerWaitTime);

        print(string.Format("You have waited for {0} seconds, the error is {1} {2}",
            playerWaitTime, deltaTime, GenerateMessage(deltaTime)));

        StartNewRandomTime();
    }

    private string GenerateMessage(float error)
    {
        string message = string.Empty;
        if (error < .15f)
            message = "Outstanding.";
        else if (error < .75f)
            message = "Exceeds Expectation.";
        else if (error < 1.25f)
            message = "Acceptable.";
        else if (error < 1.75f)
            message = "Poor.";
        else
            message = "Dreadful.";

        return message;
    }

    private void StartNewRandomTime()
    {
        // Starts new random time after specified seconds
        Invoke("SetNewRandomTime", s_roundDelaySeconds);
    }
}
