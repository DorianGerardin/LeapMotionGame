using System;
using System.Collections;
using System.Collections.Generic;
using PowerUps;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollision : MonoBehaviour
{
    private Player _player;
    private RandomPowerUpPickerManager _randomPowerUpPicker;

    private void Awake()
    {
        _player = transform.GetComponent<Player>();
    }

    private void OnCollisionEnter(Collision collisionInfo) {
        if (collisionInfo.collider.CompareTag("Respawn") || collisionInfo.collider.CompareTag("Obstacle"))
        {
            Debug.Log("collision obstacle");
            Respawn();
        }
        
    }

    private void OnTriggerEnter(Collider collisionInfo)
    {
        if(collisionInfo.CompareTag("PowerUp"))
        {
            if (!_player.hasPowerUpToUse)
            {
                collisionInfo.transform.gameObject.SetActive(false);
                IPowerUp powerUp = _randomPowerUpPicker.Pick();
                powerUp.Execute();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _randomPowerUpPicker = new RandomPowerUpPickerManager(_player);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void Respawn()
    {
        SceneManager.LoadScene(0);
    }
}
