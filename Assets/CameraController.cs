using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private void OnEnable()
    {
        MeshGenerator.onLevelLoaded += FocusPlayerWithDelay;
    }



    private void FocusPlayerWithDelay(LevelInfo info)
    {
        StartCoroutine("FocusPlayer", info);
    }

    IEnumerator FocusPlayer(LevelInfo info)
    {
        yield return new WaitForSeconds(3);
        this.transform.DOMove(info.StartingPoint,1);
    }
}
