using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;

public class GroundMovement : MonoBehaviour
{
    public float forwardForce;
    public List<GameObject> obstaclesTypes;
    public GameObject powerUpPrefab;
    public Player player;
    
    private Rigidbody _body;
    private Vector3 _newPosition;
    private List<GameObject> _obstacles;
    private GameObject _powerUp;
    List<float> _powerUpsZPositions;

    private void Awake() {
        _body = GetComponent<Rigidbody>();
        _obstacles = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _powerUpsZPositions = new List<float>{-37.5f, -12.5f, -12.5f, 12.5f, 12.5f, 37.5f};
        GenerateObstacles();
        GeneratePowersUp();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.z <= -50f) {
            transform.position = new Vector3(0f, 0f, 550f);
            UpdateObstacles();
            UpdatePowersUp();
        }
    }

    private void FixedUpdate()
    {
        _body.velocity = Vector3.zero;
        float acceleration = 1.0f;
        if (player.IsFalling) {
            acceleration = 2f;
        }
        _body.AddForce(0f, 0f, -forwardForce * acceleration * Time.deltaTime, ForceMode.VelocityChange);
    }

    private void GenerateObstacles()
    {
        for (float z = -50f; z < 50f; z+=25f)
        {
            GameObject newObstaclePrefab = obstaclesTypes[Random.Range(0, obstaclesTypes.Count)];
            float xPos = 0f;
            // Random offset
            switch (newObstaclePrefab.name) 
            {
                case "TripleLeft":
                    xPos = -3.9f;
                    break;
                case "TripleRight":
                    xPos = 3.9f;
                    break;
                case "Simple":
                    xPos = Random.Range(-4f, 4f);
                    break;
                case "Double":
                    xPos = Random.Range(-0.9f, 0.9f);
                    break;
            }
            Vector3 obstaclePos = new Vector3(xPos, 0.5f, z);
            Transform obstaclesContainer = transform.Find("Obstacles");
            GameObject newObstacle = Instantiate(newObstaclePrefab, obstaclePos, newObstaclePrefab.transform.rotation);
            newObstacle.transform.SetParent(obstaclesContainer.transform, false);
            _obstacles.Add(newObstacle);
        }
        if (transform.name == "FirstTile")
        {
            _obstacles[0].SetActive(false);
            _obstacles[1].SetActive(false);
        }
    }

    private void GeneratePowersUp()
    {
        
        float xPos = Random.Range(-3.5f, 3.5f);
        float zPos = _powerUpsZPositions[Random.Range(0, _powerUpsZPositions.Count)];
        Vector3 powerUpPos = new Vector3(xPos, 0.8f, zPos);
        Transform powerUpsContainer = transform.Find("PowerUps");
        GameObject newPowerUp = Instantiate(powerUpPrefab, powerUpPos, powerUpPrefab.transform.rotation);
        newPowerUp.transform.SetParent(powerUpsContainer.transform, false);
        _powerUp = newPowerUp;
        if (transform.name == "FirstTile")
        {
            _powerUp.SetActive(false);
        }
    }

    private void UpdatePowersUp()
    {
        
        _powerUp.SetActive(true);
        float xPos = Random.Range(-3.5f, 3.5f);
        float zPos = _powerUpsZPositions[Random.Range(0, _powerUpsZPositions.Count)];
        _powerUp.transform.localPosition = new Vector3(xPos, 0.8f, zPos);
    }

    private void UpdateObstacles()
    {
        if (transform.name == "FirstTile")
        {
            _obstacles[0].SetActive(true);
            _obstacles[1].SetActive(true);
        } 
        if (transform.name == "FirstTile") transform.name = "Tile";
        
        _obstacles.Shuffle();
        for (int i = 0; i < _obstacles.Count; i++)
        {
            float xPos = 0f;
            // Random offset
            switch (_obstacles[i].name) 
            {
                case "TripleLeft(Clone)":
                    xPos = -3.9f;
                    break;
                case "TripleRight(Clone)":
                    xPos = 3.9f;
                    break;
                case "Simple(Clone)":
                    xPos = Random.Range(-4f, 4f);
                    break;
                case "Double(Clone)":
                    xPos = Random.Range(-0.9f, 0.9f);
                    break;
            }
            foreach (Transform obstacle in _obstacles[i].transform) {
                Destroy(obstacle.GetComponent<Rigidbody>());
                obstacle.localPosition = obstacle.GetComponent<Obstacle>().InitialPosition;
                obstacle.localRotation = Quaternion.identity;
                obstacle.Rotate(new Vector3(1, 0, 0), 90);
            }
            _obstacles[i].transform.localPosition = new Vector3(xPos, 0.5f, -50f + i*25f);
        }
    }
}
