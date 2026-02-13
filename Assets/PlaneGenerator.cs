using System.Collections.Generic;
using UnityEngine;

public class PlaneGenerator : MonoBehaviour
{
    public Transform planeTransform;
    public Renderer planeRenderer;
    public BulidingManager BM;

    float minX, maxX, minZ, maxZ;
    int padding = 5;
    private bool planePositionSet = false;
    private const float squareSize = 0.9f;
    private Vector2 planeOrigin = new Vector2(-0.5f, -0.5f);
    private float currentWorldWidth = 0f;
    private float currentWorldLength = 0f;
    public void SetPlaneSize(float widthInUnits, float lengthInUnits)
    {   
        if (!planePositionSet)
        {
            planeTransform.position = new Vector3(-0.5f, 0f, -0.5f);
            planePositionSet = true;
        }
        float worldWidth = widthInUnits;
        float worldLength = lengthInUnits;

        planeTransform.localScale = new Vector3(worldWidth / 10f, 1f, worldLength / 10f);
        planeTransform.position = new Vector3(planeOrigin.x + worldWidth / 2f, 0f, planeOrigin.y + worldLength / 2f);
        planeRenderer.material.mainTextureScale = new Vector2(worldWidth / squareSize, worldLength / squareSize);

        currentWorldWidth = worldWidth;
        currentWorldLength = worldLength;
    }

    public void UpdatePlane()
    {
        Vector3 bounds = CalculateBounds();

        float snappedWidth = Mathf.Ceil(bounds.x / squareSize) * squareSize;
        float snappedLength = Mathf.Ceil(bounds.z / squareSize) * squareSize;

        float boundsMinX = minX;
        float boundsMaxX = maxX;
        float boundsMinZ = minZ;
        float boundsMaxZ = maxZ;

        // X: expand only on side where buildings exceed current plane
        if (currentWorldWidth <= 0f)
        {
            planeOrigin.x = boundsMinX;
        }
        else if (boundsMaxX > planeOrigin.x + currentWorldWidth && boundsMinX >= planeOrigin.x)
        {
            // expand right only: origin.x stays
        }
        else if (boundsMinX < planeOrigin.x && boundsMaxX <= planeOrigin.x + currentWorldWidth)
        {
            // expand left only
            planeOrigin.x = planeOrigin.x - (snappedWidth - currentWorldWidth);
        }
        else if (boundsMinX < planeOrigin.x && boundsMaxX > planeOrigin.x + currentWorldWidth)
        {
            // needs both sides: align to boundsMinX
            planeOrigin.x = boundsMinX;
        }

        // Z: similar
        if (currentWorldLength <= 0f)
        {
            planeOrigin.y = boundsMinZ;
        }
        else if (boundsMaxZ > planeOrigin.y + currentWorldLength && boundsMinZ >= planeOrigin.y)
        {
            // expand top only
        }
        else if (boundsMinZ < planeOrigin.y && boundsMaxZ <= planeOrigin.y + currentWorldLength)
        {
            planeOrigin.y = planeOrigin.y - (snappedLength - currentWorldLength);
        }
        else if (boundsMinZ < planeOrigin.y && boundsMaxZ > planeOrigin.y + currentWorldLength)
        {
            planeOrigin.y = boundsMinZ;
        }

        SetPlaneSize(snappedWidth, snappedLength);
    }

    public Vector3 CalculateBounds()
    {
        
        if (BM.buildings.Count == 0)
            return new Vector3(10f, 0f, 10f);

        List<Building> buildingsLocal = BM.buildings;

        minX = maxX = buildingsLocal[0].location.x;
        minZ = maxZ = buildingsLocal[0].location.z;

        for (int i = 1; i < buildingsLocal.Count; i++)
        {
            Vector3 pos = buildingsLocal[i].location;
            minX = Mathf.Min(minX, pos.x);
            maxX = Mathf.Max(maxX, pos.x);
            minZ = Mathf.Min(minZ, pos.z);
            maxZ = Mathf.Max(maxZ, pos.z);
            print(buildingsLocal[i].location.x);
            print(buildingsLocal[i].location.y);
        }

        minX -= padding;
        minZ -= padding;
        maxX += padding;
        maxZ += padding;

        float sizeX = (maxX - minX);
        float sizeZ = (maxZ - minZ);




        return new Vector3(sizeX, 0f, sizeZ);
    }
}
