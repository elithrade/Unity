using UnityEngine;

public class Test02 : MonoBehaviour
{
    public Transform ObjectToPlace;
    public Camera GameCamera;
    
    void Update ()
    {
        Ray ray = GameCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            ObjectToPlace.position = hitInfo.point;
            // Create rotation from the up to direction of the surface normal
            ObjectToPlace.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
        }
    }
}
