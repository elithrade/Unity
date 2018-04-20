using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    public Vector2 FallingSpeedMinMax;
    private float _fallingBoundaryThreshold;
    private float _speed;

    void Start()
    {
        // Since half screen size is the orthographicSize the lower boundary of screen
        // is the negative size plus the height of the block
        _fallingBoundaryThreshold = -Camera.main.orthographicSize - transform.localScale.y;
        _speed = Mathf.Lerp(FallingSpeedMinMax.x, FallingSpeedMinMax.y, Difficulty.GetDifficultyPercentage());
    }
    
    void Update ()
    {
        transform.Translate(Vector2.down * _speed * Time.deltaTime);

        if (transform.position.y < _fallingBoundaryThreshold)
            Destroy(gameObject);
    }
}
