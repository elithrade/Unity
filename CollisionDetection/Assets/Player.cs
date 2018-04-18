using UnityEngine;

public class Player : MonoBehaviour
{
    public float Speed = 6;
    private Rigidbody _rigidbody;
    private Vector3 _velocity;
    private int _coinCount;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update ()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 direction = input.normalized;
        _velocity = direction * Speed;
    }

    private void FixedUpdate()
    {
        // FixedUpdate is called on a fixed interval
        // this ensures the physics calculations are
        // still processed even if for some reason
        // framerate is dropped.

        // Alternatively Time.deltaTime can be replaced with Time.fixedDeltaTime
        // since we are called on a fixed update rate, they are the same
        // unless we change the fixed update interval for slow motion effect etc..
        _rigidbody.position += _velocity * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider collider)
    {
        // OnTriggerEnter is called when player
        // collide with other game objects which
        // has IsTrigger set on their box collider.

        // For collider to work one of the game object
        // must have Rigidbody attached.
        if (collider.gameObject.tag.Equals("Coin"))
        {
            Destroy(collider.gameObject);
            _coinCount++;
            print(string.Format("{0} coins collected!", _coinCount));
        }
    }
}
