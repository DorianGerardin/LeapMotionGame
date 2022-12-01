using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMovement : MonoBehaviour
{
    public float forwardForce;

    private Rigidbody body;
    private Vector3 newPosition;

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
        if(transform.position.z <= -70f) {
            Destroy(gameObject, 0f);
        }
    }

    private void FixedUpdate() {
        body.velocity = new Vector3(0f, 0f, 0f);
        body.AddForce(0f, 0f, -forwardForce * Time.deltaTime, ForceMode.VelocityChange);
    }
}
