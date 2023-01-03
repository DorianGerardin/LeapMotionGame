using System;
using System.Collections;
using System.Collections.Generic;
using PowerUps;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCollision : MonoBehaviour
{
    public GameObject powerLabelGameObject;
    public GameObject powerUpDisplayGameObject;
    public List<Sprite> powerUpsImages;
    public Animator gestureAnimator;

    private Player _player;
    private RandomPowerUpPickerManager _randomPowerUpPicker;
    private Image _powerUpDisplay;
    private TextMeshProUGUI _powerLabel;
    private string _destructionAnimation;
    private string _jumpingAnimation;
    private string _invincibleAnimation;
    private string _pinchingAnimation;
    private string _currentAnimation;


    private void Awake()
    {
        _player = transform.GetComponent<Player>();
        _powerUpDisplay = powerUpDisplayGameObject.transform.GetComponent<Image>();
        _powerLabel = powerLabelGameObject.transform.GetComponent<TextMeshProUGUI>();
    }

    private void OnCollisionEnter(Collision collisionInfo) {
        if (collisionInfo.collider.CompareTag("Respawn") || collisionInfo.collider.CompareTag("Obstacle"))
        {
            Debug.Log("collision obstacle");
            Die();
        }
        
    }

    private void OnTriggerEnter(Collider collisionInfo)
    {
        if(collisionInfo.CompareTag("PowerUp"))
        {
            if (!_player.hasPowerUpToUse)
            {
                collisionInfo.transform.gameObject.SetActive(false);
                IPowerUp powerUp = _player.AreControlsInversed ? _randomPowerUpPicker.PickOnlyPowerUp() : _randomPowerUpPicker.Pick();
                switch (powerUp.Label)
                {
                    case "Jump":
                        _powerUpDisplay.sprite = powerUpsImages[0];
                        _currentAnimation = _jumpingAnimation;
                        gestureAnimator.SetBool(_currentAnimation, true);
                        _powerLabel.text = "Saut";
                        break;
                    case "Destruct":
                        _powerUpDisplay.sprite = powerUpsImages[1];
                        _currentAnimation = _destructionAnimation;
                        gestureAnimator.SetBool(_currentAnimation, true);
                        _powerLabel.text = "Destruction";
                        break;
                    case "ScaleDown":
                        _powerUpDisplay.sprite = powerUpsImages[2];
                        _currentAnimation = _pinchingAnimation;
                        gestureAnimator.SetBool(_currentAnimation, true);
                        _powerLabel.text = "Retrécissement";
                        break;
                    case "Invincible":
                        _powerUpDisplay.sprite = powerUpsImages[3];
                        _currentAnimation = _invincibleAnimation;
                        gestureAnimator.SetBool(_currentAnimation, true);
                        _powerLabel.text = "Invincible";
                        break;
                    case "InverseControls":
                        //_powerUpDisplay.sprite = powerUpsImages[4];
                        _powerLabel.text = "Contrôles inversés";
                        break;
                }
                powerUp.Execute();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _randomPowerUpPicker = new RandomPowerUpPickerManager(_player);

        _destructionAnimation = "isDestruction";
        _invincibleAnimation = "isInvincible";
        _jumpingAnimation = "isJump";
        _pinchingAnimation = "isPinch";
        _currentAnimation = null;
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    private void Die()
    {
        _player.IsDead = true;
        SceneManager.LoadScene(2);
    }
}
