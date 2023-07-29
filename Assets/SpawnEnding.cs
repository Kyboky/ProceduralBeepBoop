using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnding : MonoBehaviour
{
    [SerializeField] private GameObject _ending;
    // Start is called before the first frame update
    void Start()
    {
        MeshGenerator.onLevelLoaded += Spawn;
    }

    private void Spawn(LevelInfo info)
    {
        Instantiate(_ending, info.EndingPoint + Vector3.up*0.05f, Quaternion.identity);
    }
}
