using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class MenuManager : MonoBehaviour
{
    
    public LeapServiceProvider LeapServiceProvider;
    public Button playButton;
    public Button quitButton;
    public Camera mainCam;

    private Ray _fingerRay;
    private Vector3 _fingerTipPosition;
    private Color _normalColor;
    private Color _highlightedColor;
    private Image _playButtonImg;
    private Image _quitButtonImg;
    private float _currentTimePointing;
    
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

    // Start is called before the first frame update
    void Start()
    {
        _fingerTipPosition = Vector3.zero;
        _playButtonImg = playButton.GetComponent<Image>();
        _quitButtonImg = quitButton.GetComponent<Image>();
        _normalColor = _playButtonImg.color;
        _highlightedColor = new Color(255, 255, 255, 255);
        _currentTimePointing = 0f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Debug.DrawRay(_fingerRay.origin, _fingerRay.direction * 150, Color.red, 0);
        if (Physics.Raycast(_fingerRay.origin, _fingerRay.direction, out var hit))
        {
            float activateTime = 1.5f;
            if (hit.collider.gameObject.name == "PlayCollider") {
                _playButtonImg.color = _highlightedColor;
                if (_currentTimePointing >= activateTime) {
                    SceneManager.LoadScene(1);
                }
            } 
            if (hit.collider.gameObject.name == "QuitCollider") {
                _quitButtonImg.color = _highlightedColor;
                if (_currentTimePointing >= activateTime) {
                    Application.Quit();
                }
            }
            _currentTimePointing += Time.deltaTime;
        }
        else {
            _playButtonImg.color = _normalColor;
            _quitButtonImg.color = _normalColor;
            _currentTimePointing = 0f;
        }
    }
}
