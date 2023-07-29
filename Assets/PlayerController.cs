using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    private Rigidbody _rb;
    [SerializeField] private float _movementInertion;
    [SerializeField] private float _movementSpeed;

    [SerializeField] private float _perlinNoiseSpeed;

    [SerializeField] private Transform _yTransform;
    [SerializeField] private Transform _xzTransform;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        Vector3 directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (directionVector.magnitude >= 1)
        {
            directionVector = directionVector.normalized;
        }
        transform.position += Vector3.Lerp(_rb.velocity, directionVector , _movementInertion) * Time.deltaTime * _movementSpeed;
        transform.position = new Vector3(transform.position.x, GetTerrainPoint()+Mathf.PerlinNoise(Time.time * _perlinNoiseSpeed, Time.time * _perlinNoiseSpeed)*0.03f, transform.position.z);
        if (directionVector != Vector3.zero) 
        {
            Quaternion yRot = Quaternion.Lerp(_yTransform.rotation,Quaternion.LookRotation(directionVector, Vector3.up),0.1f);
            _yTransform.rotation = yRot; 
        }
        
    }

    private float GetTerrainPoint()
    {
        RaycastHit hit;
        if(Physics.Raycast(_xzTransform.position, Vector3.down,out hit,1))
        {
            if (hit.transform.CompareTag("Terrain"))
            {
                return hit.point.y;
            }
        }
        return 1;
    }
}
