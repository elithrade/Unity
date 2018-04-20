using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    public Vector2 FallingSpeedMinMax;
    private float _speed;

    void Start()
    {
        _speed = Mathf.Lerp(FallingSpeedMinMax.x, FallingSpeedMinMax.y, Difficulty.GetDifficultyPercentage());
    }
    
    void Update ()
    {
        transform.Translate(Vector2.down * _speed * Time.deltaTime);
    }
}
