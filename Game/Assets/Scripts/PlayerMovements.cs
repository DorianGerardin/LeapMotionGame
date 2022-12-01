using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    public float forwardForce = 500;
    public float sidewayForce = 20;
    public float jumpForce = 500;
    public GameObject rightHand;
    public GameObject leftHand;
    public GameObject ufoModel;
    public Transform toRight;
    public Transform toLeft;
    public Transform toTop;
    public Transform toDefault;

    private Rigidbody body;
    private int Zrotation = 0;
    private bool hasJumped = false;
    private bool isFalling = false;
    private float RightThreshold = 20f;
    private float leftThreshold = 15f;
    private Vector3 newRotation;

    public LeapServiceProvider LeapServiceProvider;

    private void OnEnable()
    {
        LeapServiceProvider.OnUpdateFrame += OnUpdateFrame;
    }
    private void OnDisable()
    {
        LeapServiceProvider.OnUpdateFrame -= OnUpdateFrame;
    }

    private void Awake() {
        body = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnUpdateFrame(Frame frame)
    {
        foreach (var hand in frame.Hands)
        {
            if (!hand.IsLeft)
            {
                Vector3 handRotation = hand.Rotation.eulerAngles;
                //Sideway
                Debug.Log(handRotation);
                if(!(handRotation.z >= 360f - RightThreshold || handRotation.z <= leftThreshold)) {
                    if(handRotation.z <= 180) Zrotation = -1; 
                    else Zrotation = 1; 
                } else {
                    Zrotation = 0;
                }
                Debug.Log(Zrotation);

                
            } else {
                Vector3 handRotation = hand.Rotation.eulerAngles;
                //Jump
                if(handRotation.x <= 320f && handRotation.x >= 250f && !isFalling) {
                    hasJumped = true;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate() {

        //body.AddForce(0f, 0f, forwardForce * Time.deltaTime);

        if(rightHand.activeSelf) {
            if(Zrotation == 1) {
                body.AddForce(sidewayForce * Time.deltaTime, 0f, 0f, ForceMode.VelocityChange);
                ufoModel.transform.rotation = Quaternion.Lerp(ufoModel.transform.rotation, toRight.rotation, Time.deltaTime * 5f);
            }
            if(Zrotation == -1) {
                body.AddForce(-sidewayForce * Time.deltaTime, 0f, 0f, ForceMode.VelocityChange);
                ufoModel.transform.rotation = Quaternion.Lerp(ufoModel.transform.rotation, toLeft.rotation, Time.deltaTime * 5f);
            }
            if(Zrotation == 0) {
                ufoModel.transform.rotation = Quaternion.Lerp(ufoModel.transform.rotation, toDefault.rotation, Time.deltaTime * 5f);
            }
        }
        if(hasJumped) {
            body.AddForce(0f, jumpForce * Time.deltaTime, 0f, ForceMode.VelocityChange);
            hasJumped = false;
            isFalling = true;
            body.mass = 0.01f;
            forwardForce = 5f;
        }
    }

    private void OnCollisionEnter(Collision collisionInfo) {
        if(collisionInfo.collider.tag == "Ground" && isFalling) {
            isFalling = false;
            body.mass = 1f;
            forwardForce = 500f;
        }
    }
}
