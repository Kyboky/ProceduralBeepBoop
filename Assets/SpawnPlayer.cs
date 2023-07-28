using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    // Start is called before the first frame update
    void Start()
    {
        MeshGenerator.onLevelLoaded += Spawn;
    }

    private void Spawn(LevelInfo info)
    {
        Instantiate(_player, info.StartingPoint, Quaternion.identity);
    }
}
