using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class MyCamera : MonoBehaviour
    {
        public Transform target;
        public float distance = 5.0f;
        public float xSpeed = 120.0f;
        public float ySpeed = 120.0f;

        public float yMinLimit = 10f;
        public float yMaxLimit = 80f;

        public float distanceMin = 3f;
        public float distanceMax = 4f;

        public StateManager states;

        private Rigidbody rigidbody;

        float x = 0.0f;
        float y = 0.0f;

        void Start()
        {
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;

            rigidbody = GetComponent<Rigidbody>();

            // Make the rigid body not change rotation
            if (rigidbody != null)
            {
                rigidbody.freezeRotation = true;
            }
        }

        void LateUpdate()
        {
            if (states.paused)
            {
                return;
            }
            if (target)
            {
                if (Input.GetAxis("Mouse X") == 0 && Input.GetAxis("Mouse Y") == 0)
                {
                    x += Input.GetAxis("RightAxis X") * xSpeed * distance * 0.02f;
                    y -= Input.GetAxis("RightAxis Y") * ySpeed * 0.02f;
                }
                else if (Input.GetAxis("RightAxis X") == 0 && Input.GetAxis("RightAxis Y") == 0)
                {
                    x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
                    y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                }

                y = ClampAngle(y, yMinLimit, yMaxLimit);

                Quaternion rotation = Quaternion.Euler(y, x, 0);

                distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

                RaycastHit hit;
                if (Physics.Linecast(target.position, transform.position, out hit))
                {
                    distance -= hit.distance;
                }
                Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                Vector3 position = rotation * negDistance + target.position;

                transform.rotation = rotation;
                transform.position = position;
            }
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}