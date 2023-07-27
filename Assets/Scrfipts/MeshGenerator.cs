using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class MeshGenerator : MonoBehaviour
{
    [SerializeField] private int _meshResolution = 100;
    [SerializeField] private int _terrainSize = 10;
    [SerializeField] private int _randomSeed;
    [SerializeField] private float[,] _terrainArray;
    [SerializeField] private bool _isFlatShaded;

    private PointRadius[] _allTerrainPoints;
    private MeshCollider _meshCollider;
    [SerializeField]  IslandProceduralGeneration islandProceduralGeneration;
    Vector2 minXZ, maxXZ;

    private Mesh _mesh;
    private MeshFilter _meshFilter;

    private Vector3[] _vertices;
    private int[] _triangles;
    int _trisCount;
    int _vertCount;

    private void Awake()
    {
        _meshCollider = GetComponent<MeshCollider>();
        _meshFilter = GetComponent<MeshFilter>();
    }

    public void Generate()
    {
        minXZ = Vector2.one * Mathf.Infinity; 
        maxXZ = Vector2.one * (-Mathf.Infinity);
        _mesh = new Mesh(); 
        _vertices = new Vector3[_meshResolution * _meshResolution];
        _triangles = new int[(_meshResolution-1) * (_meshResolution - 1) * 6];
        _terrainArray = new float[_meshResolution, _meshResolution];
        GenerateTerrain();

    }


    private void GenerateTerrain()
    {        
        _allTerrainPoints = islandProceduralGeneration.GenerateLevelSkeleton(_randomSeed).ToArray();
        ArrayFromPointsAndRadius();
        ArrayAddPerlinNoise();
        GenerateMesh(_meshResolution, _terrainSize);
        _meshFilter.sharedMesh = _mesh;       
    }

    private void GenerateMesh(int resolution, float size)
    {
        Vector2[] uv = new Vector2[_vertices.Length];

        float distanceBetweenPoints = (float)size / (float)(resolution - 1);
        _trisCount = 0;
        _vertCount = 0;

        for (int i = 0; i < _meshResolution; i++)
        {
            for (int j = 0; j < _meshResolution; j++)
            {
                _vertices[_vertCount] = new Vector3(j * distanceBetweenPoints, _terrainArray[i,j], i * distanceBetweenPoints);
                uv[_vertCount] = new Vector2((float)j / _meshResolution, (float)i / _meshResolution);
                _vertCount++;
            }
        }
        _vertCount = 0;

        for (int i = 0; i < _meshResolution - 1; i++)
        {
            for (int j = 0; j < _meshResolution - 1; j++)
            {
                _triangles[_trisCount] = _vertCount;
                _triangles[_trisCount + 1] = _vertCount + _meshResolution-1 + 1;
                _triangles[_trisCount + 2] = _vertCount + 1;
                _triangles[_trisCount + 3] = _vertCount + 1;
                _triangles[_trisCount + 4] = _vertCount + _meshResolution-1 + 1;
                _triangles[_trisCount + 5] = _vertCount +_meshResolution -1+ 2;
                _trisCount += 6;
                _vertCount++;
            }
            _vertCount++;
        }

        _mesh.Clear();
        
        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;
        _mesh.uv = uv;
        if (_isFlatShaded)
        {
            CustomMeshUtility.MeshtoFlatShaded(ref _mesh);
        }
        _mesh.RecalculateNormals();
        _meshCollider.sharedMesh = _mesh;
    }

    private void ArrayFromPointsAndRadius()
    {
        GetMinMax(ref minXZ, ref maxXZ, _allTerrainPoints);
        Vector2 dir = maxXZ - minXZ;
        minXZ -= 0.2f * dir;
        maxXZ += 0.2f * dir;
        float xDiff = maxXZ.x - minXZ.x;
        float zDiff = maxXZ.y - minXZ.y;
        float pixelDistance = Mathf.Max(xDiff, zDiff) / _meshResolution;
        bool xGreater = xDiff > zDiff;

        foreach (PointRadius pointRadius in _allTerrainPoints)
        {
            int x = (int)Mathf.Floor((float)(pointRadius.Center.x + (xGreater ? 0 : (zDiff - xDiff) / 2) - minXZ.x) / (float)pixelDistance);
            
            int z = (int)Mathf.Floor((float)(pointRadius.Center.z + (xGreater ? (xDiff-zDiff)/2 : 0) - minXZ.y) / (float)pixelDistance);
            float radiusInPoints = pointRadius.Radius / pixelDistance;
            int radiusInPointsInt = (int)Mathf.Floor(radiusInPoints);

            for (int i = x - radiusInPointsInt; i < x + radiusInPoints; i++)
            {
                for (int j = z - radiusInPointsInt; j < z + radiusInPoints; j++)
                {
                    if (Vector2.Distance(new Vector2(x, z), new Vector2(i, j)) < radiusInPoints)
                    {
                        try
                        {
                            _terrainArray[i, j] += 0.3f;
                        }
                        catch {

                            Debug.Log("WTF");

                        }
                        _terrainArray[i, j] = Mathf.Clamp(_terrainArray[i, j],0, 0.3f);
                    }
                        
                }
            }
        }
    }

    private void ArrayAddPerlinNoise()
    {
        float xOffset = Random.Range(0, 100);
        float yOffset = Random.Range(0, 100);
        for (int i = 10; i < _meshResolution-10; i++)
        {
            for (int j = 10; j < _meshResolution-10; j++)
            {
                float perlin = Mathf.PerlinNoise(xOffset + i*0.05f * 200/_meshResolution, yOffset + j * 0.05f * 200 / _meshResolution) * 0.5f;
                perlin = perlin > 0.3f ? 0.5f:0f;
                _terrainArray[i, j] += perlin;

                _terrainArray[i, j] = Mathf.Clamp(_terrainArray[i, j], 0, 0.3f);
                _terrainArray[i, j] += Random.Range(-0.01f, 0.01f);
            }
        }
    }

    private void GetMinMax(ref Vector2 minXZ, ref Vector2 maxXZ, PointRadius[] terrainPoints)
    {
        foreach (PointRadius point in terrainPoints)
        {
            if (point.Center.x > maxXZ.x)
            {
                maxXZ.x = point.Center.x;
            }
            if (point.Center.z > maxXZ.y)
            {
                maxXZ.y = point.Center.z;
            }
            if (point.Center.x < minXZ.x)
            {
                minXZ.x = point.Center.x;
            }
            if (point.Center.z < minXZ.y)
            {
                minXZ.y = point.Center.z;
            }
        }
    }
}
