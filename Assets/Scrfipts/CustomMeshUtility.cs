using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomMeshUtility
{
    public static void MeshtoFlatShaded(ref Mesh newMesh)
    {
        Vector3[] vertices = newMesh.vertices;
        int[] triangles = newMesh.triangles;
        List<Vector2> newUV = new List<Vector2>();
        List<Vector3> newVertices = new List<Vector3>();
        int[] newTriangles = new int[triangles.Length];
        for(int i = 0; i < newTriangles.Length; i++)
        {
            newTriangles[i] = i;
            newVertices.Add(vertices[triangles[i]]);
            newUV.Add(newMesh.uv[triangles[i]]);
        }
        newMesh.vertices = newVertices.ToArray(); 
        newMesh.triangles = newTriangles;
        newMesh.uv = newUV.ToArray();
    }
}
