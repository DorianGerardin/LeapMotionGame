using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;


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
    public GameObject powerUpDisplayGameObject;
    public Sprite defautPowerUpImage;
    public GameObject powerLabelGameObject;
    public Material originalMat;
    public Material transparentMat;
    public GameObject ufoRoot;
    public GameObject timeRemainingUI;
    public Slider timeRemainingSlider;
    public Slider timeRemainingMalusSlider;
    public TextMeshProUGUI countDown;
    public Animator gestureAnimator;

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
    public bool AreControlsInversed => _areControlsInversed;
    private Image _powerUpDisplay;
    private TextMeshProUGUI _powerLabel;
    private bool _isInvincible;
    private float _currentLerpTimeMaterial = 0f;
    private int _currentMaterialSwitchIndex = 0;
    private float _currentTimeForPowerUp;
    private float _invincibilityDuration;
    private float _scaleDownDuration;
    private bool _isScaledDown;
    private float _inverseControlsDuration;
    
    private string _destructionAnimation;
    private string _jumpingAnimation;
    private string _invincibleAnimation;
    private string _pinchingAnimation;

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
        _powerUpDisplay = powerUpDisplayGameObject.transform.GetComponent<Image>();
        _powerLabel = powerLabelGameObject.transform.GetComponent<TextMeshProUGUI>();
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
        _isInvincible = false;
        _isScaledDown = false;
        _currentTimeForPowerUp = 0f;
        _invincibilityDuration = 7f;
        _scaleDownDuration = 5f;
        _inverseControlsDuration = 5f;
        _destructionAnimation = "isDestruction";
        _invincibleAnimation = "isInvincible";
        _jumpingAnimation = "isJump";
        _pinchingAnimation = "isPinch";
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
                    gestureAnimator.SetBool(_jumpingAnimation, false);
                    _hasJumped = true;
                    foreach (Transform rot in ufoRoot.transform) {
                        foreach (Transform ufoElement in rot.transform) {
                            if (ufoElement.name[0] == 'U') {
                                ufoElement.GetComponent<MeshRenderer>().material = transparentMat;
                            }
                        }
                    }
                }
                
                //Scale down
                if (_canScaleDown && hand.IsPinching())
                {
                    gestureAnimator.SetBool(_pinchingAnimation, false);
                    hasPowerUpToUse = true;
                    StartCoroutine(ScaleDownAndUpAfterDelay(_scaleDownDuration));
                    _canScaleDown = false;
                }
                
                //Destruct Obstacles
                if (_canDestructObstacles && hand.GrabStrength >= 0.99)
                {
                    gestureAnimator.SetBool(_destructionAnimation, false);
                    hasPowerUpToUse = true;
                    float sphereRadius = 100f;
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
                if (_canBeInvincible && handRotation.z is >= 160f and <= 190f)
                {
                    gestureAnimator.SetBool(_invincibleAnimation, false);
                    timeRemainingUI.SetActive(true);
                    _canBeInvincible = false;
                    _isInvincible = true;
                    StartCoroutine(RemoveCollisionForADuration(_invincibilityDuration));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasPowerUpToUse)
        {
            _powerUpDisplay.sprite = defautPowerUpImage;
            _powerLabel.text = "Pouvoir";
        }
        
        if (_isInvincible) {
            SwitchMaterialByInterval(0.2f);
            timeRemainingSlider.value = _currentTimeForPowerUp / _invincibilityDuration;
            _currentTimeForPowerUp += Time.deltaTime;
        }
        else {
            SetMaterial(originalMat);
        }

        if (_isScaledDown) {
            timeRemainingSlider.value = _currentTimeForPowerUp / _scaleDownDuration;
            _currentTimeForPowerUp += Time.deltaTime;
        }

        if (_areControlsInversed) {
            timeRemainingMalusSlider.value = _currentTimeForPowerUp / _inverseControlsDuration;
            _currentTimeForPowerUp += Time.deltaTime;
        }

        if (_isFalling) {
            SetMaterial(transparentMat);
        }
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
                SetMaterial(originalMat);
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
        Debug.Log("Invincibleeeeee");
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
        StartCoroutine(HandleControlsInversion(delay, _inverseControlsDuration));
        Debug.Log("InverseControls");
    }

    IEnumerator ScaleDownAndUpAfterDelay(float delay)
    {
        float epsilon = 0.5f;
        float scaledownSpeed = 5f;
        Vector3 newScale = _initialScale / 4f;
        while (transform.localScale.x > newScale.x)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, newScale, scaledownSpeed * Time.deltaTime);
            if (Mathf.Abs(transform.localScale.x - newScale.x) <= epsilon)
            {
                transform.localScale = newScale;
                timeRemainingUI.SetActive(true);
                _isScaledDown = true;
            }
            yield return null;
        }
        
        yield return new WaitForSeconds(delay);
        
        hasPowerUpToUse = false;
        _isScaledDown = false;
        StopTimerDisplay();
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

    IEnumerator HandleControlsInversion(float delay, float duration)
    {
        StartCoroutine(CountDown());
        yield return new WaitForSeconds(delay);
        _areControlsInversed = true;
        timeRemainingMalusSlider.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);
        
        SetMaterial(originalMat);
        countDown.gameObject.SetActive(false);
        _areControlsInversed = false;
        timeRemainingMalusSlider.gameObject.SetActive(false);
        _currentTimeForPowerUp = 0f;
        yield return null;
        
    }
    
    IEnumerator RemoveCollisionForADuration(float duration)
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        List<BoxCollider> obstacleColliders = new List<BoxCollider>();
        foreach (GameObject obstacle in obstacles) {
            BoxCollider collider = obstacle.transform.GetComponent<BoxCollider>();
            //obstacle.transform.GetComponent<BoxCollider>().enabled = false;
            obstacleColliders.Add(collider);
            obstacleColliders[^1].enabled = false;
        }
        yield return new WaitForSeconds(duration);
        foreach (BoxCollider obstacleCollider in obstacleColliders) {
            obstacleCollider.enabled = true;
        }
        _isInvincible = false;
        hasPowerUpToUse = false;
        StopTimerDisplay();
        yield return null;
    }

    private void SwitchMaterialByInterval(float interval)
    {
        if (_currentLerpTimeMaterial < interval)
        {
            _currentLerpTimeMaterial += Time.deltaTime;
        }
        else {
            _currentLerpTimeMaterial = 0f;
            _currentMaterialSwitchIndex = (_currentMaterialSwitchIndex + 1) % 2;
        }
        foreach (Transform rot in ufoRoot.transform) {
            foreach (Transform ufoElement in rot.transform) {
                if (ufoElement.name[0] == 'U') {
                    MeshRenderer rend = ufoElement.GetComponent<MeshRenderer>();
                    if (_currentMaterialSwitchIndex == 0) {
                        rend.material = transparentMat;
                    }
                    else {
                        rend.material = originalMat;
                    }
                }
            }
        }
    }
    
    IEnumerator CountDown()
    {
        countDown.gameObject.SetActive(true);
        countDown.fontSize = 180;
        for (int i = 3; i > 0; i--)
        {
            countDown.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        countDown.fontSize = 80;
        countDown.text = "Contrôles inversés!";
        yield return null;
    }

    private void SetMaterial(Material mat)
    {
        foreach (Transform rot in ufoRoot.transform) {
            foreach (Transform ufoElement in rot.transform) {
                if (ufoElement.name[0] == 'U') {
                    ufoElement.GetComponent<MeshRenderer>().material = mat;
                }
            }
        }
    }

    private void StopTimerDisplay() {
        _currentTimeForPowerUp = 0f;
        timeRemainingUI.SetActive(false);
        timeRemainingSlider.value = 0f;
    }

}
