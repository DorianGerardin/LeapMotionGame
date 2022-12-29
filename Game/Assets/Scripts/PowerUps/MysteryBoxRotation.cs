using UnityEngine;

namespace PowerUps
{
    public class MysteryBoxRotation : MonoBehaviour
    {
        public float speed;
        public float amplitude;

        private Vector3 _initialPosition;

        void Start()
        {
            _initialPosition = transform.position;
        }

        void Update()
        {
            // Rotate the object on the Y axis
            transform.Rotate(0, Time.deltaTime * speed, 0);

            // Calculate the sinusoidal movement
            float y = _initialPosition.y + amplitude * Mathf.Sin(Time.time);

            // Apply the movement to the object's position
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
    }
}
