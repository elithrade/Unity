using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    public float Speed = 7;
    
    void Update ()
    {
        transform.Translate(Vector2.down * Speed * Time.deltaTime);
    }
}
