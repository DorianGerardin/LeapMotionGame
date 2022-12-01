using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public PlayerMovements playerMvt;
    public GroundMovement groundMvt;
    public ObstaclesMovements obstacleMvt;

    //public Rigidbody obstaclesBodies;

    private void OnCollisionEnter(Collision collisionInfo) {
        if(collisionInfo.collider.tag == "Obstacle") {
            playerMvt.enabled = false;
            groundMvt.enabled = false;
            //obstacleMvt.enabled = false;
            //obstaclesBodies.velocity = new Vector3(0f, 0f, 0f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
