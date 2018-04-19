using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed = 7;
    private float _screenHalfWidthInWorldUnit;
    
    private void Start()
    {
        // Aspect ratio = screen width / screen height
        // Orthographic size = screen height / 2 in world unit
        // Aspect ration * orthographic size -> screen width / 2 in world unit
        _screenHalfWidthInWorldUnit = Camera.main.aspect * Camera.main.orthographicSize;
        // Take into account of player's half size
        float playerHalfSize = transform.localScale.x / 2;
        _screenHalfWidthInWorldUnit += playerHalfSize;
    }

    private void Update ()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float velocity = inputX * Speed;
        transform.Translate(Vector2.right * velocity * Time.deltaTime);

        // Screen wraparound system
        if (transform.position.x < -_screenHalfWidthInWorldUnit)
            transform.position = new Vector2(_screenHalfWidthInWorldUnit, transform.position.y);

        if (transform.position.x > _screenHalfWidthInWorldUnit)
            transform.position = new Vector2(-_screenHalfWidthInWorldUnit, transform.position.y);
    }
}
