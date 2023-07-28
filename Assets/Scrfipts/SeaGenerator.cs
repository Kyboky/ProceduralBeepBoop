using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SeaGenerator : MonoBehaviour
{

    [SerializeField] private bool _isFlatShaded;
    [SerializeField] private int _resolution;
    [SerializeField] private int _size;
    Mesh _mesh;
    MeshFilter _meshFilter;
    void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _mesh = new Mesh();
        GenerateMesh(100,20);
        _meshFilter.sharedMesh = _mesh;
    }
    
    private void GenerateMesh(int resolution, float size)
    {
        
        
        var _vertices = new Vector3[resolution * resolution];
        var _triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        Vector2[] uv = new Vector2[resolution * resolution];

        float distanceBetweenPoints = (float)size / (float)(resolution - 1);
        var _trisCount = 0;
        var _vertCount = 0;

        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                _vertices[_vertCount] = new Vector3( -5 +j * distanceBetweenPoints, 0.2f,-5 + i * distanceBetweenPoints);
                uv[_vertCount] = new Vector2((float)j / resolution, (float)i / resolution);
                _vertCount++;
            }
        }
        _vertCount = 0;

        for (int i = 0; i < resolution - 1; i++)
        {
            for (int j = 0; j < resolution - 1; j++)
            {
                _triangles[_trisCount] = _vertCount;
                _triangles[_trisCount + 1] = _vertCount + resolution - 1 + 1;
                _triangles[_trisCount + 2] = _vertCount + 1;
                _triangles[_trisCount + 3] = _vertCount + 1;
                _triangles[_trisCount + 4] = _vertCount + resolution - 1 + 1;
                _triangles[_trisCount + 5] = _vertCount + resolution - 1 + 2;
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
    }
}
