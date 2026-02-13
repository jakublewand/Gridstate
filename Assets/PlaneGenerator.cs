using System.Collections.Generic;
using UnityEngine;

public class PlaneGenerator : MonoBehaviour
{
    public Transform planeTransform;
    public Renderer planeRenderer;
    public BulidingManager BM;

    float minX, maxX, minZ, maxZ;
    int padding = 5;

    public void SetPlaneSize(float widthInUnits, float lengthInUnits)
    {
        float scaleX = widthInUnits / 10f;
        float scaleZ = lengthInUnits / 10f;
        float centerX = Mathf.Floor((minX + maxX) / 2f);
        float centerZ = Mathf.Floor((minZ + maxZ) / 2f);

        planeTransform.position = new Vector3(centerX, 0f, centerZ);

        planeTransform.localScale = new Vector3(scaleX, 1f, scaleZ);


        planeRenderer.material.mainTextureScale =
            new Vector2(scaleX, scaleZ);
    }

    public void UpdatePlane()
    {
        Vector3 bounds = CalculateBounds();
        SetPlaneSize(bounds.x, bounds.z);
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

        float sizeX = (maxX - minX) +  padding;
        float sizeZ = (maxZ - minZ) +  padding;




        return new Vector3(sizeX, 0f, sizeZ);
    }
}
