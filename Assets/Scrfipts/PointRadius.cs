using UnityEngine;
[System.Serializable]
public struct PointRadius
{
    public Vector3 Center;
    public float Radius;

    public PointRadius(Vector3 center, float radius)
    {
        Center = center;
        Radius = radius;
    }
}
