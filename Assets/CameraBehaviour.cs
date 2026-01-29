using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    private Vector3 dragOrigin;
    private Vector3 cameraOrigin;
    [SerializeField] float dragSpeed = 0.05f;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            cameraOrigin = transform.position;
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0))
            return;


        Vector3 distance = dragOrigin - Input.mousePosition;
        Vector3 move = new Vector3(distance.x * dragSpeed, 0, distance.y * dragSpeed);

        transform.position = cameraOrigin + move;
    }
}