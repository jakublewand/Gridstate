using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CameraBehaviour : MonoBehaviour
{
    private const float maxMovement = 20f;
    private Vector3 dragOrigin;
    private Vector3 cameraOrigin;
    private bool panning;
    [SerializeField] float dragSpeed = 0.002f;
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            cameraOrigin = transform.position;
            dragOrigin = Input.mousePosition;
            panning = true;
            return;
        }

        if (!Input.GetMouseButton(0))
        {
            panning = false;
        }
        if (!panning)
            return;

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