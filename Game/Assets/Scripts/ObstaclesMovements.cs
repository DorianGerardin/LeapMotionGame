using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesMovements : MonoBehaviour
{

    private Rigidbody body;

    private void Awake() {
        body = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        body.AddForce(0f, 0f, -5f * Time.deltaTime, ForceMode.VelocityChange);
    }
}
