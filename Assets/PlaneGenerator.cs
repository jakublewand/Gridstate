using System.Collections.Generic;
using UnityEngine;

public class PlaneGenerator : MonoBehaviour
{
    public Transform planeTransform;
    public Renderer planeRenderer;
    public BulidingManager BM;

    float minX, maxX, minZ, maxZ;
    int padding = 1;
    private bool planePositionSet = false;
    private const float squareSize = 4f;
    private bool firstPlaneSize = false;
    public void SetPlaneSize(float widthInUnits, float lengthInUnits)
    {   
        if (!planePositionSet)
        {
            planeTransform.position = new Vector3(0.5f, 0f,0.5f);
            planePositionSet = true;
        }
        
        planeTransform.localScale = new Vector3(widthInUnits / 10f, 1f, lengthInUnits / 10f);

        planeRenderer.material.mainTextureScale = new Vector2(widthInUnits / squareSize, lengthInUnits / squareSize);
    }

    public void UpdatePlane()
    {
        Vector3 bounds = CalculateBounds();

        float snappedWidth = Mathf.CeilToInt(bounds.x / squareSize) * squareSize;
        float snappedLength = Mathf.CeilToInt(bounds.z / squareSize) * squareSize;

        SetPlaneSize(snappedWidth, snappedLength);
    }
    public Vector3 CalculateBounds()
    {
        if (BM.buildings.Count == 0)
            return new Vector3(10f, 0f, 10f);
            
        if (!firstPlaneSize)
        {
            firstPlaneSize = true;
            return new Vector3(10f, 0f, 10f);
        }
        List<Building> buildingsLocal = BM.buildings;

        minX = maxX = buildingsLocal[0].location.x;
        minZ = maxZ = buildingsLocal[0].location.y;

        for (int i = 1; i < buildingsLocal.Count; i++)
        {
            // `Building.location` is a Vector2 (x, z) stored as (x, y) on the Vector2
            Vector2 pos = buildingsLocal[i].location;
            minX = Mathf.Min(minX, pos.x);
            maxX = Mathf.Max(maxX, pos.x);
            minZ = Mathf.Min(minZ, pos.y);
            maxZ = Mathf.Max(maxZ, pos.y);

        }

        minX -= padding;
        minZ -= padding;
        maxX += padding;
        maxZ += padding;

        float sizeX = maxX - minX;  
        float sizeZ = maxZ - minZ;

        Vector3 NewVector = new Vector3(sizeX, 0f, sizeZ);
        if (NewVector.x <= new Vector3(10f, 0f, 10f).x || NewVector.z <= new Vector3(10f, 0f, 10f).z)
        {
            return new Vector3(10f, 0f, 10f);
        }else
        {return new Vector3(sizeX, 0f, sizeZ);}
        
    }
}