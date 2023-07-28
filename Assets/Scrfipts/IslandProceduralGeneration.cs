using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class IslandProceduralGeneration : MonoBehaviour
{
    [SerializeField] public int _numberOfPoints = 10;
    [SerializeField] private float _secondarySpawnChanceNewBranch = 0.9f;
    [SerializeField] private float _secondarySpawnRandomMultiplier = 0.2f;
    [SerializeField] private float _secondaryBranchLength = 5;
    [SerializeField] private Vector2 _minMaxRadius;
    [SerializeField] private float _stageDistance = 60;

    public List<PointRadius> _allTerrainPoints;

    public List<PointRadius> GenerateLevelSkeleton(int seed)
    {
        Random.InitState(seed);
        _allTerrainPoints = new List<PointRadius>();
        Vector3[] positions = GenerateRandomStartAndEndPoints(Vector3.zero, _stageDistance);
        PointRadius[] points = GenerateSplinePoints(positions, _numberOfPoints);

        AddPointsToList(points, ref _allTerrainPoints);

        float additiveChance = 0;

        for (int i = 0; i < _numberOfPoints - 1; i++)
        {
            additiveChance += Random.Range(0,1f) * _secondarySpawnRandomMultiplier;
            if (additiveChance > _secondarySpawnChanceNewBranch)
            {

                Vector3[] newBranchPoints = GenerateRandomStartAndEndPoints(points[i].Center, _stageDistance / 3 + Random.Range(-5, 15f));
                PointRadius[] tempPoints = GenerateSplinePoints(newBranchPoints, (int)(_numberOfPoints / 2));
                AddPointsToList(tempPoints, ref _allTerrainPoints);
                additiveChance = 0;
            }
        }

        for (int i = 0; i < _allTerrainPoints.Count; i++)
        {
            float randomRadius = _minMaxRadius.x + (_minMaxRadius.y - _minMaxRadius.x) * Random.Range(0,1f);
            //Gizmos.DrawSphere(_allTerrainPoints[i].Center, _allTerrainPoints[i].Radius);
        }
        return _allTerrainPoints;
    }

    

    private Vector3[] GenerateRandomStartAndEndPoints(Vector3 startPoint, float distance)
    {
        Vector3[] newBranchPoints = new Vector3[4];
        Vector3 endDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        Vector3 forward = startPoint + endDirection.normalized*distance - startPoint;
        Vector3 direction = Vector3.Cross(forward.normalized, Vector3.up * Mathf.Sign(Random.value - 0.5f));
        newBranchPoints[0] = startPoint;
        newBranchPoints[1] = startPoint + direction * distance/2;
        newBranchPoints[3] = startPoint + forward;
        newBranchPoints[2] = newBranchPoints[3] + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * _secondaryBranchLength;
        return newBranchPoints;
    }

    private void AddPointsToList(PointRadius[] points, ref List<PointRadius> list)
    {
        foreach(PointRadius point in points)
        {
            list.Add(point);
        }
    }

    private PointRadius[] GenerateSplinePoints(Vector3[] positions , int numOfPoints)
    {
        PointRadius[] points = new PointRadius[numOfPoints];
        for (int i = 0; i <= numOfPoints - 1; i++)
        {
            points[i] = GetSplinePoint(positions, (float)i / (numOfPoints - 1));
        }

        return points;
    }

    PointRadius GetSplinePoint(Vector3[] points, float t)
    {
        if(points.Length == 1) return new PointRadius(points[0], GetRadius(1,t));
        Vector3[] newPoints = new Vector3[points.Length - 1];
        for (int i = 0;i < newPoints.Length;i++) 
        {
            newPoints[i] = points[i] + (points[i + 1] - points[i]) * t;
        }
        return GetSplinePoint(newPoints, t);
    }

    float GetRadius(float r, float t)
    {
        return _minMaxRadius.x + (_minMaxRadius.y-_minMaxRadius.x) * Mathf.PerlinNoise(r, t*5);
    }

}
