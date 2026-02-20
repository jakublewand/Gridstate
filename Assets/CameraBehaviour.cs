using UnityEngine;
using UnityEngine.EventSystems;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private float planeY = 0f;
    [SerializeField] private float maxMovement = 50f;
    private bool panning;
    [SerializeField] private Vector3 lastMouseScreenPos;

    [SerializeField] private float topDownAngle = 90f;
    [SerializeField] private float angledAngle = 45f;
    [SerializeField] private float perspectiveDuration = 0.5f;

    private bool isChangingPerspective;
    private bool isAngled;
    private float perspectiveT;
    private float lastSmoothT;
    private Quaternion rotFrom, rotTo;
    private Vector3 offsetFrom, offsetTo;

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
        if (Input.mouseScrollDelta.y != 0 && !EventSystem.current.IsPointerOverGameObject())
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

        if (panning)
        {
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

        if (isChangingPerspective)
        {
            perspectiveT += Time.deltaTime / perspectiveDuration;
            if (perspectiveT >= 1f)
            {
                perspectiveT = 1f;
                isChangingPerspective = false;
            }

            float smoothT = Mathf.SmoothStep(0f, 1f, perspectiveT);

            // Slerp to arc around the focal point instead of cutting through
            Vector3 prevOffset = Vector3.Slerp(offsetFrom, offsetTo, lastSmoothT);
            Vector3 currOffset = Vector3.Slerp(offsetFrom, offsetTo, smoothT);
            transform.position += currOffset - prevOffset;

            // Rotation can be set directly (panning doesn't affect it)
            transform.rotation = Quaternion.Slerp(rotFrom, rotTo, smoothT);

            lastSmoothT = smoothT;
        }
    }

    public void changePerspective()
    {
        if (TryGetMousePlanePoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f), out Vector3 focalPoint))
        {
            float yRot = transform.eulerAngles.y;

            bool goingToAngled = !isAngled;
            float targetXAngle = goingToAngled ? angledAngle : topDownAngle;

            offsetFrom = transform.position - focalPoint;
            float dist = offsetFrom.magnitude;

            float targetRad = targetXAngle * Mathf.Deg2Rad;
            Vector3 forward = Quaternion.Euler(0f, yRot, 0f) * Vector3.forward;
            offsetTo = new Vector3(0f, dist * Mathf.Sin(targetRad), 0f) - forward * (dist * Mathf.Cos(targetRad));

            rotFrom = transform.rotation;
            rotTo = Quaternion.Euler(targetXAngle, yRot, 0f);

            perspectiveT = 0f;
            lastSmoothT = 0f;
            isChangingPerspective = true;
            isAngled = goingToAngled;
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
