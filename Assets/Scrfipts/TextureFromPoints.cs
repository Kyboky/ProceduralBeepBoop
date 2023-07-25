using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureFromPoints : MonoBehaviour
{
    [SerializeField] private float[,] _terrainArray = new float[100,100];
    [SerializeField] private int _textureSize; 
    
    public void GenerateTexture(Vector2 minXZ, Vector2 maxXZ, List<PointRadius> _allTerrainPoints)
    {
        Vector2 dir = maxXZ - minXZ;
        minXZ -= 0.1f * dir;
        maxXZ += 0.1f * dir;

        float xDiff = maxXZ.x - minXZ.x;
        float zDiff = maxXZ.y - minXZ.y;
        float pixelDistance = Mathf.Max(xDiff, zDiff) / 100; 

        foreach(PointRadius pointRadius in _allTerrainPoints)
        {
            int x = (int)Mathf.Floor((float)(pointRadius.Center.x - minXZ.x) / (float)pixelDistance);
            int z = (int)Mathf.Floor((float)(pointRadius.Center.z - minXZ.y) / (float)pixelDistance);

            _terrainArray[x, z] = 1;

        }

        for(int i = 0; i<100; i++)
        {
            for(int j = 0; j < 100; j++)
            {
                Debug.Log(_terrainArray[i, j]);
            }
        }
    }
}
