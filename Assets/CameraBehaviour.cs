using UnityEngine;
using UnityEngine.EventSystems;

public class CameraBehaviour : MonoBehaviour
{   
    [SerializeField] private float planeY = 0f;
    [SerializeField] private float maxMovement = 50f;
    private bool panning;
    private Vector3 lastMouseScreenPos;

    public void Zoom(float direction)
    {
        var cam = Camera.main;
        if (cam == null) return;

        float step = 2f;
        float minY = 5f;
        float maxY = 30f;

        Vector3 pos = cam.transform.position;
        pos.y = Mathf.Clamp(pos.y + direction * step, minY, maxY);
        cam.transform.position = pos;
    }

    void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            Zoom(-Input.mouseScrollDelta.y);
        }

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            panning = true;
            lastMouseScreenPos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            panning = false;
        }

        if (!panning) return;

        if (TryGetMousePlanePoint(lastMouseScreenPos, out Vector3 previousHitPoint) &&
            TryGetMousePlanePoint(Input.mousePosition, out Vector3 currentHitPoint))
        {
            Vector3 delta = previousHitPoint - currentHitPoint;
            Vector3 pos = transform.position + delta;

            pos.x = Mathf.Clamp(pos.x, -maxMovement, maxMovement);
            pos.z = Mathf.Clamp(pos.z, -maxMovement, maxMovement);

            transform.position = pos;
            lastMouseScreenPos = Input.mousePosition;
        }
    }
    private bool TryGetMousePlanePoint(Vector3 screenPosition, out Vector3 worldPoint)
    {
        var cam = Camera.main;
        if (cam == null)
        {
            worldPoint = default;
            return false;
        }

        Ray ray = cam.ScreenPointToRay(screenPosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0f, planeY, 0f));

        if (plane.Raycast(ray, out float t))
        {
            worldPoint = ray.GetPoint(t);
            return true;
        }

        worldPoint = default;
        return false;
    }
    
}
