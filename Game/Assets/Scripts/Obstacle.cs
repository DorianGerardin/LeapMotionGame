using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    

    private Vector3 _initialPosition;
    public Vector3 InitialPosition => _initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        _initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
