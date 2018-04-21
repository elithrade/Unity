using UnityEngine;

public class Test01 : MonoBehaviour
{
    public LayerMask Mask;

    void Update ()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        float maxDistance = 100;

        // Mask can be created and specified on the object we want to collide with
        // Note that the object needs to have a collider attached
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, maxDistance, Mask))
        {
            // Collider is a very useful property we can filter
            // hit info based on which object the ray hits and
            // we can do what ever we want with the hitted object
            print(hitInfo.collider.gameObject.name);
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
        }
        else
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * maxDistance, Color.green);

    }
}
