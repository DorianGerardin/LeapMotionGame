using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float sidewayForce;
    public float jumpForce;
    public GameObject rightHand;
    public GameObject leftHand;
    public GameObject ufoModel;
    public Transform toRight;
    public Transform toLeft;
    public Transform toTop;
    public Transform toDefault;
    public PlayerCollision playerCollision;

    private Rigidbody _body;
    public Rigidbody GetBody => _body;

    private int _rotationZ;
    private int _lastRotationZ;
    private bool _hasJumped;
    private bool _isFalling;
    public bool IsFalling => _isFalling;
    private float _rightThreshold = 20f;
    private float _leftThreshold = 15f;
    private Vector3 _initialScale;
    
    //Power Ups
    public bool hasPowerUpToUse;
    private bool _canJump;
    private bool _canScaleDown;
    private bool _canBeInvincible;
    private bool _canDestructObstacles;
    private bool _areControlsInversed;

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
        _body = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _initialScale = transform.localScale;
        _canJump = false;
        _canScaleDown = false;
        _canBeInvincible = false;
        _canDestructObstacles = false;
        _areControlsInversed = false;
    }

    void OnUpdateFrame(Frame frame)
    {
        foreach (var hand in frame.Hands)
        {
            if (!hand.IsLeft)
            {
                Vector3 handRotation = hand.Rotation.eulerAngles;
                //Sideway
                if(!(handRotation.z >= 360f - _rightThreshold || handRotation.z <= _leftThreshold)) {
                    if(handRotation.z <= 180) _rotationZ = -1; 
                    else _rotationZ = 1; 
                } else {
                    _rotationZ = 0;
                }
            } else
            {
                Vector3 handRotation = hand.Rotation.eulerAngles;
                //Jump
                if((handRotation.x is <= 320f and >= 250f || hand.PalmPosition.y >= 1.9f) && _canJump) {
                    _hasJumped = true;
                }
                
                //Scale down
                if (_canScaleDown && hand.IsPinching())
                {
                    hasPowerUpToUse = true;
                    //StartCoroutine(ScaleDownCoroutine());
                    StartCoroutine(ScaleDownAndUpAfterDelay(2f));
                    _canScaleDown = false;
                }
                
                //Destruct Obstacles
                if (_canDestructObstacles && hand.GrabStrength >= 0.99)
                {
                    hasPowerUpToUse = true;
                    float sphereRadius = 100f;
                    //StartCoroutine(Explosion(sphereRadius));
                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereRadius);
                    foreach (var hitCollider in hitColliders) {
                        if (hitCollider.CompareTag("Obstacle")) {
                            hitCollider.AddComponent<Rigidbody>();
                            Vector3 direction = hitCollider.transform.position - transform.position;
                            float force = 3f;
                            hitCollider.GetComponent<Rigidbody>().AddForce(direction.x * 4f * force, (direction.y + 15f) * force, direction.z * force, ForceMode.Impulse);
                            hitCollider.GetComponent<Rigidbody>().AddTorque(direction.x * force,15f, 5f, ForceMode.Impulse);
                        }
                    }
                    _canDestructObstacles = false;
                    hasPowerUpToUse = false;
                }
                
                //Invincibility
                if (_canBeInvincible)
                {
                    StartCoroutine(RemoveCollisionForADuration(7f));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate() {

        if(rightHand.activeSelf) {
            switch (_rotationZ)
            {
                case 1:
                    if (_lastRotationZ == -1 && !_isFalling) {
                        _body.velocity = Vector3.Lerp(_body.velocity, Vector3.zero, Time.deltaTime * 27f);
                    }
                    if (_areControlsInversed) {
                        _body.AddForce(-sidewayForce * Time.deltaTime, 0f, 0f, ForceMode.VelocityChange);
                        ufoModel.transform.rotation = Quaternion.Lerp(ufoModel.transform.rotation, toLeft.rotation, Time.deltaTime * 5f);
                    }
                    else {
                        _body.AddForce(sidewayForce * Time.deltaTime, 0f, 0f, ForceMode.VelocityChange);
                        ufoModel.transform.rotation = Quaternion.Lerp(ufoModel.transform.rotation, toRight.rotation, Time.deltaTime * 5f);
                    }
                    _lastRotationZ = 1;
                    break;
                case -1:
                    if (_lastRotationZ == 1 && !_isFalling) {
                        _body.velocity = Vector3.Lerp(_body.velocity, Vector3.zero, Time.deltaTime * 27f);
                    }
                    if (_areControlsInversed) {
                        _body.AddForce(sidewayForce * Time.deltaTime, 0f, 0f, ForceMode.VelocityChange);
                        ufoModel.transform.rotation = Quaternion.Lerp(ufoModel.transform.rotation, toRight.rotation, Time.deltaTime * 5f);
                    }
                    else {
                        _body.AddForce(-sidewayForce * Time.deltaTime, 0f, 0f, ForceMode.VelocityChange);
                        ufoModel.transform.rotation = Quaternion.Lerp(ufoModel.transform.rotation, toLeft.rotation, Time.deltaTime * 5f);
                    }
                    _lastRotationZ = -1;
                    break;
                case 0:
                    ufoModel.transform.rotation = Quaternion.Lerp(ufoModel.transform.rotation, toDefault.rotation, Time.deltaTime * 5f);
                    _body.velocity = Vector3.Lerp(_body.velocity, new Vector3(0f, _body.velocity.y, _body.velocity.z), Time.deltaTime * 12f);
                    break;
            }
        }
        else {
            if (_body.velocity.y >= -2.5f && !_isFalling) {
                ufoModel.transform.rotation = Quaternion.Lerp(ufoModel.transform.rotation, toDefault.rotation, Time.deltaTime * 5f);
                _body.velocity = Vector3.zero;
            }
        }
        if(_hasJumped) {
            _body.AddForce(0f, jumpForce * Time.deltaTime, 0f, ForceMode.VelocityChange);
            _canJump = false;
            _hasJumped = false;
            _isFalling = true;
        }
    }

    private void OnCollisionEnter(Collision collisionInfo) {
        if(collisionInfo.collider.CompareTag("Ground")) {
            if (_isFalling) {
                _isFalling = false;
                hasPowerUpToUse = false;
            }
        }
    }

    public void Jump()
    {
        Debug.Log("Jump");
        _canJump = true;
        hasPowerUpToUse = true;
    }

    public void ScaleDown()
    {
        Debug.Log("Scaledown");
        _canScaleDown = true;
        hasPowerUpToUse = true;
    }

    public void Invincible()
    {
        Debug.Log("Invicible");
        _canBeInvincible = true;
        hasPowerUpToUse = true;
    }

    public void DestructObstacles(float radius)
    {
        Debug.Log("DestructObstacle");
        _canDestructObstacles = true;
        hasPowerUpToUse = true;
    }

    public void InverseControls(float duration, float delay)
    {
        StartCoroutine(HandleControlsInversion(delay, duration));
        Debug.Log("InverseControls");
    }

    IEnumerator ScaleDownAndUpAfterDelay(float delay)
    {
        float epsilon = 0.01f;
        float scaledownSpeed = 5f;
        Vector3 newScale = _initialScale / 4f;
        while (transform.localScale.x > newScale.x)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, newScale, scaledownSpeed * Time.deltaTime);
            if (Mathf.Abs(transform.localScale.x - newScale.x) <= epsilon)
            {
                transform.localScale = newScale;
            }
            yield return null;
        }
        
        yield return new WaitForSeconds(delay);
        
        hasPowerUpToUse = false;
        float scaleUpSpeed = 5f;
        while (transform.localScale.x < _initialScale.x)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _initialScale, scaleUpSpeed * Time.deltaTime);
            if (Mathf.Abs(transform.localScale.x - _initialScale.x) <= epsilon)
            {
                transform.localScale = _initialScale;
            }
            yield return null;
        }
    }

    IEnumerator Explosion(float radius)
    {
        float epsilon = 0.01f;
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.tag = "Explosion";
        sphere.transform.SetParent(transform, false);
        sphere.transform.position = transform.position;
        Vector3 targetScale = new Vector3(radius, radius, radius);
        while (sphere.transform.localScale.x < radius)
        {
            sphere.transform.localScale = Vector3.Lerp(sphere.transform.localScale, targetScale, 2f * Time.deltaTime);
            if (Mathf.Abs(sphere.transform.localScale.x - radius) <= epsilon)
            {
                sphere.transform.localScale = targetScale;
            }
            yield return null;
        }
    }
    
    IEnumerator HandleControlsInversion(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        _areControlsInversed = true;

        yield return new WaitForSeconds(duration);
        
        _areControlsInversed = false;
        yield return null;
        
    }
    
    IEnumerator RemoveCollisionForADuration(float duration)
    {
        playerCollision.enabled = false;
        yield return new WaitForSeconds(duration);
        playerCollision.enabled = true;
        _canBeInvincible = false;
        hasPowerUpToUse = false;
        yield return null;
        
    }

}
