using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Leap;
using Leap.Unity;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Player player;
    public List<GroundMovement> groundsMovement;
    public GameObject scoreTextGameObject;
    public AnimationCurve accelerationCurve;
    public LeapServiceProvider LeapServiceProvider;
    public Camera mainCam;

    private TextMeshProUGUI _scoreText;
    private float _tempScore;
    static public int score;
    private int _scorePerSecond;
    
    private int _maxScoreNumbers;
    private int _currentScoreNumbers;
    private int _numberOfZeros;

    private float _durationOfAcceleration;
    private float _currentTimer;
    private float _maxGroundSpeed;
    private float _minGroundSpeed;
    private float _minPlayerSideWaySpeed;
    private float _maxPlayerSideWaySpeed;
    private Ray _fingerRay;
    private Vector3 _fingerTipPosition;
    
    private void OnEnable()
    {
        LeapServiceProvider.OnUpdateFrame += OnUpdateFrame;
    }
    private void OnDisable()
    {
        LeapServiceProvider.OnUpdateFrame -= OnUpdateFrame;
    }

    void OnUpdateFrame(Frame frame)
    {
        if (player.IsDead)
        {
            foreach (var hand in frame.Hands)
            {
                if (!hand.IsLeft)
                {
                    Finger finger = hand.Fingers[1];
                    _fingerTipPosition = finger.TipPosition;
                    Vector3 fingerScreenSpacePos = mainCam.WorldToScreenPoint(_fingerTipPosition);
                    _fingerRay = mainCam.ScreenPointToRay(fingerScreenSpacePos);
                }
            }
        }
    }
    
    private void Awake()
    {
        _scoreText = scoreTextGameObject.GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        _tempScore = 0f;
        _scorePerSecond = 30;
        _maxScoreNumbers = 6;
        _numberOfZeros = _maxScoreNumbers;
        _currentScoreNumbers = 0;

        _durationOfAcceleration = 60f;
        _currentTimer = 0f;
        _minGroundSpeed = groundsMovement[0].forwardForce;
        _maxGroundSpeed = 1500f;
        _minPlayerSideWaySpeed = player.sidewayForce;
        _maxPlayerSideWaySpeed = 30f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        _tempScore += Time.deltaTime * (groundsMovement[0].forwardForce * 1/10);
        score = Mathf.RoundToInt(_tempScore);
        
        _currentScoreNumbers = score.ToString().Length;
        _numberOfZeros = _maxScoreNumbers - _currentScoreNumbers;
        if (_numberOfZeros == 0) {
            _maxScoreNumbers += 3;
        }
        String zeros = string.Join("", Enumerable.Repeat("0", _numberOfZeros));
        _scoreText.text = zeros + score;

        //Acceleration
        if (_currentTimer <= _durationOfAcceleration + 0.01f)
        {
            float groundRange = _maxGroundSpeed - _minGroundSpeed;
            float playerRange = _maxPlayerSideWaySpeed - _minPlayerSideWaySpeed;
            float currentTimerNormalized = NormalizeValue(_currentTimer, 0f, _durationOfAcceleration);
            float acceleration = accelerationCurve.Evaluate(currentTimerNormalized);
            
            player.sidewayForce = (playerRange * acceleration) + _minPlayerSideWaySpeed;
            foreach (GroundMovement gm in groundsMovement)
            {
                gm.forwardForce = (groundRange * acceleration) + _minGroundSpeed;
            }
        }
        _currentTimer += Time.deltaTime;

        if (player.IsDead) {
            foreach (GroundMovement gm in groundsMovement) {
                gm.forwardForce = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        if (player.IsDead)
        {
            Debug.Log("mort");
            Debug.DrawRay(_fingerRay.origin, _fingerRay.direction * 150, Color.red, 0);
        }
    }

    private float NormalizeValue(float value, float min, float max) {
        return Mathf.Abs(value - min) / (max-min);
    }
}
