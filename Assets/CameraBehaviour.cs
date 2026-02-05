using UnityEngine;
using System.Collections.Generic;

public class CameraBehaviour : MonoBehaviour
{
    private const float maxMovement = 20f;
    private Vector3 dragOrigin;
    private Vector3 cameraOrigin;
    [SerializeField] float dragSpeed = 0.002f;
    void Update()
    {
        RaycastHit hit;
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 fwd = transform.TransformDirection(Vector3.forward);
			if (Physics.Raycast(transform.position + new Vector3(0, 0, 1), fwd, out hit, 100))
            {
                Debug.Log("Hit " + hit.collider.gameObject.name);
            }
		}
    
        if (Input.GetMouseButtonDown(0))
        {
            cameraOrigin = transform.position;
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        Vector3 distance = dragOrigin - Input.mousePosition;
        Vector3 move = new Vector3(distance.x * dragSpeed, 0, distance.y * dragSpeed);

        transform.position = cameraOrigin + move;
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -maxMovement, maxMovement),
            transform.position.y,
            Mathf.Clamp(transform.position.z, -maxMovement, maxMovement)
        );
    }
    
}